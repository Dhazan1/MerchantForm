using FluentValidation;
using MerchantForm.MapProfile;
using Microsoft.AspNetCore.Http;
using System;

namespace MerchantForm.Validator
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(x => x.Length).NotNull().GreaterThanOrEqualTo(100)
                .WithMessage("File size is larger than allowed");
            RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("File type is not allowed");
        }

    }
    public class StringValidator : AbstractValidator<String>
    {
        public StringValidator()
        {
            RuleFor(x => x).NotNull().WithMessage("String cannot be empty");
            RuleFor(x => x.Length).LessThanOrEqualTo(50).WithMessage("String size is larger than allowed");

        }
    }
    public class ImageValidator : AbstractValidator<MerchantFormDto>
    {
        public ImageValidator()
        {
            RuleFor(x => x.Image).SetValidator(new FileValidator());

        }

    }
    public class WordValidator : AbstractValidator<MerchantFormDto>
    {
        public WordValidator()
        {
            RuleFor(x => x.DestBranch).SetValidator(new StringValidator());
            RuleFor(x => x.EmailAddress).SetValidator(new StringValidator());
            RuleFor(x => x.SourceBranch).SetValidator(new StringValidator());
            RuleFor(x => x.AccountName).SetValidator(new StringValidator());
            RuleFor(x => x.AccountNumber).SetValidator(new StringValidator());
            RuleFor(x => x.State).SetValidator(new StringValidator());
            RuleFor(x => x.WebInformation).SetValidator(new StringValidator());
        }

    }
}
