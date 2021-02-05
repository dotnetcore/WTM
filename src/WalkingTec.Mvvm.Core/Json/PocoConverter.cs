using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Json
{
    public class PocoConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(TopBasePoco).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {

            var temp = CloneOptions(options);
            foreach (var item in options.Converters)
            {
                if(item.GetType() != typeof(PocoConverter))
                {
                    temp.Converters.Add(item);
                }
            }
            temp.ReferenceHandler = ReferenceHandler.Preserve;
            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(PocoConverterInner<>).MakeGenericType(
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

        private class PocoConverterInner<T> :
            JsonConverter<T> where T : TopBasePoco
        {
            protected readonly JsonSerializerOptions _options;
            public PocoConverterInner(JsonSerializerOptions options)
            {
                _options = options;
            }

            public override T Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {

                return JsonSerializer.Deserialize<T>(ref reader, _options);
            }

            public override void Write(
                Utf8JsonWriter writer,
                T data,
                JsonSerializerOptions options)
            {
                var str = JsonSerializer.Serialize(data, _options);
                var txt = JsonEncodedText.Encode(str, JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
                writer.WriteStringValue(txt);
            }
        }
    }
}
