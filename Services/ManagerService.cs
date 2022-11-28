using MerchantForm.Contexts.WebMarchant;
using MerchantForm.Model;
using MerchantForm.Model.ViewModel;
using MerchantForm.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MerchantForm.Services
{
    public class ManagerService : IManagerService

    {
        private readonly ICharacterValidator validatorService;
        private readonly WebmerchantContext dataContext;
        private readonly IEmailService _emailService;

        public ManagerService(ICharacterValidator _validatorService, IEmailService emailService, WebmerchantContext _dataContext)
        {
            validatorService = _validatorService;
            _emailService = emailService;
            dataContext = _dataContext;
        }

        public async Task<object> MailToManager(ManagerViewModel managermodel)
        {


            if (!validatorService.InformationValidator(managermodel.Branch)) return ("Please enter a valid branch name");
            try
            {
                var link = "https://purpleworks.wemabank.com/Runtime/Runtime/Form/K2+Workdesk/?";
                var data = await Task.Run(() => dataContext.ManagerModels.Where(x => x.BranchName == managermodel.Branch).FirstOrDefault());
                var managerEmail = data.ManagerEmail;
                await Task.Run(() =>
                {
                    var model = new SendMailModel();
                    model.From = "noreply@wemabank.com";
                    model.To = managerEmail;  //manager gotten from db
                    model.Subject = "Web Merchant Notification";
                    model.Body = $"Dear Sir/Ma, <br>" +
                        $"You have a pending request for web merchant onboarding,with the following account number {managermodel.AccountNumber}<br><br>" +
                        $" kindly log on to your work desk to access it or click on this link, <a href =\"{link}\"> click here </a> <br><br>" +
                        $"Best Regards";
                    _emailService.SendEmail(model);
                });
                return (200);
            }
            catch (Exception ex) { return (ex.Message); }


        }
    }
}
