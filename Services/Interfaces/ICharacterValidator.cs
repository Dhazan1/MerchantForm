namespace MerchantForm.Services
{
    public interface ICharacterValidator
    {
        bool ClassDigitVerifier(object Val);
        bool ClassLetterVerifier(object Val);
        bool DigitVerifier(string Val);
        bool LetterVerifier(string Val);
        bool LetterAndNumberVerifier(string Val);
        bool DateVerifier(string Val);
        bool EmailValidator(string email);
        bool InformationValidator(string value);
    }
}