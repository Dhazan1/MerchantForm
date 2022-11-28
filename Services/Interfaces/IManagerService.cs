using MerchantForm.Model;
using System.Threading.Tasks;

namespace MerchantForm.Services
{
    public interface IManagerService
    {
        Task<object> MailToManager(ManagerViewModel managermodel);
    }
}