using MerchantForm.MapProfile;
using System.Linq;
using System.Text.RegularExpressions;

namespace MerchantForm.Services
{
    public class MerchantFormValidator : IMerchantFormValidator
    {
        private readonly ICharacterValidator characterValidator;
        public MerchantFormValidator(ICharacterValidator _characterValidator)
        {
            characterValidator = _characterValidator;
        }

        public string Validator(MerchantFormDto formDetail)
        {

            // validations

            if (formDetail == null) { return ("Please input a details"); }

            if (!characterValidator.InformationValidator(formDetail.AccountName) || formDetail.AccountName.Length > 30) return ("Please input a valid AccountName");
            if (!characterValidator.DigitVerifier(formDetail.AccountNumber) || formDetail.AccountNumber.Length != 10) return ("Please input a valid AccountNumber");
            if (!characterValidator.LetterVerifier(formDetail.State)) return ("Please input a valid state");
            if (!characterValidator.EmailValidator(formDetail.EmailAddress)) return ("Please input a valid Email");
            if (!characterValidator.InformationValidator(formDetail.SourceBranch)|| formDetail.SourceBranch.Length > 15) return ("Please input a valid Source Branch ");
            if (!characterValidator.InformationValidator(formDetail.DestBranch) || formDetail.DestBranch.Length > 15) return ("Please input a valid Destination Branch ");
            if (!characterValidator.InformationValidator(formDetail.WebInformation) || formDetail.WebInformation.Length > 25) return ("Please input a limited WebInformation");
            if (!formDetail.DirectorMOdel.All(item => characterValidator.InformationValidator(item.Name)  || item.Name.Length > 20)) return ("Please input a valid Director Name");
            if (!formDetail.DirectorMOdel.All(item => characterValidator.InformationValidator( item.Address)|| item.Name.Length > 30)) return ("Please input a valid Director Address");
            if (!characterValidator.DigitVerifier(formDetail.Otp) || formDetail.Otp.Length != 6) return ("Please enter a valid Otp");
            if (formDetail.Image == null) return ("Signature not inserted");


            //validation for image
            if (formDetail.Image.ContentDisposition == null) return "Invalid image";

            var type = formDetail.Image.ContentType;
            string[] fileExtensions = { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
            var imageValid = false;
            var typeValid = false;
            foreach (var ext in fileExtensions)
            {
                if (formDetail.Image.FileName.Contains(ext))
                {
                    imageValid = true;
                }
               
            }
            if (formDetail.Image.Length > 100000) return "Maximum image size is 100kb";

            string[] fileExtension = { "jpg", "png", "jpeg", "JPG", "PNG", "JPEG" };
            foreach (var ext in fileExtension)
            {
                if (type == $"image/{ext}")
                {
                    typeValid = true;
                }
            }


            string pattern = @"[.]";
            var reg = new Regex(pattern);
            var file = formDetail.Image.FileName.ToString();
            var count = reg.Matches(file).Count();
            if (count > 1) { return ("Please rename your file"); }
            if (!imageValid || !typeValid) { return ("Image invalid"); }

            return "valid";
        }
    }
}
