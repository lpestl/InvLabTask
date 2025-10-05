using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModelLayer;

// Custom converter for polymorphic property
public class PolymorphicConverter<T> : JsonConverter<T> where T : class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Deserialization via base type
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Serialization with a real object type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
