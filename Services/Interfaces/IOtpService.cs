using MerchantForm.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MerchantForm.Services
{
    public interface IOtpService
    {
        Task<object> SendOtp(string accountNumber);
        Task<string> VerifyOtp([FromBody] OtpViewModel otpcheck);
    }
}