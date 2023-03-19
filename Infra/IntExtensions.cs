namespace Infra;

public static class IntExtensions
{
    public static T ToEnum<T>(this int value)
    {
        return ((long)value).ToEnum<T>();
    }

    public static bool IsValidFor<T>(this int value, bool enumHasZero = false)
    {
        return ((long)value).IsValidFor<T>(enumHasZero);
    }

    public static bool CanBeCastTo<T>(this int value)
    {
        var type = typeof(T);
        if (!type.IsEnum)
        {
            return false;
        }
        var canBeCast = Enum.GetValues(type).Cast<int>().ToList().Contains((int)value);
        if (!canBeCast)
        {
            return false;
        }
        return true;
    }

    public static string DigitGroup(this int value)
    {
        return ((decimal)value).DigitGroup();
    }

    public static string Abbreviate(this int value)
    {
        return ((long)value).Abbreviate();
    }

    public static string DigitGroup(this int? value)
    {
        if (value == null)
        {
            return "";
        }
        return ((decimal)value).DigitGroup();
    }
}
