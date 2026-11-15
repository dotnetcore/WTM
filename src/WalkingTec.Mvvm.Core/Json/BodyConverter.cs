using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.Json
{
    /// <summary>
    /// StringIgnoreLTGTConvert
    /// 忽略客户端提交的 &lt;及&gt;字符
    /// </summary>
    public class BodyConverter : JsonConverter<PostedBody>
    {
        /// <summary>
        /// 读取客户端提交的 JSON 请求体，遍历其中所有 Token，解析出全部属性路径并记录到返回对象的 ProNames 中。
        /// </summary>
        /// <param name="reader">JSON 读取器，指向待解析的请求体内容</param>
        /// <param name="typeToConvert">要转换的目标类型，此处为 PostedBody</param>
        /// <param name="options">序列化配置选项</param>
        /// <returns>包含请求体中全部属性路径的 PostedBody 对象</returns>
        public override PostedBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rv = new PostedBody();
            rv.ProNames = new List<string>();
            //记录每一层嵌套的属性名前缀，用于拼接完整的属性路径
            List<string> prefix = new List<string>();
            //当前对象的嵌套深度
            int depth = 0;
            //最近一次读取到的属性名
            string lastObjecName = "";
            //当前是否处于数组内部，大于 0 表示在数组中
            int insideArray = 0;
            //上一个 Token 的类型，用于识别数组中的对象
            JsonTokenType lastToken = JsonTokenType.Null;
            //逐个 Token 遍历整个 JSON 结构
            while (true)
            {
                //遇到数组开始：数组标记加一、深度加一，并以“属性名[0]”作为数组层的前缀
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    insideArray++;
                    depth++;
                    prefix.Add(lastObjecName + "[0]");
                }
                //遇到数组结束：数组标记减一、深度减一，并移除数组层的前缀
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    insideArray--;
                    depth--;
                    prefix.RemoveAt(prefix.Count - 1);
                }
                //遇到对象开始：不在数组内时深度加一并记录前缀，同时移除刚记录的属性名本身；
                //在数组内时，除紧随数组开始后的第一个对象外，其余对象直接跳过
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    if (insideArray == 0)
                    {
                        depth++;
                        prefix.Add(lastObjecName);
                        if (rv.ProNames.Count > 0)
                        {
                            rv.ProNames.RemoveAt(rv.ProNames.Count - 1);
                        }
                    }
                    else
                    {
                        if (lastToken != JsonTokenType.StartArray)
                        {
                            reader.TrySkip();
                            reader.Read();
                            continue;
                        }
                    }
                }
                //遇到属性名：结合父级前缀拼接出完整的属性路径，去重后记录到 ProNames 中
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var pname = reader.GetString();
                    lastObjecName = pname;
                    var p = prefix.Take(depth).ToSepratedString(seperator: ".");
                    if (string.IsNullOrEmpty(p) == false)
                    {
                        pname = p + "." + pname;
                    }
                    if (rv.ProNames.Contains(pname) == false)
                    {
                        rv.ProNames.Add(pname);
                    }
                }
                //遇到对象结束：深度减一并移除对应前缀；已读取到根对象结束时退出循环
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (insideArray == 0)
                    {
                        depth--;
                        prefix.RemoveAt(prefix.Count - 1);
                    }
                    if (reader.IsFinalBlock == true && reader.CurrentDepth == 0)
                    {
                        reader.Read();
                        break;
                    }
                }
                lastToken = reader.TokenType;
                reader.Read();
            }
            return rv;
        }


        /// <summary>
        /// 序列化时不输出任何内容（PostedBody 仅用于解析请求体，不需要序列化）。
        /// </summary>
        /// <param name="writer">JSON 写入器</param>
        /// <param name="value">要写入的 PostedBody 对象</param>
        /// <param name="options">序列化配置选项</param>
        public override void Write(Utf8JsonWriter writer, PostedBody value, JsonSerializerOptions options)
        {
            return;
        }

    }

    /// <summary>
    /// 表示客户端提交的 JSON 请求体，记录请求体中出现的所有属性路径。
    /// </summary>
    public class PostedBody
    {
        /// <summary>
        /// 请求体中出现的所有属性路径列表，嵌套属性使用“属性.子属性”的形式，数组元素以 [0] 占位。
        /// </summary>
        public List<string> ProNames { get; set; }
    }
}
