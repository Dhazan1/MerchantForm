using MerchantForm.Models.WebMarchant;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MerchantForm.MapProfile
{
    public class OutputMerchantView
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string EmailAddress { get; set; }
        public string State { get; set; }
        public string SourceBranch { get; set; }
        public string WebInformation { get; set; }
        public string DestBranch { get; set; }
    }
    public class OutputDirectorView {
    public string Name { get; set; }
    public string Address { get; set; }
    }
}

