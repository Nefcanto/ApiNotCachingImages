namespace Infra;

public static class ObjectExtensions
{
    private static Random random = new Random();

    public static object Clone(this object @object)
    {
        var serialized = @object.Serialize();
        var copy = serialized.Deserialize<object>();
        return copy;
    }

    public static T Clone<T>(this T t)
    {
        var serialized = t.Serialize();
        var copy = serialized.Deserialize<T>();
        return copy;
    }

    public static T CastTo<T>(this object @object)
    {
        var serialized = @object.Serialize();
        var copy = serialized.Deserialize<T>();
        return copy;
    }

    public static T GetNewValues<T>(this T target, T source, params string[] exludedProperties)
    {
        var properties = target.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (exludedProperties.Contains(property.Name))
            {
                continue;
            }
            var targetValue = property.GetValue(target);
            var soureValue = property.GetValue(source);
            property.SetValue(target, soureValue);
        }
        return target;
    }

    public static bool Has(this object obj, string propertyName)
    {
        var dynamicObject = obj as DynamicObject;
        if (dynamicObject == null) return false;
        return dynamicObject.GetDynamicMemberNames().Contains(propertyName);
    }

    public static bool ToBoolean(this object @object)
    {
        if (@object == null)
        {
            return false;
        }
        bool result = false;
        Boolean.TryParse(@object.ToString(), out result);
        return result;
    }

    public static bool IsNumeric(this object @object)
    {
        if (@object == null)
        {
            return false;
        }
        decimal temp = 0;
        return decimal.TryParse(@object.ToString(), out temp);
    }

    public static int ToInt(this object @object)
    {
        int result = 0;
        int.TryParse(@object.ToString(), out result);
        return result;
    }

    public static long ToLong(this object @object)
    {
        long result = 0;
        long.TryParse(@object.ToString(), out result);
        return result;
    }

    public static decimal ToDecimal(this object @object)
    {
        decimal result = 0;
        decimal.TryParse(@object.ToString(), out result);
        return result;
    }
    public static ExpandoObject ToExpando(this object @object)
    {
        var properties = @object.GetType().GetProperties();
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var property in properties)
        {
            expando.Add(property.Name, property.GetValue(@object));
        }
        return (ExpandoObject)expando;
    }

    public static string Serialize(this object @object)
    {
        return JsonSerializer.Serialize(@object, JsonHelper.Options);
    }

    public static object Random(object[] objects)
    {
        var index = random.Next(objects.Length);
        var value = objects[index];
        return value;
    }
}
