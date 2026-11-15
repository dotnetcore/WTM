using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// 布尔类型转换器：读取时兼容字符串形式的 true/false 和原生布尔值，其他内容一律按 false 处理；写出时输出原生布尔值。
    /// </summary>
    public class BoolStringConverter :
JsonConverter<bool>
    {

        /// <summary>
        /// 读取 JSON 并转换为布尔值：字符串按内容解析，原生布尔值直接读取，其他类型一律返回 false。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>解析得到的布尔值，无法识别时返回 false</returns>
        public override bool Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {

            JsonTokenType token = reader.TokenType;

            //字符串形式：按内容判断真假，无法识别时按 false 处理
            if (token == JsonTokenType.String)
            {
                var s = reader.GetString() ?? "";
                if (s.ToLower() == "true")
                {
                    return true;
                }
                else if (s.ToLower() == "false")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            //原生布尔 Token：直接读取布尔值
            else if (token == JsonTokenType.True || token == JsonTokenType.False)
            {
                return reader.GetBoolean();
            }
            //其他类型（数字、对象等）：统一返回 false
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将布尔值以原生 JSON 布尔值形式写出。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="data">要写入的布尔值</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(
            Utf8JsonWriter writer,
            bool data,
            JsonSerializerOptions options)
        {
                writer.WriteBooleanValue(data);
            
        }

    }

}

