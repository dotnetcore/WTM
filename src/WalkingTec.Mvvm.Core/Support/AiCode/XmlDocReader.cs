using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WalkingTec.Mvvm.Core.Support.AiCode
{
    /// <summary>
    /// 单个成员的XML文档注释
    /// </summary>
    public class XmlMemberDoc
    {
        /// <summary>summary节点内容</summary>
        public string Summary { get; set; }
        /// <summary>参数名→param节点内容</summary>
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        /// <summary>returns节点内容</summary>
        public string Returns { get; set; }
    }

    /// <summary>
    /// XML文档注释读取器：从程序集同目录的同名XML文件中按DocID反查
    /// 方法/类型/属性的 summary/param/returns 注释。
    /// 需要项目开启 GenerateDocumentationFile；文件缺失或成员无注释时返回null（调用方降级到特性/推断）。
    /// 按程序集缓存解析结果，解析失败缓存空表
    /// </summary>
    public static class XmlDocReader
    {
        private static readonly ConcurrentDictionary<Assembly, Lazy<Dictionary<string, XmlMemberDoc>>> Cache
            = new ConcurrentDictionary<Assembly, Lazy<Dictionary<string, XmlMemberDoc>>>();

        /// <summary>
        /// 读取方法的XML注释（summary/param/returns）
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns>注释对象，无则返回null</returns>
        public static XmlMemberDoc GetMethodDoc(MethodInfo method)
        {
            return Find(method?.DeclaringType?.Assembly, "M:" + MethodDocId(method));
        }

        /// <summary>
        /// 读取类型的XML注释（summary）
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>注释文本，无则返回null</returns>
        public static string GetTypeSummary(Type type)
        {
            return Find(type?.Assembly, "T:" + TypeDocId(type))?.Summary;
        }

        /// <summary>
        /// 读取属性的XML注释（summary）
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>注释文本，无则返回null</returns>
        public static string GetPropertySummary(PropertyInfo property)
        {
            return Find(property?.DeclaringType?.Assembly, "P:" + TypeDocId(property.DeclaringType) + "." + property.Name)?.Summary;
        }

        private static XmlMemberDoc Find(Assembly asm, string docId)
        {
            if (asm == null || string.IsNullOrEmpty(docId))
                return null;
            var dict = Cache.GetOrAdd(asm, a => new Lazy<Dictionary<string, XmlMemberDoc>>(() => Load(a)));
            return dict.Value.TryGetValue(docId, out var doc) ? doc : null;
        }

        /// <summary>
        /// 加载并解析程序集同名XML文档文件
        /// </summary>
        private static Dictionary<string, XmlMemberDoc> Load(Assembly asm)
        {
            var dict = new Dictionary<string, XmlMemberDoc>();
            try
            {
                if (string.IsNullOrEmpty(asm.Location))
                    return dict;
                var xmlPath = Path.ChangeExtension(asm.Location, ".xml");
                if (!File.Exists(xmlPath))
                    return dict;
                var root = XDocument.Load(xmlPath).Root;
                var membersRoot = root?.Element("members");
                if (membersRoot == null)
                    return dict;
                foreach (var member in membersRoot.Elements("member"))
                {
                    var name = member.Attribute("name")?.Value;
                    if (string.IsNullOrEmpty(name))
                        continue;
                    var doc = new XmlMemberDoc
                    {
                        Summary = CleanText(member.Element("summary")?.Value),
                        Returns = CleanText(member.Element("returns")?.Value)
                    };
                    foreach (var p in member.Elements("param"))
                    {
                        var pname = p.Attribute("name")?.Value;
                        if (!string.IsNullOrEmpty(pname))
                            doc.Params[pname] = CleanText(p.Value);
                    }
                    if (doc.Summary != null || doc.Returns != null || doc.Params.Count > 0)
                        dict[name] = doc;
                }
            }
            catch
            {
                //XML损坏时按无注释处理
            }
            return dict;
        }

        /// <summary>
        /// 压缩XML注释文本中的空白
        /// </summary>
        private static string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            return Regex.Replace(text.Trim(), @"\s+", " ");
        }

        #region DocID生成

        /// <summary>
        /// 生成方法的DocID（如 M:Ns.Type.Method(System.Double)），
        /// 覆盖占位函数场景的类型范围：基元/业务类/泛型/可空/数组/元组/ByRef
        /// </summary>
        public static string MethodDocId(MethodInfo method)
        {
            var namePart = method.Name;
            if (method.IsGenericMethodDefinition)
                namePart += "`" + method.GetGenericArguments().Length;
            var paras = method.GetParameters().Select(p => TypeDocId(p.ParameterType));
            return TypeDocId(method.DeclaringType) + "." + namePart + "(" + string.Join(",", paras) + ")";
        }

        /// <summary>
        /// 生成类型的DocID（泛型实例为大括号参数形式，如 System.Collections.Generic.List{System.Int32}）
        /// </summary>
        public static string TypeDocId(Type t)
        {
            if (t == null)
                return "";
            if (t.IsGenericParameter)
                return "`" + t.GenericParameterPosition;
            if (t.IsByRef)
                return TypeDocId(t.GetElementType()) + "@";
            if (t.IsArray)
                return TypeDocId(t.GetElementType()) + "[" + new string(',', t.GetArrayRank() - 1) + "]";
            if (t.IsGenericType)
            {
                var def = t.GetGenericTypeDefinition();
                var defName = def.IsNested
                    ? TypeDocId(def.DeclaringType) + "." + def.Name
                    : (def.Namespace == null ? def.Name : def.Namespace + "." + def.Name);
                return defName + "{" + string.Join(",", t.GetGenericArguments().Select(TypeDocId)) + "}";
            }
            if (t.IsNested)
                return TypeDocId(t.DeclaringType) + "." + t.Name;
            return t.Namespace == null ? t.Name : t.Namespace + "." + t.Name;
        }

        #endregion
    }

    /// <summary>
    /// 描述推断器：当XML注释与特性都缺失时，按成员名称与类型生成兜底中文描述，
    /// 供AI提示词使用（弱信号，调用方应标注"按名称推断"）
    /// </summary>
    public static class DescriptionGuesser
    {
        /// <summary>
        /// 按名称与类型推断中文描述
        /// </summary>
        /// <param name="name">成员名（属性名/参数名/元组元素名）</param>
        /// <param name="type">成员类型，可为null</param>
        /// <returns>推断的中文描述</returns>
        public static string Guess(string name, Type type = null)
        {
            var words = SplitWords(name);
            var joined = string.Join(" ", words);
            var lower = string.Join("", words).ToLowerInvariant();

            var underlying = type == null ? null : (Nullable.GetUnderlyingType(type) ?? type);
            bool isBool = underlying == typeof(bool);
            bool isTime = underlying == typeof(DateTime) || underlying == typeof(DateTimeOffset);

            //布尔：是否xxx
            if (isBool)
            {
                var lead = new[] { "is", "has", "can", "need", "should", "enable", "allow" }
                    .FirstOrDefault(w => lower.StartsWith(w) && lower.Length > w.Length);
                if (lead != null)
                    return "是否" + JoinCn(words.Skip(1));
                return "是否" + JoinCn(words);
            }
            //时间：xxx时间
            if (isTime && (lower.EndsWith("time") || lower.EndsWith("date") || lower.EndsWith("at")))
                return JoinCn(words.SkipLast(1)) + "时间";

            //常用词映射
            if (lower.StartsWith("min")) return "最小" + JoinCn(words.Skip(1));
            if (lower.StartsWith("max")) return "最大" + JoinCn(words.Skip(1));
            if (lower == "id" || lower.EndsWith("id")) return JoinCn(words.SkipLast(1)) + "编号";
            if (lower.EndsWith("count") || lower.EndsWith("num") || lower.EndsWith("qty")) return JoinCn(words.SkipLast(1)) + "数量";
            if (lower == "name" || lower.EndsWith("name")) return JoinCn(words.SkipLast(1)) + "名称";
            if (lower.EndsWith("type")) return JoinCn(words.SkipLast(1)) + "类型";
            if (lower.EndsWith("status") || lower.EndsWith("state")) return JoinCn(words.SkipLast(1)) + "状态";
            if (lower.EndsWith("desc") || lower.EndsWith("description") || lower.EndsWith("remark") || lower.EndsWith("comment"))
                return JoinCn(words.SkipLast(1)) + "说明";
            if (lower.EndsWith("msg") || lower.EndsWith("message")) return JoinCn(words.SkipLast(1)) + "消息";
            if (lower.EndsWith("value") || lower.EndsWith("val")) return JoinCn(words.SkipLast(1)) + "值";
            if (lower.EndsWith("time")) return JoinCn(words.SkipLast(1)) + "时间";
            if (lower.EndsWith("date")) return JoinCn(words.SkipLast(1)) + "日期";
            if (lower.EndsWith("flag")) return JoinCn(words.SkipLast(1)) + "标志";

            //无规则命中：返回拆词后的原词（LLM可直接理解英文语义）
            return joined;
        }

        /// <summary>
        /// 驼峰/下划线拆词：inputValue → [input, Value]
        /// </summary>
        private static List<string> SplitWords(string name)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(name))
                return list;
            var parts = name.Split('_', '-');
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;
                var matches = Regex.Matches(part, @"([A-Z]+(?![a-z])|[A-Z][a-z0-9]*|[a-z0-9]+)");
                foreach (Match m in matches)
                    list.Add(m.Value);
            }
            return list;
        }

        /// <summary>
        /// 把英文词直接拼接（词典式翻译交给LLM，这里不做逐词汉化）
        /// </summary>
        private static string JoinCn(IEnumerable<string> words)
        {
            var joined = string.Join(" ", words).Trim();
            return string.IsNullOrEmpty(joined) ? "" : joined;
        }
    }
}
