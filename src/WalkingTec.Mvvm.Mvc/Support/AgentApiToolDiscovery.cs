using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Agent;

namespace WalkingTec.Mvvm.Mvc.Support
{
    /// <summary>
    /// 反射发现的接口工具：控制器上的标准动作（查询/新增/修改/删除）或[AgentTool]标记的动作，
    /// 携带参数Schema，供Agent对话模式注册为大模型可调用的工具
    /// </summary>
    public class AgentApiTool
    {
        /// <summary>工具分类：query / add / edit / delete / call</summary>
        public string Kind { get; set; }
        /// <summary>工具定义（名称/描述/参数Schema）</summary>
        public LlmToolDefinition Definition { get; set; }
        /// <summary>完整相对URL（已解析[area]/[controller]/[action]占位符，可能含{id}等路由参数）</summary>
        public string Url { get; set; }
        /// <summary>HTTP方法</summary>
        public string HttpMethod { get; set; } = "POST";
        /// <summary>URL中的路由参数名（如Delete/{id}中的id）</summary>
        public List<string> RouteParams { get; set; } = new List<string>();
        /// <summary>参数形态：searcher=Searcher查询条件；vm={Entity:实体字段}；ids=ID数组；plain=简单参数对象</summary>
        public string ParamKind { get; set; }
        /// <summary>动作参数类型（仅用于诊断）</summary>
        public Type ParamType { get; set; }
    }

    /// <summary>
    /// 接口工具自动发现器：扫描已加载的程序集，把BaseApiController派生控制器上的
    /// 标准动作注册为工具——查询(Search)始终注册；新增(Add)/修改(Edit)/删除(BatchDelete)
    /// 由ToolScope配置控制（Query=仅查询，All=全部）；[AgentTool(true)]可把任意自定义接口
    /// 纳入工具，[AgentTool(false)]可排除任意接口（方法级优先于控制器级）。
    /// 执行时以当前用户身份HTTP自调用对应接口，天然继承接口的权限校验与数据权限过滤
    /// </summary>
    public static class AgentApiToolDiscovery
    {
        private static readonly Dictionary<string, List<AgentApiTool>> _cache = new Dictionary<string, List<AgentApiTool>>();
        private static readonly object _lock = new object();

        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        private static readonly JsonSerializerOptions Relaxed = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static readonly Regex RouteParamRegex = new Regex(@"\{(\w+)(:[^}]*)?\}", RegexOptions.Compiled);

        /// <summary>
        /// 发现可注册为工具的接口（按范围缓存，进程内每个范围只扫描一次）
        /// </summary>
        /// <param name="allowWrite">true=包含新增/修改/删除工具（ToolScope=All）</param>
        /// <returns>工具列表</returns>
        public static List<AgentApiTool> Discover(bool allowWrite)
        {
            var scopeKey = allowWrite ? "all" : "query";
            lock (_lock)
            {
                if (_cache.TryGetValue(scopeKey, out var cached))
                    return cached;
                var tools = new List<AgentApiTool>();
                var entryAsm = Assembly.GetEntryAssembly();
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Distinct())
                {
                    //只扫描框架与宿主应用程序集，避免遍历系统程序集
                    var name = asm.GetName().Name ?? "";
                    if (!name.StartsWith("WalkingTec", StringComparison.OrdinalIgnoreCase) && asm != entryAsm)
                        continue;
                    if (asm.IsDynamic)
                        continue;
                    Type[] types;
                    try { types = asm.GetTypes(); }
                    catch { continue; }

                    foreach (var type in types)
                    {
                        try
                        {
                            tools.AddRange(BuildFromController(type, allowWrite));
                        }
                        catch { /* 单个控制器解析失败不影响整体 */ }
                    }
                }
                //按名称排序保证工具列表顺序稳定：与系统提示词一起构成固定前缀，有利于命中LLM的前缀缓存
                tools = tools.OrderBy(t => t.Definition.Name).ToList();
                _cache[scopeKey] = tools;
                return tools;
            }
        }

