using Mapster;
using MerchantForm.Contexts.WebMarchant;
using MerchantForm.Model;
using MerchantForm.Model.ViewModel;
using MerchantForm.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using MerchantForm.Models.WebMarchant;
using MerchantForm.Services.Interfaces;

namespace MerchantForm.Services
{
    public class OtpService : IOtpService
    {
        private readonly IAccountDetailsService _accountDetailsService;
        private readonly WebmerchantContext dataContext;
        private readonly IEmailService _emailService;
        private readonly IFinnacleService finnacleService;
        private readonly ICharacterValidator validator;
        public OtpService(IAccountDetailsService accountDetailsService, IEmailService emailService, ICharacterValidator _validator, IFinnacleService _finnacleService, WebmerchantContext _dataContext)
        {
            _emailService=emailService;
            _accountDetailsService = accountDetailsService;
            validator = _validator;
            finnacleService = _finnacleService;
            dataContext = _dataContext;
        }

        public async Task<object> SendOtp(string accountNumber)
        {
            try
            {
                var content = await _accountDetailsService.GetAcctDetails(accountNumber);
                if (content.Equals("Invalid")) return "Account number should be ten digits";
                if (content.Equals("BadRequest")) return "No account details was found for the specified account";
                var data = content.Adapt<AcctResponseRequired>();
                Random rand = new Random();
                string otp = rand.Next(0, 999999).ToString();
                var otpUpdate = await Task.Run(() => dataContext.OtpModels.Where(x => x.Email == data.cemail).FirstOrDefault());
                if (otpUpdate != null)
                {
                    otpUpdate.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    otpUpdate.Otp = otp;
                    otpUpdate.Email = data.cemail;
                    await Task.Run(() => dataContext.OtpModels.Update(otpUpdate));
                }
                else
                {
                    // var otpModel = new OtpRequestModel();
                    var otpModel = new OtpModel();
                    otpModel.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    otpModel.Otp = otp;
                    otpModel.Email = data.cemail;
                    await dataContext.OtpModels.AddAsync(otpModel);
                }
                await dataContext.SaveChangesAsync();
                var model = new SendMailModel();
                model.From = "noreply@wemabank.com";
                model.To = data.cemail;
                model.Subject = "WEB MERCHANT";
                model.Body = $"Dear customer, <br> your otp is {otp} <br>";
                await _emailService.SendEmail(model);
                return "Otp sent successfully";
            }
            catch (Exception ex) { return ex.Message; }
        }


        public async Task<string> VerifyOtp( OtpViewModel otpcheck)
        {
            try
            {
                if(otpcheck.Otp.Length!= 6) { return "Please enter a valid Otp"; }
                var otpModel = await Task.Run(() => dataContext.OtpModels.Where(x => x.Email == otpcheck.Email).FirstOrDefault());
                if (otpModel == null) return "No Otp was found";
                var timer = DateTime.Parse(otpModel.Date).AddMinutes(10);

                var now=DateTime.Now;

                if(timer <now) return "Otp expired";
               
                var otpValid = otpModel.Otp.Equals(otpcheck.Otp);

                if (otpValid)
                {
                    return "00";
                }
                return "Invalid otp";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}
