using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{

    /// <summary>
    /// 日期时间转换器：读取时优先按字符串解析，写出时统一格式化为 yyyy-MM-dd HH:mm:ss 格式的字符串。
    /// </summary>
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 读取 JSON 并转换为 DateTime：字符串形式优先尝试常规格式解析，解析失败或非字符串时按 ISO 8601 规则读取。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>反序列化得到的 DateTime 值</returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //字符串形式：尝试按常规日期时间格式解析
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                    return date;
            }
            //非字符串或解析失败时，交给读取器按 ISO 8601 格式读取
            return reader.GetDateTime();
        }

        /// <summary>
        /// 将 DateTime 以 yyyy-MM-dd HH:mm:ss 格式的字符串写出。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的日期时间值</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
