namespace Infra;

public static class StringExtensions
{
    public static bool IsNothing(this string text)
    {
        var isNothing = string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
        return isNothing;
    }

    public static bool IsSomething(this string text)
    {
        var isSomething = !text.IsNothing();
        return isSomething;
    }

    public static T ToEnum<T>(this string value)
    {
        if (typeof(T).BaseType.Name != typeof(Enum).Name)
        {
            throw new Exception("Input type of generic method ToEnum<T>() is not an Enum");
        }
        if (value.IsNumeric())
        {
            var number = (long)value.ToLong();
            if (!number.IsValidFor<T>())
            {
                throw new Exception($"Enum {typeof(T).Name} does not have a numeric value of {value}");
            }
        }
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static int ToInt(this string text)
    {
        int result = 0;
        int.TryParse(text, out result);
        return result;
    }

    public static long ToLong(this string text)
    {
        long result = 0;
        long.TryParse(text, out result);
        return result;
    }

    public static decimal ToDecimal(this string text)
    {
        decimal result = 0;
        decimal.TryParse(text, out result);
        return result;
    }

    public static JsonElement Deserialize(this string json)
    {
        var rootElement = JsonDocument.Parse(json).RootElement;
        return rootElement;
    }

    public static JsonNode ParseJson(this string json)
    {
        return JsonNode.Parse(json);
    }

    public static T Deserialize<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonHelper.Options);
    }

    public static List<T> SplitCsv<T>(this string text)
    {
        List<T> result = new List<T>();
        if (text.IsNothing())
        {
            return result;
        }
        List<string> tokens = text.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
        tokens.ForEach(t =>
        {
            try
            {
                result.Add((T)Convert.ChangeType(t, typeof(T)));
            }
            catch
            {
                Logger.LogError($"Error in converting item {t} to type {typeof(T)} in SplitCsv<T>() method");
            }
        });
        return result;
    }

    public static List<string> SplitCsv(this string text)
    {
        return SplitCsv<string>(text);
    }

    public static bool CanBeCastTo<T>(this string value)
    {
        var type = typeof(T);
        if (!type.IsEnum)
        {
            return false;
        }
        var canBeCast = Enum.GetNames(type).Select(i => i.ToLower()).Contains(value.ToLower());
        if (!canBeCast)
        {
            return false;
        }
        return true;
    }

    public static string Cut(this string text, int length)
    {
        if (text.IsNothing())
        {
            return "";
        }
        if (text.Length < length)
        {
            return text;
        }
        string result = text.Substring(0, length);
        if (result.EndsWith(" "))
        {
            return $"{result}...";
        }
        return $"{result} ...";
    }
}
