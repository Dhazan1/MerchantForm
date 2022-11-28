using AutoMapper;
using Mapster;
using MerchantForm.Contexts.WebMarchant;
using MerchantForm.MapProfile;
using MerchantForm.Model.ViewModel;
using MerchantForm.Models.WebMarchant;
using MerchantForm.Services.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;

namespace MerchantForm.Services
{
    public class FormService : IFormService

    {
        private WebmerchantContext dataContext;
        private readonly IEmailService _emailService;
        public FormService(WebmerchantContext _dataContext, IEmailService emailService)
        {
            dataContext = _dataContext;
            _emailService = emailService;
        }

        public async Task<string> FormPost(MerchantFormDto formDetail)
        {
            try
            {
                var type = formDetail.Image.ContentType;
                string pattern = @"[.]";
                var reg = new Regex(pattern);
                var file = formDetail.Image.FileName.ToString();
                var count = reg.Matches(file).Count();
                if (count > 1) { return ("Please rename your file"); }

                //mapping
                MerchantDetail detail = new MerchantDetail();
                detail = formDetail.Adapt<MerchantDetail>();
                //  detail = mapper.Map<MerchantDetail>(formDetail);         
                using (var ms = new MemoryStream())
                {
                    formDetail.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    detail.FileName = fileBytes;
                }
                await dataContext.MerchantDetails.AddAsync(detail);
                foreach (var director in formDetail.DirectorMOdel)
                {
                    var directorModel = new Director();
                    directorModel.Address = director.Address;
                    directorModel.Name = director.Name;
                    directorModel.Merchant = detail;
                    await dataContext.Directors.AddAsync(directorModel);
                }
                
                var response = await dataContext.SaveChangesAsync();
                if (response >= 1)
                {
                    var model = new SendMailModel();
                    model.From = "noreply@wemabank.com";
                    model.To = detail.EmailAddress;
                    model.Subject = "Web Merchant Notification";
                    model.Body = $"Dear {formDetail.AccountName}, <br> We are currently reviewing your request, we will notify you of the status of your request once the review is completed.<br>" +
                        $" Best Regards";
                    await _emailService.SendEmail(model);
                }
                return ("succesfull");
            }
            catch (Exception ex) { return ex.Message; }
        }

        public async Task<object> GetForm(string accountNumber)
        {
            var merchant = new MerchantDetail();
            var directors = new List<Director>();
            try
            {
                merchant = await Task.Run(() => dataContext.MerchantDetails.Where(x => x.AccountNumber == accountNumber).FirstOrDefault());
            }
            catch (Exception ex) { return ex.Message; }

            if( merchant == null) return ("No details was found for the specified account number");

            try
            {
                directors = await Task.Run(() => dataContext.Directors.Where(x => x.MerchantId == merchant.Id).ToList());
            }
            catch (Exception ex) { return ex.Message; }
            if (merchant == null) return ("No result found");           
            OutputMerchantView dataMerchant = merchant.Adapt<OutputMerchantView>();
            var dataDirector = directors.Adapt<List<OutputDirectorView>>();
            var model = new
            {
                merchantModel = dataMerchant,
                directorModel = dataDirector,
            };
            return model;
        }

        public async Task<object> GetSignature(string accountNumber)
        {
            var merchant = await Task.Run(() => dataContext.MerchantDetails.Where(x => x.AccountNumber == accountNumber).FirstOrDefault());
            if (merchant == null) { return "No merchant was found"; }
            byte[] imgData = merchant.FileName;
            var base64File = Convert.ToBase64String(imgData, 0, imgData.Length, Base64FormattingOptions.None);


            //MemoryStream ms= new MemoryStream(imgData,0,imgData.Length);
            //return Image.FromStream(ms,true);


            return (base64File);
        }
    }
}
