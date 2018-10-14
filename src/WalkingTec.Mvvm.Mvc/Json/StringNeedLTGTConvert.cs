using Newtonsoft.Json;
using System;

namespace WalkingTec.Mvvm.Mvc.Json
{
    /// <summary>
    /// StringNeedLTGTConvert
    /// </summary>
    public class StringNeedLTGTConvert : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    return reader.Value as string;
                }
            }
            catch (Exception)
            {
                throw new Exception($"Error converting value {reader.Value} to type '{objectType}'");
            }
            throw new Exception($"Unexpected token {reader.TokenType} when parsing string");
        }
    }
}
