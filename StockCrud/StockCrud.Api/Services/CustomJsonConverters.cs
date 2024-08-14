using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace StockCrud.Api.Services;


public static class CustomJsonConverters
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string _format = "yyyy-MM-dd";
        
        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(_format));
        }

        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return DateOnly.ParseExact((string)reader.Value, _format);
        }
    }
}