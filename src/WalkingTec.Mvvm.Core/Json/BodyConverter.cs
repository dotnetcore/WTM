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
        public override PostedBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rv = new PostedBody();
            rv.ProNames = new List<string>();
            List<string> prefix = new List<string>();
            int depth = 0;
            string lastObjecName = "";
            int insideArray = 0;
            JsonTokenType lastToken = JsonTokenType.Null;
            while (true)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    insideArray++;
                    depth++;
                    prefix.Add(lastObjecName + "[0]");
                }
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    insideArray--;
                    depth--;
                    prefix.RemoveAt(prefix.Count - 1);
                }
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


        public override void Write(Utf8JsonWriter writer, PostedBody value, JsonSerializerOptions options)
        {
            return;
        }

    }

    public class PostedBody
    {
        public List<string> ProNames { get; set; }
    }
}
