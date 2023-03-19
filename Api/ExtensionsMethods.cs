namespace Api;

public static class ExtensionMethods
{
    public static bool IsFileUploadParameter(this System.Type parameterType)
    {
        return parameterType == typeof(IFormFile) || parameterType == typeof(IFormFileCollection) || typeof(IEnumerable<IFormFile>).IsAssignableFrom(parameterType);
    }

    public static T Extract<T>(this HttpContext httpContext) where T : class, new()
    {
        var instance = new T();
        var properties = instance.GetType().GetProperties().Where(i => i.Name != "RelatedItems").ToList();
        foreach (var property in properties)
        {
            if (httpContext.Request.ContentType.Contains("application/json"))
            {
                property.SetValue(instance, httpContext.ExtractProperty(property.Name));
            }
            else 
            {
                ExtractProperty(httpContext, instance, property);
            }
        }
        return instance;
    }

    public static object ExtractProperty(this HttpContext httpContext, string propertyName)
    {
        string json = httpContext.GetBody().Result;

        var element = json.Deserialize();

        var property = element.Get(propertyName);
        var kind = property?.ValueKind;

        switch (kind)
        {
            case JsonValueKind.String:
                return property?.GetString();
            case JsonValueKind.Number:
                return property?.GetInt64();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return property?.GetBoolean();
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                return property?.GetRawText();
            default:
                return null;
        }
    }

    private static void ExtractProperty(HttpContext httpContext, object instance, PropertyInfo property)
    {
        var values = httpContext.Request.Form[property.Name];
        if (values.Count > 0)
        {
            if (property.PropertyType.Name == typeof(long).Name || property.PropertyType.FullName == typeof(long?).FullName)
            {
                property.SetValue(instance, Convert.ToInt64(values[0]));
            }
            else if (property.PropertyType.Name == typeof(int).Name || property.PropertyType.FullName == typeof(int?).FullName)
            {
                property.SetValue(instance, Convert.ToInt32(values[0]));
            }
            else if (property.PropertyType.Name == typeof(Guid).Name || property.PropertyType.FullName == typeof(Guid?).FullName)
            {
                var guid = values[0];
                if (guid.IsNonEmptyGuid())
                {
                    property.SetValue(instance, Guid.Parse(values[0]));
                }
                else
                {
                    Logger.LogWarning($"Property {property.PropertyType.Name} of model {instance.GetType().FullName} requires a Guid value, but in HTTP request we received {guid}.");
                }
            }
            else if (property.PropertyType.Name == typeof(string).Name)
            {
                property.SetValue(instance, values[0]);
            }
        }
    }

    public static async Task<string> GetBody(this HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();
        httpContext.Request.Body.Position = 0;
        var rawRequestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
        return rawRequestBody;
    }
}
