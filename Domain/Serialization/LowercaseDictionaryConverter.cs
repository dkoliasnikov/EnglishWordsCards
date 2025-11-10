using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Serialization;

public class LowercaseDictionaryConverter : JsonConverter<Dictionary<string, int>>
{
    public override Dictionary<string, int> ReadJson(JsonReader reader, Type objectType, [AllowNull] Dictionary<string, int>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dictionary = new Dictionary<string, int>();
        JObject jo = JObject.Load(reader);

        foreach (var property in jo.Properties())
        {
            string lowerCaseKey = property.Name.ToLower();
            var value = property.Value.ToObject<int>(serializer);
            dictionary[lowerCaseKey] = value; 
        }

        return dictionary;
    }

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, Dictionary<string, int>? value, JsonSerializer serializer) => throw new NotImplementedException("Writing is not supported by this converter.");
}

