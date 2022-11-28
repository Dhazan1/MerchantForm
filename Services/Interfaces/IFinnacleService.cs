using MerchantForm.Models.WebMarchant;
using System.Threading.Tasks;

namespace MerchantForm.Services.Interfaces
{
    public interface IFinnacleService
    {
        Task<object> GetTransaction(TransactionModel model);
        Task<object> GetBranch(string branch);
    }
}