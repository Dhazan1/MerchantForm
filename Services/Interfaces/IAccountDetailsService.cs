using MerchantForm.Model;
using System.Threading.Tasks;

namespace MerchantForm.Services
{
    public interface IAccountDetailsService
    {
        Task<object> GetAcctDetails(string accNum);
    }
}