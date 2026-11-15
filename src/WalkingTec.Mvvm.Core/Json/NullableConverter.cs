using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{

    /// <summary>
    /// 可空值类型转换器：读取时把空字符串或空白字符串视为 null，其余情况按底层类型正常反序列化；写出时按底层类型序列化。
    /// </summary>
    public class NullableConverter<T> : JsonConverter<Nullable<T>> where T : struct
    {
        /// <summary>
        /// 读取 JSON 并转换为可空值类型：字符串为空或空白时返回 null，其他情况按底层类型反序列化。
        /// </summary>
        /// <param name="reader">JSON 读取器</param>
        /// <param name="typeToConvert">要转换的目标类型</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>反序列化得到的可空值，空字符串时返回 null</returns>
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //字符串形式：空字符串或空白字符串视为 null
            if (reader.TokenType == JsonTokenType.String)
            {
                if (string.IsNullOrEmpty(reader.GetString()) || string.IsNullOrWhiteSpace(reader.GetString()))
                {
                    return null;
                }
                else
                {
                    //非空字符串：按底层类型 T 反序列化
                    return JsonSerializer.Deserialize<T>(ref reader, options);
                }
            }
            //其他 Token（数字、布尔等）：直接按底层类型 T 反序列化
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }


        /// <summary>
        /// 将可空值按底层类型 T 序列化写出。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的可空值</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value!.Value, options);
        }
    }

}
