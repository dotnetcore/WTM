using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// 可空枚举转换器工厂：为所有可空枚举类型（Nullable 包装的枚举）创建专用的内层转换器。
    /// </summary>
    public class NullableEnumConverter : JsonConverterFactory
    {

        /// <summary>
        /// 判断指定类型是否为可空枚举类型，即 Nullable 泛型且类型参数为枚举。
        /// </summary>
        /// <param name="typeToConvert">待判断的类型</param>
        /// <returns>是可空枚举类型返回 true，否则返回 false</returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>) && typeToConvert.GetGenericArguments()[0].IsEnum;
        }

        /// <summary>
        /// 为指定的可空枚举类型创建内层转换器实例，创建时使用复制并剔除工厂类转换器的序列化选项以避免递归。
        /// </summary>
        /// <param name="type">可空枚举类型</param>
        /// <param name="options">当前的序列化配置选项</param>
        /// <returns>与指定类型匹配的转换器实例</returns>
        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {

            //复制序列化选项，并剔除工厂类转换器，防止内层转换器再次触发本工厂造成递归
            var temp = CloneOptions(options);
            foreach (var item in options.Converters)
            {
                if(item.GetType() != typeof(PocoConverter) && item.GetType() != typeof(NullableEnumConverter))
                {
                    temp.Converters.Add(item);
                }
            }
            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(NullableEnumConverterInner<>).MakeGenericType(
                    new Type[] { type }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { temp },
                culture: null);

            return converter;
        }

        /// <summary>
        /// 复制一份序列化选项，供内层转换器独立使用。
        /// </summary>
        /// <param name="op">原始序列化选项</param>
        /// <returns>复制得到的新序列化选项</returns>
        private JsonSerializerOptions CloneOptions(JsonSerializerOptions op)
        {
            JsonSerializerOptions rv = new JsonSerializerOptions();
            rv.PropertyNamingPolicy = op.PropertyNamingPolicy;
            rv.AllowTrailingCommas = op.AllowTrailingCommas;
            rv.DefaultBufferSize = op.DefaultBufferSize;
            rv.DefaultIgnoreCondition = op.DefaultIgnoreCondition;
            rv.DictionaryKeyPolicy = op.DictionaryKeyPolicy;
            rv.Encoder = op.Encoder;
            rv.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            rv.IgnoreReadOnlyFields = op.IgnoreReadOnlyFields;
            rv.IgnoreReadOnlyProperties = op.IgnoreReadOnlyProperties;
            rv.IncludeFields = op.IncludeFields;
            rv.DefaultIgnoreCondition = op.DefaultIgnoreCondition;
            rv.DictionaryKeyPolicy = op.DictionaryKeyPolicy;
            rv.MaxDepth = op.MaxDepth;
            rv.NumberHandling = op.NumberHandling;
            rv.ReadCommentHandling = op.ReadCommentHandling;
            rv.ReferenceHandler = op.ReferenceHandler;
            rv.WriteIndented = op.WriteIndented;
            return rv;
        }

        /// <summary>
        /// 可空枚举的内层转换器：读取时空字符串返回默认值（即 null），其他情况按枚举类型正常反序列化。
        /// </summary>
        private class NullableEnumConverterInner<T> :
            JsonConverter<T>
        {
            /// <summary>
            /// 复制后的序列化选项，已剔除工厂类转换器
            /// </summary>
            protected readonly JsonSerializerOptions _options;
            /// <summary>
            /// 使用指定的序列化选项初始化内层转换器。
            /// </summary>
            /// <param name="options">复制后的序列化选项</param>
            public NullableEnumConverterInner(JsonSerializerOptions options)
            {
                _options = options;
            }

            /// <summary>
            /// 读取 JSON 并转换为枚举值：字符串为空或空白时返回默认值（可空枚举即 null），其他情况按枚举类型反序列化。
            /// </summary>
            /// <param name="reader">JSON 读取器</param>
            /// <param name="typeToConvert">要转换的目标类型</param>
            /// <param name="options">序列化配置选项</param>
            /// <returns>反序列化得到的枚举值，空字符串时返回默认值</returns>
            public override T Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                //字符串形式：空字符串或空白字符串返回默认值（可空枚举即 null）
                if (reader.TokenType == JsonTokenType.String)
                {
                    if (string.IsNullOrEmpty(reader.GetString()) || string.IsNullOrWhiteSpace(reader.GetString()))
                    {
                        return default(T);
                    }
                }
                //其他情况按 T 反序列化（使用剔除工厂转换器后的选项）
                return JsonSerializer.Deserialize<T>(ref reader, _options);
            }

            /// <summary>
            /// 将枚举值按其类型序列化写出。
            /// </summary>
            /// <param name="writer">JSON 写入器</param>
            /// <param name="data">要写入的枚举值</param>
            /// <param name="options">序列化配置选项</param>
            public override void Write(
                Utf8JsonWriter writer,
                T data,
                JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, data, typeof(T), _options);
            }
        }

    }
}
