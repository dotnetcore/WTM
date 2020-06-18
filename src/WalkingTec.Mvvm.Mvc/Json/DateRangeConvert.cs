using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Json
{
    public class DateRangeConvert : JsonConverter<DateRange>
    {
        public override void WriteJson(JsonWriter writer, DateRange value, JsonSerializer serializer)
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

        public override DateRange ReadJson(JsonReader reader, Type objectType, DateRange existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            try
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    string[] ds = new string[2];
                    ds[0] = reader.ReadAsString();
                    ds[1] = reader.ReadAsString();
                    reader.Read();
                    if(DateRange.TryParse(ds, out var dateRange))
                    {
                        return dateRange;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
