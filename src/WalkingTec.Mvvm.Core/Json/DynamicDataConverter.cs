using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// 动态数据转换器：读取时把 JSON 对象中的各字段（含嵌套对象、数组和基础类型值）收集到 DynamicData 的 Fields 字典，写出时按字段逐一输出。
    /// </summary>
    public class DynamicDataConverter : JsonConverter<DynamicData>
    {
        /// <summary>
        /// 读取 JSON 并转换为 DynamicData：遍历所有 Token，把属性名作为键、属性值作为值存入 Fields 字典，解析失败时返回 null。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>反序列化得到的 DynamicData 对象，解析失败时返回 null</returns>
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
                    //嵌套对象：递归反序列化为 DynamicData 后作为当前字段的值
                    if(reader.TokenType == JsonTokenType.StartObject) {
                        if(level > 0)
                        {
                            var inner = JsonSerializer.Deserialize<DynamicData>(ref reader, options);
                            rv.Fields.Add(currentkey, inner);
                        }
                        level++;
                    }
                    //对象结束：层级减一
                    if(reader.TokenType == JsonTokenType.EndObject)
                    {
                        level--;
                    }
                    //数组：逐个元素反序列化，仅含单个空键字段的视为简单值，否则视为嵌套对象，最终以列表形式存入当前字段
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
                    //属性名：记录当前正在处理的字段名
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        currentkey = reader.GetString();
                    }
                    //基础类型值（字符串、数字、布尔、null）：按 object 反序列化后存入当前字段
                    if(reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Number || reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.Null)
                    {
                        var val = JsonSerializer.Deserialize<object>(ref reader,options);
                        rv.Fields.Add(currentkey, val);
                    }
                    //已读取到数据末尾且回到顶层：结束解析
                    if (reader.IsFinalBlock && level == 0)
                    {
                        //reader.Read();
                        break;
                    }
                    reader.Read();
                }
                return rv;
            }
            //解析过程中出现任何异常时返回 null
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将 DynamicData 写出为 JSON 对象：值为 null 或没有字段时输出 JSON null，否则逐个写出 Fields 中的键值。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的动态数据对象</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, DynamicData value, JsonSerializerOptions options)
        {
            //值为 null 或字段字典为 null 时输出 JSON null
            if (value == null || value.Fields == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                foreach (var item in value.Fields)
                {
                    //字段值为 null 时，仅在不忽略 null 值的配置下显式输出 null
                    if(item.Value == null)
                    {
                        if (options.DefaultIgnoreCondition == JsonIgnoreCondition.Never)
                        {
                            writer.WriteNull(item.Key);
                        }
                    }
                    //正常字段：写出属性名后按其运行时类型序列化
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
