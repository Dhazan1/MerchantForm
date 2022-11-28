using ContextFormData;
using MerchantForm.Models.WebMarchant;
using System.Collections;
using System.Collections.Generic;

namespace MerchantForm.Converter
{
    public class DirectorConcatService : IDirectorConcatService
    {
        public string ListToStringName(List<Director> directors)
        {
            string directorName = "";
            foreach (Director director in directors)
            {
                directorName += director.Name.ToString()+"||" ;
            }
            return directorName;
        }
        public string ListToStringAddress(List<Director> directors)
        {
            string directorAddress = "";
            foreach (Director director in directors)
            {
                directorAddress +=  director.Address.ToString() + "||";
            }
            return directorAddress;
        }
        public IEnumerable<Director> DirectorsList(string directorName,string directorAddress)
        {
            List<Director> directors = new List<Director>(); 
            string[] names=directorName.Split("||");
            string[] addresses = directorAddress.Split("||");
            foreach (string name in names) 
            {
                int i=0;
                string address = addresses[i];
                i++;
                Director director = new Director();
                director.Name = name;
                director.Address = address;
                directors.Add(director);
            }        
            return directors;   
        }

    }
}
