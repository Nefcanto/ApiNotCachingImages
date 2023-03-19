namespace Infra;

public static class JsonHelper
{
    private static object lockToken = new Object();

    public static void ConfigureJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        jsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        jsonSerializerOptions.UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.WriteIndented = true;
    }

    private static JsonSerializerOptions options;

    public static JsonSerializerOptions Options
    {
        get
        {
            if (options == null)
            {
                lock (lockToken)
                {
                    options = new JsonSerializerOptions();
                    ConfigureJsonSerializerOptions(options);
                    options.Converters.Add(new JsonStringEnumConverter());
                }
            }
            return options;
        }
    }

    public static JsonElement? Get(this JsonElement element, string name) =>
        element.ValueKind != JsonValueKind.Null && element.ValueKind != JsonValueKind.Undefined && element.TryGetProperty(name, out var value)
            ? value : (JsonElement?)null;

    public static JsonElement? Get(this JsonElement element, int index)
    {
        if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            return null;
        var value = element.EnumerateArray().ElementAtOrDefault(index);
        return value.ValueKind != JsonValueKind.Undefined ? value : (JsonElement?)null;
    }
}
