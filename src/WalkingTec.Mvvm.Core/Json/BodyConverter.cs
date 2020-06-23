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
            prefix.Add("");
            int depth = 0;
            string lastObjecName = "";
            int insideArray = 0;
            while(reader.TokenType != JsonTokenType.Null )
            {
                if(reader.TokenType == JsonTokenType.StartArray)
                {
                    insideArray++;
                }
                if(reader.TokenType == JsonTokenType.EndArray)
                {
                    insideArray--;
                }
                if(insideArray > 0)
                {
                    reader.Read();
                    continue;
                }
                if(reader.TokenType == JsonTokenType.StartObject)
                {
                    depth++;
                    if (prefix.Count < depth)
                    {
                        prefix.Add(lastObjecName);
                    }
                    else
                    {
                        prefix[depth - 1] = lastObjecName;
                    }
                    rv.ProNames.Remove(lastObjecName);
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var pname = reader.GetString();
                    lastObjecName = pname;
                    var p = prefix.Take(depth).ToSepratedString(seperator: ".");
                    if(string.IsNullOrEmpty(p) == false)
                    {
                        pname = p + "." + pname;
                    }
                    rv.ProNames.Add(pname);
                }
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    depth--;
                    if(reader.IsFinalBlock == true)
                    {
                        reader.Read();
                        break;
                    }
                }
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
