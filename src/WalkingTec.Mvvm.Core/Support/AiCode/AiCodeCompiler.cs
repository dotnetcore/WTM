using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WalkingTec.Mvvm.Core.Support.AiCode
{
    /// <summary>
    /// AI代码动态编译结果：成功时携带可调用的MethodInfo与所属的可卸载上下文，
    /// 失败时携带编译诊断或黑名单命中信息
    /// </summary>
    public class AiCompileResult
    {
        /// <summary>是否成功（编译通过且未命中黑名单）</summary>
        public bool Success { get; set; }
        /// <summary>编译诊断/错误信息</summary>
        public List<string> Diagnostics { get; set; } = new List<string>();
        /// <summary>命中的黑名单项（类型或方法名）</summary>
        public string BlacklistHit { get; set; }
        /// <summary>编译产物中的方法（成功时有值）</summary>
        public MethodInfo Method { get; set; }
        /// <summary>方法所在的加载上下文（用于缓存生命周期管理）</summary>
        internal AssemblyLoadContext Context { get; set; }
    }

    /// <summary>
    /// AI代码编译器：把AI生成的方法代码包装编译为内存程序集，
    /// 反射取出 MethodInfo 供运行器调用。
    /// 编译结果按（代码+函数名+模型类型）的内容哈希缓存，
    /// 上下文使用可卸载 AssemblyLoadContext，缓存容量超限时逐出并卸载旧程序集。
    /// 编译前先做语法树黑名单扫描，拦截文件/网络/进程/反射等危险API
    /// </summary>
    public static class AiCodeCompiler
    {
        /// <summary>
        /// 缓存容量上限，超过后按插入顺序逐出最旧条目并卸载
        /// </summary>
        private const int CacheCapacity = 128;

        private static readonly ConcurrentDictionary<string, AiCompileResult> Cache = new ConcurrentDictionary<string, AiCompileResult>();
        private static readonly object EvictLock = new object();
        private static readonly List<string> CacheOrder = new List<string>();

        /// <summary>
        /// 危险类型黑名单：按成员访问表达式的标识符匹配（如 File.Delete 中的 File）。
        /// 反射相关类型（Type/MethodInfo等）一并拦截，防止通过 Type.GetType("字符串")
        /// 绕过类型名扫描后在运行时还原任意API（曾被发现的安全漏洞：GetType+GetMethods+Invoke 组合可执行任意代码）
        /// </summary>
        private static readonly HashSet<string> BlacklistTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "File", "Directory", "DriveInfo", "FileSystemInfo", "FileSystemWatcher",
            "Process", "ProcessStartInfo",
            "HttpClient", "WebClient", "HttpWebRequest", "HttpWebResponse", "Dns", "Socket", "TcpClient", "TcpListener", "UdpClient", "WebRequest", "WebUtility",
            "Assembly", "Activator", "Marshal", "GCHandle",
            "Registry", "RegistryKey",
            "Environment", "AppDomain",
            "Thread", "ThreadPool", "TaskFactory",
            "Mutex", "Semaphore", "SemaphoreSlim",
            //反射族：拦截反射句柄类型，配合方法黑名单封死运行时类型还原
            "Type", "MethodInfo", "MethodBase", "PropertyInfo", "FieldInfo", "ConstructorInfo", "MemberInfo", "ParameterInfo",
            "DynamicMethod", "ILGenerator", "Expression", "LambdaExpression",
            "Unsafe", "MemoryMarshal", "IntPtr",
        };

        /// <summary>
        /// 危险方法名黑名单：任意对象上调用这些名字的方法即拦截（GetType().GetMethod、Assembly.Load 等）。
        /// GetType/GetMethods/Invoke/Compile/CreateDelegate 等补入：单复数形式全部覆盖，
        /// 防止 Type.GetType("System.IO.File, System.Runtime")+GetMethods()+Invoke() 这类字符串反射绕过
        /// </summary>
        private static readonly HashSet<string> BlacklistMethods = new HashSet<string>(StringComparer.Ordinal)
        {
            "GetMethod", "GetField", "GetProperty", "InvokeMember", "CreateInstance",
            "LoadFrom", "LoadFile", "LoadModule", "LoadByteArray", "Load",
            "Exit", "Kill", "Abort", "Suspend", "Resume",
            "GetEnvironmentVariable", "SetEnvironmentVariable",
            "Freeze", "AllocHGlobal", "PtrToStructure", "StructureToPtr",
            //反射族（含复数形式与执行入口）
            "GetType", "GetMethods", "GetProperties", "GetFields", "GetConstructor", "GetConstructors",
            "GetInterface", "GetCustomAttributes", "GetTypeFromHandle", "GetTypeFromProgID", "GetTypeFromCLSID",
            "Invoke", "CreateDelegate", "MakeGenericMethod", "MakeGenericType", "GetUninitializedObject",
            //表达式树编译（可构造任意调用后Compile执行）
            "Compile", "CompileToAssembly",
        };

        /// <summary>
        /// 编译AI代码（带缓存）。code应为形如
        /// "public static bool ShouldSend(CommuData item, int minInterval) { ... }"
        /// 的静态方法定义，方法名必须等于funcName
        /// </summary>
        /// <param name="code">AI生成的方法代码</param>
        /// <param name="funcName">函数名（同时是方法名）</param>
        /// <param name="modelType">关联的模型类型（其所在程序集会被引用，供脚本使用其属性）</param>
        /// <returns>编译结果</returns>
        public static AiCompileResult Compile(string code, string funcName, Type modelType)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new AiCompileResult { Success = false, Diagnostics = new List<string> { "代码为空" } };

            var key = BuildCacheKey(code, funcName, modelType);
            if (Cache.TryGetValue(key, out var cached))
                return cached;

            var result = CompileCore(code, funcName, modelType);
            Store(key, result);
            return result;
        }

        /// <summary>
        /// 清空编译缓存并卸载全部动态程序集
        /// </summary>
        public static void ClearCache()
        {
            lock (EvictLock)
            {
                foreach (var key in CacheOrder)
                {
                    if (Cache.TryRemove(key, out var removed))
                        TryUnload(removed);
                }
                CacheOrder.Clear();
            }
        }

        /// <summary>
        /// 内容寻址缓存键：代码+函数名+模型类型全名的SHA256
        /// </summary>
        private static string BuildCacheKey(string code, string funcName, Type modelType)
        {
            var raw = $"{funcName}|{modelType?.FullName}|{code}";
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes);
        }

        private static void Store(string key, AiCompileResult result)
        {
            lock (EvictLock)
            {
                Cache[key] = result;
                CacheOrder.Add(key);
                while (CacheOrder.Count > CacheCapacity)
                {
                    var oldest = CacheOrder[0];
                    CacheOrder.RemoveAt(0);
                    if (Cache.TryRemove(oldest, out var evicted))
                        TryUnload(evicted);
                }
            }
        }

        private static void TryUnload(AiCompileResult result)
        {
            try { result.Context?.Unload(); } catch { }
            result.Context = null;
            result.Method = null;
        }

        private static AiCompileResult CompileCore(string code, string funcName, Type modelType)
        {
            //1. 黑名单扫描（确定性拦截，先于编译）
            var hit = ScanBlacklist(code);
            if (hit != null)
            {
                return new AiCompileResult
                {
                    Success = false,
                    BlacklistHit = hit,
                    Diagnostics = new List<string> { $"代码中使用了被禁止的API：{hit}（涉及文件/网络/进程/反射/环境等系统资源，不允许在AI代码中使用）" }
                };
            }

            //2. 包装为静态类成员编译（脚本方法名必须等于funcName）
            var source = $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
{(modelType != null && string.IsNullOrEmpty(modelType.Namespace) == false ? $"using {modelType.Namespace};" : "")}
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.AiCode;

namespace __AiGenerated
{{
    public static class __AiScript
    {{
{code}
    }}
}}
";
            var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));

            //临时诊断：编译失败时转储包装源码到临时目录，便于定位解析问题（定位后移除）
            try
            {
                if (System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "aicode_debug")) == false)
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "aicode_debug"));
                System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "aicode_debug", $"last_compile_{DateTime.Now:HHmmss_fff}.cs"), source);
            }
            catch { }

            //3. 编译到内存流
            var compilation = CSharpCompilation.Create(
                $"__aiscript_{Guid.NewGuid():N}",
                new[] { tree },
                BuildReferences(modelType),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Debug)
                    .WithOverflowChecks(true));

            using var ms = new MemoryStream();
            var emit = compilation.Emit(ms);
            if (!emit.Success)
            {
                var diags = emit.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString())
                    .ToList();
                if (diags.Count == 0)
                    diags.Add("编译失败但没有返回具体错误");
                return new AiCompileResult { Success = false, Diagnostics = diags };
            }

            //4. 加载到可卸载上下文并反射取方法
            ms.Position = 0;
            var alc = new AssemblyLoadContext($"aicode_{Guid.NewGuid():N}", isCollectible: true);
            try
            {
                var asm = alc.LoadFromStream(ms);
                var type = asm.GetType("__AiGenerated.__AiScript");
                var method = type?.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    alc.Unload();
                    return new AiCompileResult
                    {
                        Success = false,
                        Diagnostics = new List<string> { $"代码中找不到名为 {funcName} 的 public static 方法，方法名必须与函数名一致" }
                    };
                }
                return new AiCompileResult { Success = true, Method = method, Context = alc };
            }
            catch
            {
                try { alc.Unload(); } catch { }
                throw;
            }
        }

        /// <summary>
        /// 构建编译引用：从当前已加载程序集中挑选系统库与业务库（模型所在程序集、Core程序集）
        /// </summary>
        private static List<MetadataReference> BuildReferences(Type modelType)
        {
            var wanted = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "System.Runtime", "System.Private.CoreLib", "System.Linq", "System.Collections",
                "System.Console", "System.Text", "System.Text.RegularExpressions", "System.Private.Uri",
                "netstandard", "System.ObjectModel", "System.Memory", "System.Threading.Tasks",
                typeof(AiCodeCompiler).Assembly.GetName().Name,
                modelType?.Assembly.GetName().Name,
            };
            var refs = new List<MetadataReference>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.IsDynamic || string.IsNullOrEmpty(asm.Location))
                    continue;
                if (wanted.Contains(asm.GetName().Name))
                    refs.Add(MetadataReference.CreateFromFile(asm.Location));
            }
            //兜底：object所在库必须有
            if (refs.Count == 0)
                refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            return refs;
        }

        /// <summary>
        /// 语法树黑名单扫描：检查代码中所有标识符（类型名）与被调用的方法名，
        /// 覆盖全限定名写法（System.IO.File.Delete 也命中 File），
        /// 并拦截P/Invoke（DllImport）、unsafe上下文、指针类型与stackalloc等非托管入口。
        /// 返回命中的黑名单项描述，未命中返回null。
        /// 这是确定性拦截层，与AI审查互补，不保证覆盖所有危险代码
        /// </summary>
        internal static string ScanBlacklist(string code)
        {
            try
            {
                var tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest));
                var root = tree.GetRoot();
                //0. P/Invoke与非托管入口：DllImport/UnmanagedFunctionPointer属性、指针类型、stackalloc、unsafe/fixed块
                foreach (var attr in root.DescendantNodes().OfType<AttributeSyntax>())
                {
                    var attrName = attr.Name.ToString();
                    if (attrName == "DllImport" || attrName == "DllImportAttribute" || attrName.EndsWith(".DllImport")
                        || attrName == "UnmanagedFunctionPointer" || attrName.EndsWith(".UnmanagedFunctionPointer"))
                        return "DllImport";
                }
                if (root.DescendantNodes().OfType<PointerTypeSyntax>().Any())
                    return "指针类型";
                if (root.DescendantNodes().OfType<StackAllocArrayCreationExpressionSyntax>().Any())
                    return "stackalloc";
                foreach (var tok in root.DescendantTokens())
                {
                    if (tok.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.UnsafeKeyword) || tok.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.FixedKeyword))
                        return tok.Text;
                }
                //1. 所有标识符（含成员访问链的每一段）对照危险类型表
                foreach (var node in root.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    var name = node.Identifier.ValueText;
                    if (BlacklistTypes.Contains(name))
                        return name;
                }
                //2. 所有被调用的方法名对照危险方法表
                foreach (var node in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var nameExpr = node.Expression;
                    if (nameExpr is MemberAccessExpressionSyntax ma)
                    {
                        var right = ma.Name.Identifier.ValueText;
                        if (BlacklistMethods.Contains(right))
                            return right;
                    }
                    else if (nameExpr is IdentifierNameSyntax id && BlacklistMethods.Contains(id.Identifier.ValueText))
                        return id.Identifier.ValueText;
                }
                return null;
            }
            catch
            {
                //语法解析失败交给编译器报错
                return null;
            }
        }

        /// <summary>
        /// 校验实体上所有 [AiCode] 标记的代码字段：黑名单扫描+试编译（不执行），
        /// 供保存入口（DoAdd/DoEdit等）在持久化前调用，防止跳过前端直接提交恶意代码。
        /// 校验失败时向ModelState添加错误并返回false
        /// </summary>
        /// <param name="entity">业务实体</param>
        /// <param name="msd">ModelState服务，用于回传错误信息</param>
        /// <returns>全部通过返回true；有字段校验失败返回false</returns>
        public static bool ValidateAiCodeFields(object entity, IModelStateService msd)
        {
            if (entity == null)
                return true;
            foreach (var prop in entity.GetType().GetProperties())
            {
                if (prop.GetCustomAttribute<AiCodeAttribute>() == null)
                    continue;
                if (prop.PropertyType != typeof(string))
                    continue;
                var code = prop.GetValue(entity) as string;
                if (string.IsNullOrWhiteSpace(code))
                    continue;
                var funcName = prop.GetCustomAttribute<AiCodeAttribute>().FuncName;
                var hit = ScanBlacklist(code);
                if (hit != null)
                {
                    msd?.AddModelError(prop.Name, $"字段 {prop.Name} 的AI代码使用了被禁止的API：{hit}");
                    return false;
                }
                var compile = Compile(code, funcName, entity.GetType());
                if (compile.Success == false)
                {
                    var msg = compile.BlacklistHit != null
                        ? $"字段 {prop.Name} 的AI代码命中黑名单：{compile.BlacklistHit}"
                        : $"字段 {prop.Name} 的AI代码编译未通过：{(compile.Diagnostics != null && compile.Diagnostics.Count > 0 ? compile.Diagnostics[0] : "未知错误")}";
                    msd?.AddModelError(prop.Name, msg);
                    return false;
                }
            }
            return true;
        }
    }
}
