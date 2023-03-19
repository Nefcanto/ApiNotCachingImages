namespace Infra;

public static class UniversalDateTime
{
    public static DateTime Parse(string text)
    {
        var date = DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        return date;
    }

    public static DateTime Now
    {
        get
        {
            var dateTime = DateTime.Now;
            return dateTime;
        }
    }
}