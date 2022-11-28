using MerchantForm.MapProfile;
using System.Threading.Tasks;

namespace MerchantForm.Services
{
    public interface IFormService
    {
        Task<string> FormPost(MerchantFormDto formDetail);
        Task<object> GetForm(string accountNumber);
        Task<object> GetSignature(string accountNumber);
    }
}