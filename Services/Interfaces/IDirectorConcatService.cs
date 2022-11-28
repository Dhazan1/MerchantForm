using ContextFormData;
using MerchantForm.Models.WebMarchant;
using System.Collections.Generic;

namespace MerchantForm.Converter
{
    public interface IDirectorConcatService
    {
        string ListToStringAddress(List<Director> directors);
        string ListToStringName(List<Director> directors);
        public IEnumerable<Director> DirectorsList(string directorName, string directorAddress);
    }
}