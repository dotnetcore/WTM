using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// 原始字符串转换器：读取时仅接受字符串 Token，写出时在内容两端添加 _raw_ 标记并使用宽松的编码器避免转义。
    /// </summary>
    public class RawStringConverter : JsonConverter<string>
    {
        /// <summary>
        /// 读取 JSON 并返回原始字符串：null 返回 null，字符串返回原文，读取异常或非字符串时同样返回 null。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>读取到的字符串，无法读取时返回 null</returns>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //null 直接返回 null
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            try
            {
                //字符串形式：直接返回原文
                if (reader.TokenType == JsonTokenType.String)
                {
                    string rv = reader.GetString();
                    return rv;
                }
            }
            //读取过程中出现异常时忽略，统一返回 null
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// 将字符串写出：null 时输出 JSON null，非空时在内容两端添加 _raw_ 标记后写出。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的字符串</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            //null 时输出 JSON null
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {                
                //在内容两端添加 _raw_ 标记，并使用宽松的编码器避免对中文等字符进行转义
                var txt = JsonEncodedText.Encode($"_raw_{value}_raw_", JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
                writer.WriteStringValue(txt);
            }
        }
    }

}

