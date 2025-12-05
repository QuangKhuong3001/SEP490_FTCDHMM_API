using System.Text.Json;
using System.Text.Json.Serialization;

public class SafeNullableGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var str = reader.GetString();

            if (string.IsNullOrWhiteSpace(str))
                return null;

            if (Guid.TryParse(str, out var result))
                return result;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}
