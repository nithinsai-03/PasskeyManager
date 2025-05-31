using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PasswordManagerBackend.Models
{
    public class PasswordEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Use 'required' keyword for properties that must be non-null
        public required string Website { get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }

        // Use 'required' keyword for Salt as well
        [JsonConverter(typeof(ByteArrayConverter))]
        public required byte[] Salt { get; set; }
    }

    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? base64String = reader.GetString(); // Can be null
            if (string.IsNullOrEmpty(base64String))
            {
                return Array.Empty<byte>(); // Return an empty byte array instead of null
            }
            return Convert.FromBase64String(base64String);
        }

        public override void Write(Utf8JsonWriter writer, byte[]? value, JsonSerializerOptions options) // value can be null
        {
            if (value == null)
            {
                writer.WriteNullValue(); // Write null if the value is null
            }
            else
            {
                writer.WriteStringValue(Convert.ToBase64String(value));
            }
        }
    }
}