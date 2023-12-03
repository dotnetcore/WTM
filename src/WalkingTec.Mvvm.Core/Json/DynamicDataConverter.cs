using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.Json
{
    public class DynamicDataConverter : JsonConverter<DynamicData>
    {
        public override DynamicData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                DynamicData rv = new DynamicData();
                rv.Fields = new Dictionary<string, object>();
                string currentkey = "";
                //object currentvalue;
                int level = 0;
                while (true)
                {
                    if(reader.TokenType == JsonTokenType.StartObject) {
                        if(level > 0)
                        {
                            var inner = JsonSerializer.Deserialize<DynamicData>(ref reader, options);
                            rv.Fields.Add(currentkey, inner);
                        }
                        level++;
                    }
                    if(reader.TokenType == JsonTokenType.EndObject)
                    {
                        level--;
                    }
                    if(reader.TokenType == JsonTokenType.StartArray)
                    {
                        List<object> list = new List<object>();
                        reader.Read();
                        while(reader.TokenType!= JsonTokenType.EndArray)
                        {
                            var inner = JsonSerializer.Deserialize<DynamicData>(ref reader, options);
                            if(inner.Fields.Count == 1 && inner.Fields.First().Key == "")
                            {
                                list.Add(inner.Fields.First().Value);
                            }
                            else
                            {
                                list.Add(inner);
                            }
                            reader.Read();
                        }
                        rv.Fields.Add(currentkey, list);
                    }
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        currentkey = reader.GetString();
                    }
                    if(reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Number || reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.Null)
                    {
                        var val = JsonSerializer.Deserialize<object>(ref reader,options);
                        rv.Fields.Add(currentkey, val);
                    }
                    if (reader.IsFinalBlock && level == 0)
                    {
                        //reader.Read();
                        break;
                    }
                    reader.Read();
                }
                return rv;
            }
            catch
            {
                return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, DynamicData value, JsonSerializerOptions options)
        {
            if (value == null || value.Fields == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                foreach (var item in value.Fields)
                {
                    if(item.Value == null)
                    {
                        if (options.DefaultIgnoreCondition == JsonIgnoreCondition.Never)
                        {
                            writer.WriteNull(item.Key);
                        }
                    }
                    else
                    {
                        writer.WritePropertyName(item.Key);
                        JsonSerializer.Serialize(writer, item.Value, options);
                    }
                }
                writer.WriteEndObject();
            }
        }

    }
}
