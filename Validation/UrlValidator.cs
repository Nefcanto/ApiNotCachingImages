namespace Validation;

public class UrlValidator
{
    public static bool IsUrl(string text)
    {
        Uri uri;
        if (Uri.TryCreate(text, UriKind.Absolute, out uri))
        {
            return true;
        }
        return false;
    }
}
