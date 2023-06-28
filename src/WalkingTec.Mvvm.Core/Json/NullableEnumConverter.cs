using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.Json
{
    public class NullableEnumConverter : JsonConverterFactory
    {

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>) && typeToConvert.GetGenericArguments()[0].IsEnum;
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {

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

        private JsonSerializerOptions CloneOptions(JsonSerializerOptions op)
        {
            JsonSerializerOptions rv = new JsonSerializerOptions();
            rv.PropertyNamingPolicy = op.PropertyNamingPolicy;
            rv.AllowTrailingCommas = op.AllowTrailingCommas;
            rv.DefaultBufferSize = op.DefaultBufferSize;
            rv.DefaultIgnoreCondition = op.DefaultIgnoreCondition;
            rv.DictionaryKeyPolicy = op.DictionaryKeyPolicy;
            rv.Encoder = op.Encoder;
            rv.IgnoreNullValues = op.IgnoreNullValues;
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

        private class NullableEnumConverterInner<T> :
            JsonConverter<T>
        {
            protected readonly JsonSerializerOptions _options;
            public NullableEnumConverterInner(JsonSerializerOptions options)
            {
                _options = options;
            }

            public override T Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    if (string.IsNullOrEmpty(reader.GetString()) || string.IsNullOrWhiteSpace(reader.GetString()))
                    {
                        return default(T);
                    }
                }
                return JsonSerializer.Deserialize<T>(ref reader, _options);
            }

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
