namespace Validation;

public class EmailValidator
{
    public static bool IsEmail(string text)
    {
        if (text.IsNothing())
        {
            return false;
        }
        var regex = new Regex(RegularExpressions.Email);
        if (!regex.Match(text).Success)
        {
            return false;
        }
        return true;
    }
}