        /// <summary>
        /// 把一个控制器类型转换为工具列表；非BaseApiController派生时返回空
        /// </summary>
        private static List<AgentApiTool> BuildFromController(Type type, bool allowWrite)
        {
            var tools = new List<AgentApiTool>();
            if (!type.IsClass || type.IsAbstract || !typeof(ControllerBase).IsAssignableFrom(type))
                return tools;
            if (!typeof(BaseApiController).IsAssignableFrom(type) && !type.IsDefined(typeof(ApiControllerAttribute), false))
                return tools;
            var baseRoute = ResolveControllerRoute(type);
            if (string.IsNullOrEmpty(baseRoute))
                return tools;

            var ctrlName = type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                ? type.Name[..^"Controller".Length]
                : type.Name;
            var ctrlDesc = type.GetCustomAttribute<ActionDescriptionAttribute>(false)?.Description;
            if (string.IsNullOrEmpty(ctrlDesc) || ctrlDesc.StartsWith("MenuKey.", StringComparison.OrdinalIgnoreCase))
                ctrlDesc = ctrlName;
            var ctrlToolAttr = type.GetCustomAttribute<AgentToolAttribute>(false);

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var (httpMethod, template) = GetVerb(method);
                if (httpMethod == null)
                    continue;
                //方法级[AgentTool]优先于控制器级
                var attr = method.GetCustomAttribute<AgentToolAttribute>(false) ?? ctrlToolAttr;
                if (attr?.Enable == false)
                    continue;

                var path = template.Replace("[action]", method.Name, StringComparison.OrdinalIgnoreCase);
                AgentApiTool tool = null;
                try { tool = TryBuildAction(method, httpMethod, path, baseRoute, ctrlName, ctrlDesc, attr, allowWrite); }
                catch { /* 单个动作解析失败不影响同控制器其他动作 */ }
                if (tool != null)
                    tools.Add(tool);
            }
            return tools;
        }

        /// <summary>
        /// 按动作分类构造工具：查询（Search）恒注册；增删改受范围控制；[AgentTool(true)]任意动作注册
        /// </summary>
        private static AgentApiTool TryBuildAction(MethodInfo method, string httpMethod, string path, string baseRoute, string ctrlName, string ctrlDesc, AgentToolAttribute attr, bool allowWrite)
        {
            var isSearch = string.Equals(method.Name, "Search", StringComparison.OrdinalIgnoreCase)
                || string.Equals(path, "search", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith("/search", StringComparison.OrdinalIgnoreCase);
            var isAdd = httpMethod == "POST" && IsAction(method.Name, path, "Add");
            var isEdit = (httpMethod == "PUT" || httpMethod == "POST") && IsAction(method.Name, path, "Edit");
            var isDelete = httpMethod == "POST" && IsAction(method.Name, path, "BatchDelete");

            string kind;
            if (isSearch)
                kind = "query";
            else if (isAdd || isEdit || isDelete)
            {
                if (!allowWrite)
                    return null;
                kind = isAdd ? "add" : isEdit ? "edit" : "delete";
            }
            else if (attr?.Enable == true)
                kind = "call";
            else
                return null;

            var url = $"{baseRoute}/{path}".TrimEnd('/');
            var routeParams = RouteParamRegex.Matches(url).Select(m => m.Groups[1].Value).ToList();

            //确定参数形态与Schema
            string paramKind;
            string schema;
            if (kind == "query")
            {
                var searcher = method.GetParameters()
                    .FirstOrDefault(p => p.ParameterType.IsClass && !p.ParameterType.IsAbstract && typeof(BaseSearcher).IsAssignableFrom(p.ParameterType));
                if (searcher == null)
                    return null;
                paramKind = "searcher";
                schema = BuildSearcherSchema(searcher.ParameterType);
            }
            else if (kind == "add" || kind == "edit")
            {
                var vm = method.GetParameters()
                    .FirstOrDefault(p => p.ParameterType.IsClass && typeof(BaseVM).IsAssignableFrom(p.ParameterType));
                if (vm == null)
                    return null;
                paramKind = "vm";
                schema = BuildEntitySchema(vm.ParameterType, kind == "edit");
            }
            else if (kind == "delete")
            {
                if (method.GetParameters().All(p => p.ParameterType != typeof(string[])))
                    return null;
                paramKind = "ids";
                schema = "{\"type\":\"object\",\"properties\":{\"Ids\":{\"type\":\"array\",\"items\":{\"type\":\"string\"},\"description\":\"要删除的记录ID列表\"}},\"required\":[\"Ids\"]}";
            }
            else
            {
                paramKind = "plain";
                schema = BuildPlainSchema(method, routeParams);
            }

            var name = kind switch
            {
                "query" => "query_" + ctrlName,
                "add" => "add_" + ctrlName,
                "edit" => "edit_" + ctrlName,
                "delete" => "delete_" + ctrlName,
                _ => "call_" + ctrlName + "_" + method.Name,
            };
            var actionDesc = method.GetCustomAttribute<ActionDescriptionAttribute>(false)?.Description;
            if (!string.IsNullOrEmpty(actionDesc) && actionDesc.StartsWith("Sys.", StringComparison.OrdinalIgnoreCase))
                actionDesc = null;
            var description = attr?.Description;
            if (string.IsNullOrEmpty(description))
            {
                description = kind switch
                {
                    "query" => $"查询「{ctrlDesc}」的业务数据列表，支持条件过滤、分页与排序；返回JSON的Data数组与Count总数",
                    "add" => $"新增一条「{ctrlDesc}」记录，参数为实体字段；执行前应先向用户确认信息",
                    "edit" => $"按ID修改「{ctrlDesc}」记录的其他字段；ID是定位记录的主键、不可修改（要更换主键只能删除旧记录后新增）；执行前应先向用户确认",
                    "delete" => $"按ID列表删除「{ctrlDesc}」记录（不可恢复）；执行前必须向用户确认",
                    _ => (string.IsNullOrEmpty(actionDesc) ? $"{ctrlDesc} - {method.Name}" : $"{ctrlDesc} - {actionDesc}"),
                };
            }

            return new AgentApiTool
            {
                Kind = kind,
                Url = url,
                HttpMethod = httpMethod,
                RouteParams = routeParams,
                ParamKind = paramKind,
                ParamType = method.GetParameters().FirstOrDefault()?.ParameterType,
                Definition = new LlmToolDefinition { Name = name, Description = description, ParametersJsonSchema = schema },
            };
        }

        private static bool IsAction(string methodName, string path, string standard)
        {
            return string.Equals(methodName, standard, StringComparison.OrdinalIgnoreCase)
                || string.Equals(path, standard, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 读取动作的HTTP动词属性，返回(方法名, 路由模板)
        /// </summary>
        private static (string, string) GetVerb(MethodInfo method)
        {
            var post = method.GetCustomAttribute<HttpPostAttribute>(false);
            if (post != null) return ("POST", post.Template ?? method.Name);
            var put = method.GetCustomAttribute<HttpPutAttribute>(false);
            if (put != null) return ("PUT", put.Template ?? method.Name);
            var delete = method.GetCustomAttribute<HttpDeleteAttribute>(false);
            if (delete != null) return ("DELETE", delete.Template ?? method.Name);
            var get = method.GetCustomAttribute<HttpGetAttribute>(false);
            if (get != null) return ("GET", get.Template ?? method.Name);
            return (null, null);
        }

        /// <summary>
        /// 解析控制器级[Route]模板为基址URL，替换[controller]与[area]占位符
        /// </summary>
        private static string ResolveControllerRoute(Type type)
        {
            var route = type.GetCustomAttribute<RouteAttribute>(false);
            if (route == null || string.IsNullOrEmpty(route.Template))
                return null;
            var template = route.Template.TrimStart('/');
            var ctrlName = type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                ? type.Name[..^"Controller".Length]
                : type.Name;
            template = template.Replace("[controller]", ctrlName, StringComparison.OrdinalIgnoreCase);
            //AreaAttribute的分区名通过构造参数读取（不同版本属性名不一致）
            var areaData = type.GetCustomAttributesData()
                .FirstOrDefault(d => d.AttributeType.Name == "AreaAttribute");
            var area = areaData?.ConstructorArguments.FirstOrDefault().Value as string;
            if (!string.IsNullOrEmpty(area))
                template = template.Replace("[area]", area, StringComparison.OrdinalIgnoreCase);
            return template;
        }

        /// <summary>
        /// 由Searcher类型生成JSON Schema：每个简单类型查询字段一个参数（描述取[Display(Name)]中文标签），
        /// 附加Page/Limit分页与SortInfo排序（字段名+方向）；跳过列表/复杂类型字段
        /// </summary>
        private static string BuildSearcherSchema(Type searcherType)
        {
            var props = new Dictionary<string, object>();
            foreach (var prop in searcherType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var mapped = MapSimpleType(prop);
                if (mapped != null)
                    props[prop.Name] = mapped;
            }
            props["Page"] = new { type = "integer", description = "页码，从1开始，默认1" };
            props["Limit"] = new { type = "integer", description = "每页条数，默认5，最大20" };
            props["SortInfo"] = new
            {
                type = "object",
                description = "排序设置",
                @properties = new Dictionary<string, object>
                {
                    ["Property"] = new { type = "string", description = "排序字段名（用返回数据中的字段名，如CreateTime、Name）" },
                    ["Direction"] = new { type = "string", description = "排序方向", @enum = Enum.GetNames(typeof(SortDir)) },
                }
            };
            var schema = new
            {
                type = "object",
                properties = props,
                required = Array.Empty<string>()
            };
            return JsonSerializer.Serialize(schema, Relaxed);
        }

        /// <summary>
        /// 由CRUD VM类型生成Schema：取VM的Entity属性对应模型的简单字段，
        /// 描述含中文标签，必填字段同时进入Entity.required列表，
        /// 整体包装为 {Entity:{...}}（WTM标准提交结构）。
        /// 修改接口中主键字段标注为定位用、不可修改（框架按ID定位记录，改主键须删除后新增）
        /// </summary>
        private static string BuildEntitySchema(Type vmType, bool isEdit = false)
        {
            var entityProp = vmType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);
            if (entityProp == null)
                return null;
            var props = new Dictionary<string, object>();
            var requiredFields = new List<string>();
            foreach (var prop in entityProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var mapped = MapSimpleTypeDict(DescOf(prop), prop.PropertyType);
                if (mapped == null || props.Count >= 20)
                    continue;
                var isKey = prop.IsDefined(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false) || prop.Name == "ID";
                if (isEdit && isKey)
                {
                    mapped["description"] = DescOf(prop) + "（定位记录的主键，必须传原值，不可修改；更换主键只能删除旧记录后新增）";
                    requiredFields.Add(prop.Name);
                }
                else if (prop.IsDefined(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false))
                {
                    mapped["description"] = DescOf(prop) + "（必填）";
                    requiredFields.Add(prop.Name);
                }
                props[prop.Name] = mapped;
            }
            var entity = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["description"] = "实体字段",
                ["properties"] = props,
            };
            if (requiredFields.Count > 0)
                entity["required"] = requiredFields;
            var schema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object> { ["Entity"] = entity },
                ["required"] = new List<string> { "Entity" },
            };
            return JsonSerializer.Serialize(schema, Relaxed);
        }

        /// <summary>
        /// 自定义接口的Schema：路由参数（必填string）+ 简单类型的方法参数 + 可能的VM Entity包装
        /// </summary>
        private static string BuildPlainSchema(MethodInfo method, List<string> routeParams)
        {
            var props = new Dictionary<string, object>();
            var required = new List<string>();
            foreach (var rp in routeParams)
            {
                props[Capitalize(rp)] = new { type = "string", description = "路径参数" };
                required.Add(Capitalize(rp));
            }
            foreach (var p in method.GetParameters())
            {
                if (routeParams.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                    continue;
                if (p.ParameterType == typeof(string[]))
                {
                    props[Capitalize(p.Name)] = new { type = "array", items = new { type = "string" }, description = p.Name };
                    continue;
                }
                if (p.ParameterType.IsClass && typeof(BaseVM).IsAssignableFrom(p.ParameterType))
                {
                    var entityProp = p.ParameterType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);
                    if (entityProp != null)
                    {
                        var fields = new Dictionary<string, object>();
                        foreach (var prop in entityProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                        {
                            var mapped = MapSimpleType(prop);
                            if (mapped != null && fields.Count < 20)
                                fields[prop.Name] = mapped;
                        }
                        props["Entity"] = new { type = "object", description = "实体字段", properties = fields };
                        required.Add("Entity");
                    }
                    continue;
                }
                var mappedParam = MapSimpleType(p);
                if (mappedParam != null)
                    props[Capitalize(p.Name)] = mappedParam;
            }
            var schema = new
            {
                type = "object",
                properties = props,
                required = required
            };
            return JsonSerializer.Serialize(schema, Relaxed);
        }

        /// <summary>
        /// 把简单类型映射为Schema片段（string/数值/布尔/日期/枚举/Guid），复杂类型返回null
        /// </summary>
        private static object MapSimpleType(string desc, Type propertyType)
        {
            var pt = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (pt == typeof(string))
                return new { type = "string", description = desc };
            if (pt == typeof(int) || pt == typeof(long) || pt == typeof(short))
                return new { type = "integer", description = desc };
            if (pt == typeof(decimal) || pt == typeof(double) || pt == typeof(float))
                return new { type = "number", description = desc };
            if (pt == typeof(bool))
                return new { type = "boolean", description = desc };
            if (pt == typeof(DateTime) || pt == typeof(DateOnly))
                return new { type = "string", format = "date", description = desc };
            if (pt == typeof(Guid))
                return new { type = "string", description = desc + "（GUID格式）" };
            if (pt.IsEnum)
                return new { type = "string", description = desc, @enum = Enum.GetNames(pt) };
            return null;
        }

        private static object MapSimpleType(PropertyInfo prop)
            => MapSimpleType(DescOf(prop), prop.PropertyType);

        private static object MapSimpleType(ParameterInfo p)
            => MapSimpleType(p.Name, p.ParameterType);

        /// <summary>
        /// 同MapSimpleType但返回字典形式，便于调用方追加/修改键（枚举键名为enum）
        /// </summary>
        private static Dictionary<string, object> MapSimpleTypeDict(string desc, Type propertyType)
        {
            var pt = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (pt == typeof(string))
                return new Dictionary<string, object> { ["type"] = "string", ["description"] = desc };
            if (pt == typeof(int) || pt == typeof(long) || pt == typeof(short))
                return new Dictionary<string, object> { ["type"] = "integer", ["description"] = desc };
            if (pt == typeof(decimal) || pt == typeof(double) || pt == typeof(float))
                return new Dictionary<string, object> { ["type"] = "number", ["description"] = desc };
            if (pt == typeof(bool))
                return new Dictionary<string, object> { ["type"] = "boolean", ["description"] = desc };
            if (pt == typeof(DateTime) || pt == typeof(DateOnly))
                return new Dictionary<string, object> { ["type"] = "string", ["format"] = "date", ["description"] = desc };
            if (pt == typeof(Guid))
                return new Dictionary<string, object> { ["type"] = "string", ["description"] = desc + "（GUID格式）" };
            if (pt.IsEnum)
                return new Dictionary<string, object> { ["type"] = "string", ["description"] = desc, ["enum"] = Enum.GetNames(pt) };
            return null;
        }

        private static string DescOf(MemberInfo member)
        {
            var display = member.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>(false);
            return string.IsNullOrEmpty(display?.Name) ? member.Name : display.Name;
        }

        private static string Capitalize(string s)
            => string.IsNullOrEmpty(s) ? s : char.ToUpperInvariant(s[0]) + s[1..];

        /// <summary>
        /// 按工具名查找已发现的接口工具（供Excel工具等服务端取数场景复用）
        /// </summary>
        /// <param name="name">工具名，如query_Student</param>
        /// <param name="allowWrite">是否包含增删改工具的范围</param>
        /// <returns>接口工具，未找到返回null</returns>
        public static AgentApiTool FindTool(string name, bool allowWrite)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return Discover(allowWrite).FirstOrDefault(t => string.Equals(t.Definition.Name, name, StringComparison.Ordinal));
        }

        /// <summary>
        /// 以当前用户身份执行工具：按参数形态组装请求（查询条件JSON / 实体JSON / ID数组 / 路由参数+查询串）
        /// 自调用本服务对应接口并转发登录令牌以继承权限校验与数据权限
        /// </summary>
        /// <param name="tool">接口工具</param>
        /// <param name="args">模型给出的工具参数</param>
        /// <param name="baseUrl">本服务基址，如http://localhost:8598</param>
        /// <param name="authorization">当前请求的Authorization头（Bearer令牌）</param>
        /// <returns>接口返回的JSON对象；失败时返回带error说明的对象</returns>
        public static async Task<object> InvokeAsync(AgentApiTool tool, JsonElement args, string baseUrl, string authorization)
        {
            var body = new Dictionary<string, object>();
            if (args.ValueKind == JsonValueKind.Object)
            {
                foreach (var p in args.EnumerateObject())
                    body[p.Name] = p.Value.Clone();
            }

            //替换URL中的路由参数（如Delete/{id}），从参数中取值后从请求体剔除
            var url = tool.Url;
            foreach (var rp in tool.RouteParams)
            {
                var key = body.Keys.FirstOrDefault(k => string.Equals(k, rp, StringComparison.OrdinalIgnoreCase));
                var value = key != null ? body[key]?.ToString()?.Trim('"') : null;
                if (string.IsNullOrEmpty(value))
                    return new { error = $"缺少路径参数 {rp}" };
                url = Regex.Replace(url, @"\{" + rp + @"(:[^}]*)?\}", Uri.EscapeDataString(value));
                body.Remove(key);
            }

            HttpContent content = null;
            var finalUrl = baseUrl.TrimEnd('/') + "/" + url;
            switch (tool.ParamKind)
            {
                case "searcher":
                    //分页默认值：第一页、最多5条，防止把大结果集塞给模型
                    if (!body.ContainsKey("Limit")) body["Limit"] = 5;
                    if (!body.ContainsKey("Page")) body["Page"] = 1;
                    content = new StringContent(JsonSerializer.Serialize(body, Relaxed), Encoding.UTF8, "application/json");
                    break;
                case "vm":
                case "plain" when tool.HttpMethod == "POST" || tool.HttpMethod == "PUT":
                    content = new StringContent(JsonSerializer.Serialize(body, Relaxed), Encoding.UTF8, "application/json");
                    break;
                case "ids":
                    //BatchDelete(string[] ids)按裸数组绑定
                    var ids = body.TryGetValue("Ids", out var idsVal) && idsVal is JsonElement je && je.ValueKind == JsonValueKind.Array
                        ? je
                        : (JsonElement?)null;
                    content = new StringContent(ids != null ? JsonSerializer.Serialize(ids.Value, Relaxed) : "[]", Encoding.UTF8, "application/json");
                    break;
                default:
                    //GET等无请求体：剩余参数进查询串
                    if (body.Count > 0)
                    {
                        var qs = string.Join("&", body.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value.ToString())}"));
                        finalUrl += "?" + qs;
                    }
                    break;
            }

            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(20));
            using var req = new HttpRequestMessage(new HttpMethod(tool.HttpMethod), finalUrl);
            if (content != null)
                req.Content = content;
            if (!string.IsNullOrEmpty(authorization))
                req.Headers.TryAddWithoutValidation("Authorization", authorization);
            using var resp = await Http.SendAsync(req, cts.Token);
            var text = await resp.Content.ReadAsStringAsync(cts.Token);
            if (!resp.IsSuccessStatusCode)
                return new { error = $"接口返回{(int)resp.StatusCode}", detail = text.Length > 500 ? text[..500] : text };
            try
            {
                return JsonSerializer.Deserialize<JsonElement>(text);
            }
            catch
            {
                return new { error = "接口返回内容不是有效JSON", detail = text.Length > 500 ? text[..500] : text };
            }
        }
    }
}
