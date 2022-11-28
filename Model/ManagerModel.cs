using System.ComponentModel.DataAnnotations;

namespace MerchantForm.Model
{
    public class ManagerModel
    {

        [Key]
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ManagerEmail { get; set; }

    }
}
