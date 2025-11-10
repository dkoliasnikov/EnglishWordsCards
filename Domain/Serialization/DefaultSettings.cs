using Newtonsoft.Json;

namespace Domain.Serialization;
internal static class DefaultSettings
{
    public static JsonSerializerSettings DefaultJsonSerializationSettings => new JsonSerializerSettings
    {
        Converters = new List<JsonConverter>
        {
            new LowerCaseValueConverter(),
            new LowercaseDictionaryConverter(),
        }
    };

    public static JsonSerializerSettings DefaultJsonDeserializationSettings => new JsonSerializerSettings
    {
        Converters = new List<JsonConverter>
        {
            new LowerCaseValueConverter(),

            new LowercaseDictionaryConverter(),
        }
    };
}
