using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// Type 类型转换器：读写时均不处理实际数据，始终返回或输出 null。
    /// </summary>
    public class TypeConverter : JsonConverter<Type>
    {
        /// <summary>
        /// 读取时不做任何解析，直接返回 null（Type 类型不参与反序列化）。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>始终返回 null</returns>
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        /// <summary>
        /// 写出时始终输出 JSON null（Type 类型不参与序列化）。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的类型对象</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
                writer.WriteNullValue();
        }
    }

}

