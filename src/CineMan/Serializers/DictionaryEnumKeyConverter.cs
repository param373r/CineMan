using System.Text.Json;
using System.Text.Json.Serialization;

namespace CineMan.Serializers;

public class DictionaryEnumKeyConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : Enum
{
    public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dictionary = new Dictionary<TKey, TValue>();
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        foreach (var element in jsonDocument.RootElement.EnumerateObject())
        {
            var key = (TKey)Enum.Parse(typeof(TKey), element.Name, true);
            var value = JsonSerializer.Deserialize<TValue>(element.Value.GetRawText(), options);

            if (value == null) throw new JsonException();

            dictionary.Add(key, value);
        }
        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key.ToString());
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }
        writer.WriteEndObject();
    }
}