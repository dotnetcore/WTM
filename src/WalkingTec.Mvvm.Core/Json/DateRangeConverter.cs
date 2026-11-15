using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// 日期范围转换器：读取时支持从包含开始时间和结束时间的字符串数组解析 DateRange，写出时输出为同样的字符串数组。
    /// </summary>
    public class DateRangeConverter : JsonConverter<DateRange>
    {
        /// <summary>
        /// 读取 JSON 并转换为 DateRange：仅支持数组形式（第一个元素为开始时间，第二个元素为结束时间），解析失败或格式不符时返回 null。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>解析得到的 DateRange 对象，无法解析时返回 null</returns>
        public override DateRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //null 值直接返回 null
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            try
            {
                //数组形式：依次读取开始时间和结束时间两个字符串，尝试解析为 DateRange
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    string[] ds = new string[2];
                    ds[0] = reader.GetString();
                    reader.Read();
                    ds[1] = reader.GetString();
                    reader.Read();
                    if (DateRange.TryParse(ds, out var dateRange))
                    {
                        return dateRange;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            //解析过程中出现异常时忽略，统一返回 null
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// 将 DateRange 写出为包含开始时间和结束时间两个字符串的 JSON 数组，值为 null 时输出 JSON null。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的日期范围对象</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, DateRange value, JsonSerializerOptions options)
        {
            //值为 null 时输出 JSON null
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                //writer.WriteStringValue(JsonSerializer.Serialize(value),);
                //写出为 [开始时间, 结束时间] 形式的字符串数组
                writer.WriteStartArray();
                writer.WriteStringValue(value.GetStartTime().ToString());
                writer.WriteStringValue(value.GetEndTime().ToString());
                writer.WriteEndArray();
            }
        }
    }
}
