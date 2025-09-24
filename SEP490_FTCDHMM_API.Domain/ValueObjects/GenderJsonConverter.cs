using System.Text.Json;
using System.Text.Json.Serialization;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class GenderJsonConverter : JsonConverter<Gender>
    {
        public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            return value switch
            {
                "Male" => Gender.Male,
                "Female" => Gender.Female,
                "Other" => Gender.Other,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }

        public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
