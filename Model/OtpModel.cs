using System;

namespace MerchantForm.Model
{
    public class OtpRequestModel
    {
        public int Id { get; set; }
        public string Otp { get; set; }
        public string Email { get; set; }
        public string Date { get; set; }
    }
}
