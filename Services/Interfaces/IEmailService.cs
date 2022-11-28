using MerchantForm.Model.ViewModel;
using System.Threading.Tasks;

namespace MerchantForm.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmail(SendMailModel data);
    }
}
