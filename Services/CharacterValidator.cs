using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MerchantForm.Services
{
    public class CharacterValidator : ICharacterValidator
    {

        public bool ClassDigitVerifier(Object Val)
        {
            Type type = Val.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo p in props)
            {
                string pValue = p.GetValue(Val, null).ToString();

                var check = !SqlValidator(pValue);
                if (check) return false;

                if (pValue == null) { return false; }
                IEnumerable<char> charValue = pValue;
                var value = charValue.All(ch => Char.IsDigit(ch));
                if (value == false) { return false; }
            }
            return true;
        }
        public bool ClassLetterVerifier(Object Val)
        {
            Type type = Val.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo p in props)
            {             
                string pValue = p.GetValue(Val, null).ToString();

                var check = !SqlValidator(pValue);
                if (check) return false;

                if (pValue == null) { return false; }
                IEnumerable<char> charValue = pValue;
                var value = charValue.All(ch => Char.IsLetter(ch));
                if (value == false) { return false; }
            }
            return true;
        }

        public bool DigitVerifier(string Val)
        {
            var check = !SqlValidator(Val);
            if (check) return false;

            if (Val== null) { return false; }
            IEnumerable<char> digitValues = from ch in Val where Char.IsDigit(ch) select ch;
            if (digitValues.Count() != Val.Length) { return false; }
            return true;
        }

        public bool LetterVerifier(string Val)
        {
            var check = !SqlValidator(Val);
            if (check) return false;

            if (Val == null) { return false; }
            IEnumerable<char> letterValues = from ch in Val where Char.IsLetter(ch) select ch;
            if (letterValues.Count() != Val.Length) { return false; }
            return true;
        }

        public bool LetterAndNumberVerifier(string Val)
        {
            var check= !SqlValidator(Val);
            if (check) return false;

            if (Val == null) { return false; }
            IEnumerable<char> letterValues = from ch in Val where Char.IsLetterOrDigit(ch) select ch;
            if (letterValues.Count() != Val.Length) { return false; }
            return true;
        }
        public bool DateVerifier(string Val)
        {

            var check = !SqlValidator(Val);
            if (check) return false;

            if (Val == null || Val.Length > 10) { return false; }
            try
            {
                Val.Replace("/", "-");
                var date = DateTime.ParseExact(Val,"dd-MM-yyyy", CultureInfo.InvariantCulture);
                //DateTime.Parse(Val);
                if (date.GetType() == typeof(DateTime)) return true;
                return false;
            }
            catch (Exception) { return false; }
        }

        public bool EmailValidator(string email)
        {
            var check = !SqlValidator(email);
            if (check) return false;

            if (email == null || email.Length > 30) { return false; }
            if(!email.Contains('@')||!email.Contains('.')) return false;
            
            email= email.Replace('@', '9').Replace('.', '9');
            IEnumerable<char> letterValues = from ch in email where Char.IsLetterOrDigit(ch) select ch;
            if (letterValues.Count() != email.Length) { return false; }
            return true;
          
        }

        public bool InformationValidator(string value)
        {
            var check = !SqlValidator(value);
            if (check) return false;

            if (value == null || value.Length > 30) { return false; }
            value = value.Replace(',', '9').Replace('.', '9').Replace(' ', '9').Replace('-','9');
            value=value.Trim();
            IEnumerable<char> letterValues = from ch in value where Char.IsLetterOrDigit(ch) select ch;
            if (letterValues.Count() != value.Length) { return false; }
            return true;

        }


        private bool SqlValidator(string query)
        {

            string[] pattern = {"SELECT", "UPDATE", "DELETE", "INSERT", "CREATE", "ALTER", "TABLE", "DROP", "DATABASE"};


            var check = true;
            foreach (var item in pattern) { 
              if (Regex.IsMatch(query, item, RegexOptions.IgnoreCase))
                {
                    check = false;
                };
            }
           

            return check;
        }


    }
}
