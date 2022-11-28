
using MerchantForm.Context;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Drawing;
using AutoMapper;
using MerchantForm.Converter;
using System.Linq;
using MerchantForm.MapProfile;
using System.Net;
using System.Net.Http;
using System;
using MerchantForm.Services.Interfaces;
using MerchantForm.Model.ViewModel;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using System.Collections;
using Dapper;
using MerchantForm.Model;
using MerchantForm.Services;
using MerchantForm.Models.WebMarchant;
using MerchantForm.Contexts.WebMarchant;
using System.Collections.Generic;
using Mapster;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using MerchantForm.Models;
using System.Reflection.Metadata;
//using Oracle.DataAccess.Client;

namespace MerchantForm.Controllers
{

    [ApiController]
    [Route("/Merchant/[Controller]")]
    public class FormController : ControllerBase
    {
        private WebmerchantContext dataContext;
        private IMapper mapper;
        private readonly IAccountDetailsService _accountDetailsService;
        private readonly IManagerService managerService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration configuration;
        private readonly IFinnacleService finnacleService;
        private readonly ICharacterValidator validatorService;
        private readonly IFormService formService;
        private readonly IOtpService otpService;
        private readonly IMerchantFormValidator formPostValidator;
        public FormController(IAccountDetailsService accountDetailsService, IManagerService _managerService, IMerchantFormValidator _formPostValidator, IOtpService _otpService, IFormService _formService, ICharacterValidator _validatorService, IFinnacleService _finnacleService, WebmerchantContext _dataContext, IMapper _mapper, IImageAndByteArrayConverter _converter, IEmailService emailService, IConfiguration _configuration)
        {
            managerService = _managerService;
            formService = _formService;
            _accountDetailsService = accountDetailsService;
            mapper = _mapper;
            dataContext = _dataContext;
            _emailService = emailService;
            configuration = _configuration;
            finnacleService = _finnacleService;
            validatorService = _validatorService;
            otpService = _otpService;
            formPostValidator = _formPostValidator;
        }

        [HttpPost("/post/details")]
        public async Task<IActionResult> FormPost([FromForm] MerchantFormDto formDetail)
        {
            var message = "";
            var check = formPostValidator.Validator(formDetail);
            if (check == "valid")
            {
                var response = await otpService.VerifyOtp(new OtpViewModel { Email = formDetail.EmailAddress, Otp = formDetail.Otp });
                if (response == "00") message = await formService.FormPost(formDetail);
                else message = response;
                await managerService.MailToManager(new ManagerViewModel{AccountNumber=formDetail.AccountNumber,Branch=formDetail.DestBranch});
                return Ok(new { message = message });
        }
            else return BadRequest(check);


        }
        [HttpGet("/GetAccountDetails")]
        public async Task<ActionResult> GetAcctDetails([FromQuery] string accNum)
        {
            if (!validatorService.DigitVerifier(accNum) || accNum.Length != 10) return BadRequest("Please input Valid Account number ");
            var content = await _accountDetailsService.GetAcctDetails(accNum);
            if (content.ToString() == ("BadRequest")) return BadRequest("No account details was found for the specified account number");
            return Ok(content);
        }

        [HttpGet("/Get/Details")]
        public async Task<IActionResult> FormGet([FromQuery] string accountNumber)
        {
            if (!validatorService.DigitVerifier(accountNumber) || accountNumber.Length != 10) return BadRequest("Please input Valid Account number ");
            var response = await formService.GetForm(accountNumber);
            return Ok(response);
        }

        [HttpGet("/GetSignature")]
        public async Task<ActionResult> FormGetSignature([FromQuery] string accountNumber)
        {
            if (!validatorService.DigitVerifier(accountNumber) || accountNumber.Length != 10) return BadRequest("Please input Valid Account number ");
            var response = await formService.GetSignature(accountNumber);         
            return Ok(response);
        }

        [HttpPost("/MailToManager")]
        public async Task<IActionResult> ManagerMail([FromBody] ManagerViewModel managermodel)

        {
           
            var response=managerService.MailToManager(managermodel);
            if (response.ToString() == "200") return Ok(response);
            else return BadRequest(response);
           ;
        }

        [HttpPost("/SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] AccountNumber acctNum)
        {
            if (!validatorService.ClassDigitVerifier(acctNum)) { return BadRequest("Please input a valid account number"); }
            var response = await otpService.SendOtp(acctNum.AccountNum);
            return Ok(response);
        }

        [HttpPost("/VerifyOtp")]
        public async Task<ActionResult> VerifyOtp([FromBody] OtpViewModel otpcheck)
        {
            if (!validatorService.DigitVerifier(otpcheck.Otp)||!validatorService.EmailValidator(otpcheck.Email)) return BadRequest("Please enter a valid Otp details");
            var response = await otpService.VerifyOtp(otpcheck);
            return Ok(response);
        }

        [HttpGet("/GetBranches")]
        public async Task<ActionResult> GetBranch([FromQuery] string state)
        {
            if(!validatorService.LetterAndNumberVerifier(state)) { return BadRequest("Please input a valid branch name"); }
            var result = await Task.Run(() => finnacleService.GetBranch(state));
            return Ok(result);
        }

        [HttpGet("/GetTransactions")]
        public async Task<ActionResult> GetTransactions([FromQuery] TransactionModel details)
        {
            if (!validatorService.LetterAndNumberVerifier(details.TransId) || details.TransId.Length > 9) return BadRequest("Please enter a valid Transaction Id");
            if (!validatorService.DateVerifier(details.TransDate)) return BadRequest("Please enter a valid Transaction Date");
            var response = await Task.Run(() => finnacleService.GetTransaction(details));
            if (Convert.ToString(response) == "Nil") return BadRequest(new { StatusCode = 404, message = "No transaction was found" });
            return Ok(response);

        }

    }
}