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
            rv.IgnoreReadOnlyFields = op.IgnoreReadOnlyFields;
            rv.IgnoreReadOnlyProperties = op.IgnoreReadOnlyProperties;
            rv.IncludeFields = op.IncludeFields;
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
                var _datacache = new Dictionary<string, int>();
                RemoveCycleReference(data, _datacache);
                JsonSerializer.Serialize(writer, data, typeof(T), _options);
            }

            private void RemoveCycleReference(object Entity, Dictionary<string, int> datacache)
            {
                var pros = Entity.GetType().GetAllProperties();
                var mainkey = Entity.GetType().FullName + (Entity as TopBasePoco).GetID();
                datacache.TryAdd(mainkey, 1);

                foreach (var pro in pros)
                {
                    if (typeof(TopBasePoco).IsAssignableFrom(pro.PropertyType))
                    {
                        var subentity = pro.GetValue(Entity) as TopBasePoco;
                        string key = pro.PropertyType.FullName + subentity?.GetID() ?? "";
                        if (subentity != null && datacache.ContainsKey(key) == false)
                        {
                            RemoveCycleReference(subentity, datacache);
                        }
                        else
                        {
                            pro.SetValue(Entity,null);
                        }
                    }
                    //找到类型为List<xxx>的字段
                    if (pro.PropertyType.GenericTypeArguments.Count() > 0)
                    {
                        //获取xxx的类型
                        var ftype = pro.PropertyType.GenericTypeArguments.First();
                        //如果xxx继承自TopBasePoco
                        if (ftype.IsSubclassOf(typeof(TopBasePoco)))
                        {
                            //界面传过来的子表数据

                            if (pro.GetValue(Entity) is IEnumerable<TopBasePoco> list && list.Count() > 0)
                            {
                                bool found = false;
                                foreach (var newitem in list)
                                {
                                    if (newitem != null)
                                    {
                                        string subkey = ftype.FullName + newitem?.GetID() ?? "";
                                        if (datacache.ContainsKey(subkey) == false)
                                        {
                                            RemoveCycleReference(newitem, datacache.ToDictionary(x=>x.Key,x=>x.Value));
                                            found = true;
                                        }
                                        else
                                        {
                                            found = false;
                                            break;
                                        }
                                    }
                                }
                                if(found == false)
                                {
                                    pro.SetValue(Entity, null);
                                }
                            }
                        }
                    }
                }

            }
        }

    }
}
