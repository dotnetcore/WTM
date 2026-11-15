namespace System.Text.Json.Serialization
{
    /// <summary>
    /// StringIgnoreLTGTConvert
    /// 忽略客户端提交的 &lt;及&gt;字符
    /// </summary>
    public class StringIgnoreLTGTConverter : JsonConverter<string>
    {
        /// <summary>
        /// 读取 JSON 字符串：null 返回 null，字符串返回去除小于号和大于号后的内容，其他类型返回 null。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>去除尖括号后的字符串，无法读取时返回 null</returns>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            //字符串形式：去除其中的小于号和大于号，防止脚本注入
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString().Replace("<", string.Empty).Replace(">", string.Empty);
            }

            return null;
        }

        /// <summary>
        /// 将字符串按原样写出：null 时输出 JSON null，否则输出普通字符串。
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
