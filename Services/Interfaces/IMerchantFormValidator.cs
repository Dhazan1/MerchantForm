using MerchantForm.MapProfile;

namespace MerchantForm.Services
{
    public interface IMerchantFormValidator
    {
        string Validator(MerchantFormDto formDetail);
    }
}