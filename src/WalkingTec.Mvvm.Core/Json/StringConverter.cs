namespace  System.Text.Json.Serialization
{
    /// <summary>
    /// 通用字符串转换器：读取时仅接受 JSON 字符串和 null，写出时按普通字符串输出。
    /// </summary>
    public class JsonStringConverter : JsonConverter<string>
    {
        /// <summary>
        /// 读取 JSON 字符串：null 返回 null，字符串 Token 返回原文，其他类型一律返回 null。
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

            //字符串形式：直接返回原文
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            //其他类型：返回 null
            return null;
        }

        /// <summary>
        /// 将字符串写出：null 时输出 JSON null，否则输出普通字符串。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的字符串</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }

    }
}
