using ContextFormData;
using MerchantForm.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Drawing;

namespace MerchantForm.MapProfile
{

    public class MerchantFormDto
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string EmailAddress { get; set; }
        public string State { get; set; }
        public string SourceBranch { get; set; }
        public string DestBranch { get; set; }
        public string WebInformation { get; set; }
        public List<DirectorModel> DirectorMOdel { get; set; }
        public IFormFile Image {get; set;}
        public string Otp { get; set; }



    }
}
