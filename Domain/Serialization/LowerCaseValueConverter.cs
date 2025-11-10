using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Serialization;

public class LowerCaseValueConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        ProcessJToken(token);
        
        return objectType != null ? token.ToObject(objectType) : null;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        JToken token = JToken.FromObject(value);

        ProcessJToken(token);

        token.WriteTo(writer);
    }

    private static void ProcessJToken(JToken token)
    {
        if (token is JValue jValue && jValue.Value is string stringValue)
        {
            jValue.Value = stringValue.ToLowerInvariant();
        }
        else if (token is JContainer container)
        {
            foreach (JToken childToken in container.Children())
            {
                ProcessJToken(childToken);
            }
        }
    }
}
