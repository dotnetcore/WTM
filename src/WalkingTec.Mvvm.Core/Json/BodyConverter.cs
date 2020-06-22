using System;
using System.Collections.Generic;
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
            while(reader.TokenType != JsonTokenType.Null)
            {
                if(reader.TokenType == JsonTokenType.StartObject)
                {
                    depth++;
                }
                if(reader.TokenType == JsonTokenType.PropertyName)
                {
                    var pname = reader.GetString();
                    if(prefix.Count < depth)
                    {
                        prefix.Add(pname);
                    }
                    else
                    {
                        prefix[depth] = pname;
                    }
                    var p = prefix.ToSepratedString(seperator: ".");
                    if(string.IsNullOrEmpty(p) == false)
                    {
                        pname = p + "." + pname;
                    }
                    rv.ProNames.Add(pname);
                }

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
