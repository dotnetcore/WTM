using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.AiCode;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// LLM调用请求，支持按节点覆盖模型、温度等参数，
    /// 未指定的参数由默认服务使用系统配置填充
    /// </summary>
    public class AgentLlmRequest
    {
        /// <summary>系统提示词</summary>
        public string System { get; set; }
        /// <summary>用户提示词</summary>
        public string User { get; set; }
        /// <summary>模型名称，空则使用系统默认</summary>
        public string Model { get; set; }
        /// <summary>温度，空则使用系统默认</summary>
        public double? Temperature { get; set; }
        /// <summary>最大Token数，空则使用系统默认</summary>
        public int? MaxTokens { get; set; }
    }

    /// <summary>
    /// 引擎运行时依赖的外部服务集合（向量、大模型、HTTP、知识库查询）。
    /// 通过委托注入，生产环境由 FromWtm 构造，单元测试可注入假实现
    /// </summary>
    public class AgentRuntimeServices
    {
        /// <summary>文本向量化服务</summary>
        public Func<string, Task<float[]>> Embedding { get; set; }
        /// <summary>大模型对话服务</summary>
        public Func<AgentLlmRequest, Task<string>> Chat { get; set; }
        /// <summary>HTTP发送服务</summary>
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> HttpSend { get; set; }
        /// <summary>知识库分块查询（默认实现已按状态、向量和数据权限过滤）</summary>
        public IQueryable<KnowledgeChunk> Chunks { get; set; }
        /// <summary>
        /// 内部API调用服务：按接口工具名（如query_Student）携带参数发起服务端调用，
        /// 由触发方（HTTP请求上下文/定时任务）注入实现；未注入时内部API节点报错
        /// </summary>
        public Func<string, System.Text.Json.JsonElement, Task<object>> InternalApiCall { get; set; }

        /// <summary>
        /// 共享HTTP客户端（API节点使用，超时通过 CancellationToken 控制）
        /// </summary>
        private static readonly HttpClient SharedHttpClient = new HttpClient();

        /// <summary>
        /// 从WTM上下文构造生产环境的默认服务：
        /// 大模型取DI中的LlmService，向量服务取IEmbeddingService（未配置时回退内置实现），
        /// 知识库查询按文档状态和DPWhere数据权限过滤
        /// </summary>
        /// <param name="wtm">WTM上下文</param>
        /// <returns>运行时服务集合</returns>
        public static AgentRuntimeServices FromWtm(WTMContext wtm)
        {
            return new AgentRuntimeServices
            {
                Embedding = async text =>
                {
                    var svc = wtm?.ServiceProvider?.GetService(typeof(IEmbeddingService)) as IEmbeddingService
                    ?? new BuiltInEmbeddingService();
                    return await svc.GetEmbeddingAsync(text);
                },
                Chat = async req =>
                {
                    var llm = wtm?.ServiceProvider?.GetService(typeof(LlmService)) as LlmService
                    ?? new LlmService(wtm?.ConfigInfo?.Llm ?? new LlmOption());
                    return await llm.ChatAsync(req.System, req.User, req.Model, req.Temperature, req.MaxTokens);
                },
                HttpSend = (msg, ct) => SharedHttpClient.SendAsync(msg, ct),
                Chunks = BuildDefaultChunksQuery(wtm),
                InternalApiCall = null,
            };
        }

        /// <summary>
        /// 构造默认的知识库分块查询：已完成且已向量化的分块，并按数据权限过滤文档
        /// </summary>
        /// <param name="wtm">WTM上下文</param>
        /// <returns>分块查询</returns>
        private static IQueryable<KnowledgeChunk> BuildDefaultChunksQuery(WTMContext wtm)
        {
            var dc = wtm?.DC;
            if (dc == null)
                return Enumerable.Empty<KnowledgeChunk>().AsQueryable();
            var permittedDocIds = dc.Set<KnowledgeDoc>().DPWhere(wtm, x => x.ID).Select(x => x.ID);
            //Include文档标题，供检索结果展示所属文档
            return dc.Set<KnowledgeChunk>()
                .Include(c => c.Doc)
                .AsNoTracking()
                .Where(c => c.Doc.Status == DocStatusEnum.Completed && c.Embedding != null)
                .Where(c => permittedDocIds.Contains(c.DocId));
        }
    }

    /// <summary>
    /// 单个节点的执行日志，运行结束后整体返回前端并持久化到AgentRun
    /// </summary>
    public class AgentNodeLog
    {
        /// <summary>节点Id</summary>
        public string NodeId { get; set; }
        /// <summary>节点名称</summary>
        public string NodeName { get; set; }
        /// <summary>节点类型</summary>
        public string NodeType { get; set; }
        /// <summary>是否执行成功</summary>
        public bool Success { get; set; }
        /// <summary>耗时（毫秒）</summary>
        public long ElapsedMs { get; set; }
        /// <summary>输入摘要（渲染后的提示词/请求等，超长截断）</summary>
        public string Input { get; set; }
        /// <summary>输出摘要（超长截断）</summary>
        public string Output { get; set; }
        /// <summary>失败时的错误信息</summary>
        public string Error { get; set; }
    }

    /// <summary>
    /// 流程执行结果：状态、最终输出、各节点日志、总耗时
    /// </summary>
    public class AgentRunResult
    {
        /// <summary>执行状态</summary>
        public AgentRunStatusEnum Status { get; set; }
        /// <summary>最终输出文本</summary>
        public string Output { get; set; }
        /// <summary>各节点执行日志（按执行顺序）</summary>
        public List<AgentNodeLog> NodeLogs { get; set; } = new List<AgentNodeLog>();
        /// <summary>失败原因</summary>
        public string Error { get; set; }
        /// <summary>总耗时（毫秒）</summary>
        public long TotalMs { get; set; }
    }

    /// <summary>
    /// Agent流程执行引擎：从开始节点沿连线依次执行各节点，
    /// 通过 {{节点Id.字段}} 模板语法在节点之间传递数据。
    /// 支持的节点类型：开始、LLM、知识库、API调用、条件分支、回复
    /// </summary>
    public class AgentExecutor
    {
        /// <summary>
        /// 单次执行的最大步数，防止条件分支组成环导致死循环
        /// </summary>
        private const int MaxSteps = 200;

        /// <summary>
        /// 日志中输入/输出摘要的最大长度
        /// </summary>
        private const int LogMaxLength = 2000;

        private static readonly Regex TemplateRegex = new Regex(@"\{\{\s*([\w\.\[\]-]+)\s*\}\}", RegexOptions.Compiled);

        private readonly AgentGraph _graph;
        private readonly AgentRuntimeServices _services;
        private readonly Dictionary<string, object> _vars;
        private readonly List<AgentNodeLog> _logs;

        private AgentExecutor(AgentGraph graph, AgentRuntimeServices services)
        {
            _graph = graph;
            _services = services;
            _vars = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _logs = new List<AgentNodeLog>();
        }

        /// <summary>
        /// 执行流程图
        /// </summary>
        /// <param name="graph">流程图定义</param>
        /// <param name="inputs">输入变量（与开始节点声明的变量对应）</param>
        /// <param name="services">运行时服务（为空时使用无外部依赖的空实现，仅适合纯模板流程）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>执行结果</returns>
        public static async Task<AgentRunResult> RunAsync(AgentGraph graph, Dictionary<string, string> inputs, AgentRuntimeServices services = null, CancellationToken cancellationToken = default)
        {
            var executor = new AgentExecutor(graph ?? new AgentGraph(), services ?? new AgentRuntimeServices());
            return await executor.RunCoreAsync(inputs ?? new Dictionary<string, string>(), cancellationToken);
        }

        /// <summary>
        /// 执行主循环：定位开始节点后沿连线推进，条件节点按出口选择分支，
        /// 回复节点或无后继节点时结束
        /// </summary>
        private async Task<AgentRunResult> RunCoreAsync(Dictionary<string, string> inputs, CancellationToken ct)
        {
            var totalWatch = Stopwatch.StartNew();
            var result = new AgentRunResult { Status = AgentRunStatusEnum.Running };
            try
            {
                var current = _graph.FindStartNode();
                if (current == null)
                    throw new InvalidOperationException("流程图中找不到开始节点");

                string lastNodeId = null;
                string answer = null;
                int steps = 0;
                while (current != null && steps < MaxSteps)
                {
                    steps++;
                    ct.ThrowIfCancellationRequested();

                    //条件节点的执行结果决定下一跳的出口
                    bool? conditionResult = null;
                    var output = await ExecuteNodeAsync(current, inputs, condition => conditionResult = condition, ct);
                    lastNodeId = current.Id;

                    //回复节点：渲染结果即最终输出，流程结束
                    if (current.Type == AgentNodeTypes.Answer)
                    {
                        answer = output as string;
                        break;
                    }

                    var handle = current.Type == AgentNodeTypes.Condition && conditionResult.HasValue
                        ? (conditionResult.Value ? "true" : "false")
                        : null;
                    current = FindNext(current.Id, handle);
                }

                if (steps >= MaxSteps)
                    throw new InvalidOperationException($"执行步数超过上限{MaxSteps}，请检查流程是否成环");

                result.Output = answer ?? (lastNodeId != null && _vars.TryGetValue(lastNodeId, out var lastVal)
                    ? Stringify(lastVal)
                    : string.Empty);
                result.Status = AgentRunStatusEnum.Success;
            }
            catch (Exception ex)
            {
                result.Status = AgentRunStatusEnum.Failed;
                result.Error = ex.Message;
                if (_logs.Count > 0 && _logs[_logs.Count - 1].Success)
                {
                    //异常发生在节点之间（如找不到下一跳）时补一条失败日志，保证前端时间线完整
                    _logs[_logs.Count - 1] = new AgentNodeLog
                    {
                        NodeId = _logs[_logs.Count - 1].NodeId,
                        NodeName = _logs[_logs.Count - 1].NodeName,
                        NodeType = _logs[_logs.Count - 1].NodeType,
                        Success = false,
                        Error = ex.Message
                    };
                }
            }
            totalWatch.Stop();
            result.TotalMs = totalWatch.ElapsedMilliseconds;
            result.NodeLogs = _logs;
            return result;
        }

        /// <summary>
        /// 按出口查找下一跳节点；条件节点优先找匹配出口的连线，找不到时回退第一条出边
        /// </summary>
        private AgentGraphNode FindNext(string nodeId, string handle)
        {
            var edges = _graph.Edges.Where(e => e.Source == nodeId).ToList();
            if (edges.Count == 0)
                return null;
            var edge = handle != null
                ? edges.FirstOrDefault(e => string.Equals(e.SourceHandle, handle, StringComparison.OrdinalIgnoreCase))
                    ?? edges[0]
                : edges[0];
            return _graph.FindNode(edge.Target);
        }

        /// <summary>
        /// 执行单个节点并记录日志；返回节点输出对象；
        /// 条件节点通过回调回传判定结果
        /// </summary>
        private async Task<object> ExecuteNodeAsync(AgentGraphNode node, Dictionary<string, string> inputs, Action<bool> onCondition, CancellationToken ct)
        {
            var log = new AgentNodeLog { NodeId = node.Id, NodeName = node.Name, NodeType = node.Type, Success = true };
            var watch = Stopwatch.StartNew();
            object output = null;
            try
            {
                switch (node.Type)
                {
                    case AgentNodeTypes.Start:
                        output = ExecuteStart(node, inputs, log);
                        break;
                    case AgentNodeTypes.LLM:
                        output = await ExecuteLlmAsync(node, log, ct);
                        break;
                    case AgentNodeTypes.Knowledge:
                        output = await ExecuteKnowledgeAsync(node, log);
                        break;
                    case AgentNodeTypes.Api:
                        output = await ExecuteApiAsync(node, log, ct);
                        break;
                    case AgentNodeTypes.InnerApi:
                        output = await ExecuteInnerApiAsync(node, log);
                        break;
                    case AgentNodeTypes.Code:
                        output = await ExecuteCodeAsync(node, log);
                        break;
                    case AgentNodeTypes.Condition:
                        output = ExecuteCondition(node, onCondition, log);
                        break;
                    case AgentNodeTypes.Answer:
                        output = ExecuteAnswer(node, log);
                        break;
                    default:
                        throw new InvalidOperationException($"不支持的节点类型：{node.Type}");
                }
                _vars[node.Id] = output;
                log.Output = Truncate(Stringify(output));
            }
            catch (Exception ex)
            {
                log.Success = false;
                log.Error = ex.Message;
                watch.Stop();
                log.ElapsedMs = watch.ElapsedMilliseconds;
                _logs.Add(log);
                throw;
            }
            watch.Stop();
            log.ElapsedMs = watch.ElapsedMilliseconds;
            _logs.Add(log);
            return output;
        }

        /// <summary>
        /// 开始节点：把输入变量放入变量表供后续节点引用
        /// </summary>
        private object ExecuteStart(AgentGraphNode node, Dictionary<string, string> inputs, AgentNodeLog log)
        {
            var vars = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in inputs)
                vars[kv.Key] = kv.Value;
            log.Input = JsonSerializer.Serialize(inputs);
            return vars;
        }

        /// <summary>
        /// LLM节点：渲染系统/用户提示词后调用大模型，输出为回答文本
        /// </summary>
        private async Task<object> ExecuteLlmAsync(AgentGraphNode node, AgentNodeLog log, CancellationToken ct)
        {
            var cfg = AgentNodeConfigResolver.Resolve<LlmConfig>(node);
            var system = RenderTemplate(cfg.SystemPrompt);
            var user = RenderTemplate(cfg.UserPrompt);
            if (string.IsNullOrWhiteSpace(user))
                throw new InvalidOperationException("LLM节点的用户提示词不能为空");
            if (_services.Chat == null)
                throw new InvalidOperationException("未配置大模型服务（Llm）");
            log.Input = Truncate((string.IsNullOrEmpty(system) ? "" : $"[system]{system}\n") + $"[user]{user}");
            return await _services.Chat(new AgentLlmRequest
            {
                System = system,
                User = user,
                Model = cfg.Model,
                Temperature = cfg.Temperature,
                MaxTokens = cfg.MaxTokens
            });
        }

        /// <summary>
        /// 知识库节点：渲染查询文本并向量化，与已向量化的分块计算余弦相似度，
        /// 输出包含 text（拼接文本）和 results（分块明细列表）
        /// </summary>
        private async Task<object> ExecuteKnowledgeAsync(AgentGraphNode node, AgentNodeLog log)
        {
            var cfg = AgentNodeConfigResolver.Resolve<KnowledgeConfig>(node);
            var query = RenderTemplate(cfg.Query);
            if (string.IsNullOrWhiteSpace(query))
                throw new InvalidOperationException("知识库节点的查询内容不能为空");
            if (_services.Embedding == null || _services.Chunks == null)
                throw new InvalidOperationException("未配置向量服务或知识库数据源");
            log.Input = Truncate(query);

            var queryVector = await _services.Embedding(query);
            var chunks = _services.Chunks.ToList();
            if (cfg.DocIds != null && cfg.DocIds.Count > 0)
                chunks = chunks.Where(c => cfg.DocIds.Contains(c.DocId)).ToList();

            var results = chunks
                .Select(c => new
                {
                    Title = c.Doc?.Title,
                    DocId = c.DocId.ToString(),
                    c.Content,
                    Score = CosineSimilarity(queryVector, DeserializeEmbedding(c.Embedding))
                })
                .Where(x => x.Score >= cfg.Threshold)
                .OrderByDescending(x => x.Score)
                .Take(cfg.TopK <= 0 ? 3 : cfg.TopK)
                .ToList();

            var output = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["text"] = string.Join("\n---\n", results.Select(r => r.Content)),
                ["results"] = results.Select(r => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["title"] = r.Title,
                    ["docId"] = r.DocId,
                    ["content"] = r.Content,
                    ["score"] = Math.Round(r.Score, 4)
                }).ToList()
            };
            return output;
        }

        /// <summary>
        /// API节点：渲染URL/请求头/请求体后发送HTTP请求，
        /// 输出包含 status（状态码）、body（响应文本）、json（解析后的响应JSON，解析失败为null）
        /// </summary>
        private async Task<object> ExecuteApiAsync(AgentGraphNode node, AgentNodeLog log, CancellationToken ct)
        {
            var cfg = AgentNodeConfigResolver.Resolve<ApiConfig>(node);
            var url = RenderTemplate(cfg.Url);
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("API节点的请求地址不能为空");
            var method = (cfg.Method ?? "GET").ToUpperInvariant();
            if (!new[] { "GET", "POST", "PUT", "DELETE", "PATCH" }.Contains(method))
                throw new InvalidOperationException($"不支持的HTTP方法：{method}");

            using var request = new HttpRequestMessage(new HttpMethod(method), url);
            if (cfg.Headers != null)
            {
                foreach (var h in cfg.Headers)
                {
                    if (string.IsNullOrWhiteSpace(h?.Key))
                        continue;
                    var value = RenderTemplate(h.Value);
                    if (!request.Headers.TryAddWithoutValidation(h.Key, value))
                        request.Content?.Headers.TryAddWithoutValidation(h.Key, value);
                }
            }
            string body = null;
            if (method != "GET" && !string.IsNullOrWhiteSpace(cfg.Body))
            {
                body = RenderTemplate(cfg.Body);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            log.Input = Truncate($"{method} {url}" + (body == null ? "" : $"\n{body}"));

            if (_services.HttpSend == null)
                throw new InvalidOperationException("未配置HTTP服务");

            var timeout = cfg.TimeoutSeconds <= 0 ? 30 : cfg.TimeoutSeconds;
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeout));

            using var response = await _services.HttpSend(request, timeoutCts.Token);
            var responseBody = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            JsonElement? json = null;
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                json = JsonSerializer.SerializeToElement(doc.RootElement.Clone());
            }
            catch
            {
                //响应不是合法JSON时json保持为null，body仍可使用
            }
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"API调用失败：HTTP {(int)response.StatusCode} {Truncate(responseBody, 500)}");

            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["status"] = (int)response.StatusCode,
                ["body"] = responseBody,
                ["json"] = json
            };
        }

        /// <summary>
        /// 内部API节点：按变量渲染参数后调用框架自动发现的系统接口
        /// （与Agent对话工具同源），输出包含 body（原始JSON文本）和 json（解析后的对象）
        /// </summary>
        private async Task<object> ExecuteInnerApiAsync(AgentGraphNode node, AgentNodeLog log)
        {
            var cfg = AgentNodeConfigResolver.Resolve<InnerApiConfig>(node);
            if (string.IsNullOrWhiteSpace(cfg.ToolName))
                throw new InvalidOperationException("内部API节点未选择接口");
            if (_services.InternalApiCall == null)
                throw new InvalidOperationException("当前运行环境不支持内部API调用");
            log.Input = Truncate(cfg.ToolName);

            //参数值做变量渲染后组装为JSON对象，数值/布尔按字面量传
            var argDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in cfg.Args ?? new List<AgentKeyValue>())
            {
                if (string.IsNullOrWhiteSpace(a?.Key))
                    continue;
                var rendered = RenderTemplate(a.Value ?? "");
                if (long.TryParse(rendered, out var l))
                    argDict[a.Key] = l;
                else if (double.TryParse(rendered, out var d))
                    argDict[a.Key] = d;
                else if (bool.TryParse(rendered, out var b))
                    argDict[a.Key] = b;
                else
                    argDict[a.Key] = rendered;
            }
            var argsJson = System.Text.Json.JsonSerializer.SerializeToElement(argDict);
            var result = await _services.InternalApiCall(cfg.ToolName, argsJson);

            //统一为与外部API一致的输出形态：body原文 + json对象
            var bodyText = result is System.Text.Json.JsonElement je
                ? je.GetRawText()
                : System.Text.Json.JsonSerializer.Serialize(result);
            object json = null;
            try
            {
                json = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(bodyText);
            }
            catch { }
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["body"] = bodyText,
                ["json"] = json,
            };
        }

        /// <summary>
        /// 代码节点：编译（复用AI代码的缓存/黑名单设施）并执行单个C#静态方法，
        /// 参数值按变量渲染后转换为参数类型；方法返回值即节点输出
        /// </summary>
        private async Task<object> ExecuteCodeAsync(AgentGraphNode node, AgentNodeLog log)
        {
            var cfg = AgentNodeConfigResolver.Resolve<CodeConfig>(node);
            if (string.IsNullOrWhiteSpace(cfg.Code))
                throw new InvalidOperationException("代码节点内容为空");
            if (string.IsNullOrWhiteSpace(cfg.MethodName))
                throw new InvalidOperationException("代码节点缺少方法名，请先执行语法检查");
            log.Input = Truncate($"{cfg.MethodName}({string.Join(",", (cfg.ParamValues ?? new List<AgentKeyValue>()).Select(p => p.Key))})");

            var compile = AiCodeCompiler.Compile(cfg.Code, cfg.MethodName, null);
            if (compile.Success == false)
                throw new InvalidOperationException("代码编译失败：" + string.Join("；", compile.Diagnostics));

            var parameters = compile.Method.GetParameters();
            var bound = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                var def = (cfg.ParamValues ?? new List<AgentKeyValue>()).FirstOrDefault(x => string.Equals(x.Key, p.Name, StringComparison.Ordinal));
                var raw = def == null ? null : RenderTemplate(def.Value ?? "");
                bound[i] = AiArgParser.ConvertToType(raw, p.ParameterType, p.Name);
            }

            //复用AI代码的超时保护：可取消任务中执行同步方法调用
            var invoker = Task.Run(() => compile.Method.Invoke(null, bound));
            var finished = await Task.WhenAny(invoker, Task.Delay(AiCodeRunner.TimeoutMs));
            if (finished != invoker)
                throw new InvalidOperationException($"代码执行超时（>{AiCodeRunner.TimeoutMs}毫秒）");
            if (invoker.IsFaulted)
                throw invoker.Exception.GetBaseException();
            return invoker.Result;
        }

        /// <summary>
        /// 条件节点：渲染左右操作数并按操作符比较，通过回调回传结果，
        /// 输出为true/false字符串
        /// </summary>
        private object ExecuteCondition(AgentGraphNode node, Action<bool> onCondition, AgentNodeLog log)
        {
            var cfg = AgentNodeConfigResolver.Resolve<ConditionConfig>(node);
            var left = RenderTemplate(cfg.Left);
            var right = RenderTemplate(cfg.Right);
            log.Input = $"{cfg.Operator}: {Truncate(left, 200)} / {Truncate(right, 200)}";
            var result = Evaluate(left, cfg.Operator ?? ConditionOperator.Equals, right);
            onCondition(result);
            return result ? "true" : "false";
        }

        /// <summary>
        /// 回复节点：渲染模板文本，作为流程最终输出
        /// </summary>
        private object ExecuteAnswer(AgentGraphNode node, AgentNodeLog log)
        {
            var cfg = AgentNodeConfigResolver.Resolve<AnswerConfig>(node);
            var text = RenderTemplate(cfg.Text);
            log.Input = Truncate(cfg.Text);
            return text;
        }

        /// <summary>
        /// 渲染模板：把 {{节点Id.字段}} 形式的变量替换为变量表中的值；
        /// 无法解析的变量替换为空字符串
        /// </summary>
        private string RenderTemplate(string template)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;
            return TemplateRegex.Replace(template, m =>
            {
                var value = ResolvePath(m.Groups[1].Value);
                return value == null ? string.Empty : Stringify(value);
            });
        }

        /// <summary>
        /// 按路径解析变量：首段为节点Id，后续段依次进入字典键、对象属性或数组下标
        /// </summary>
        private object ResolvePath(string path)
        {
            var key = path;
            var rest = "";
            var dotIndex = path.IndexOf('.');
            if (dotIndex >= 0)
            {
                key = path.Substring(0, dotIndex);
                rest = path.Substring(dotIndex);
            }
            if (!_vars.TryGetValue(key, out var value))
                return null;
            foreach (var token in SplitPathTokens(rest))
            {
                value = Dig(value, token);
                if (value == null)
                    return null;
            }
            return value;
        }

        /// <summary>
        /// 把路径剩余部分拆成字段名/下标令牌，例如 .results[0].content → [results, 0, content]
        /// </summary>
        private static IEnumerable<string> SplitPathTokens(string rest)
        {
            var tokens = new List<string>();
            var sb = new StringBuilder();
            foreach (var c in rest)
            {
                if (c == '.')
                {
                    if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
                }
                else if (c == '[' || c == ']')
                {
                    if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
                }
                else
                {
                    sb.Append(c);
                }
            }
            if (sb.Length > 0) tokens.Add(sb.ToString());
            return tokens;
        }

        /// <summary>
        /// 单步取值：字典按键（忽略大小写）、JsonElement按属性/数组下标、
        /// 列表按下标、其他对象按反射属性
        /// </summary>
        private static object Dig(object value, string token)
        {
            if (value is Dictionary<string, object> dict)
                return dict.TryGetValue(token, out var v) ? v : null;
            if (value is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Object && je.TryGetProperty(token, out var prop))
                    return prop;
                if (je.ValueKind == JsonValueKind.Array && int.TryParse(token, out var index) && index >= 0 && index < je.GetArrayLength())
                    return je[index];
                return null;
            }
            if (value is System.Collections.IList list && int.TryParse(token, out var idx) && idx >= 0 && idx < list.Count)
                return list[idx];
            var prop2 = value?.GetType().GetProperty(token);
            return prop2?.GetValue(value);
        }

        /// <summary>
        /// 把变量值转为字符串：字符串原样返回，JsonElement返回原始JSON文本，其他对象序列化为JSON
        /// </summary>
        private static string Stringify(object value)
        {
            return value switch
            {
                null => string.Empty,
                string s => s,
                JsonElement je => je.ValueKind == JsonValueKind.String ? je.GetString() : je.GetRawText(),
                _ => JsonSerializer.Serialize(value)
            };
        }

        /// <summary>
        /// 条件求值：文本类操作符按字符串比较，gt/lt优先按数值比较（解析失败回退字符串比较）
        /// </summary>
        private static bool Evaluate(string left, string op, string right)
        {
            switch (op)
            {
                case ConditionOperator.Equals:
                    return string.Equals(left ?? "", right ?? "", StringComparison.OrdinalIgnoreCase);
                case ConditionOperator.NotEquals:
                    return !string.Equals(left ?? "", right ?? "", StringComparison.OrdinalIgnoreCase);
                case ConditionOperator.Contains:
                    return (left ?? "").Contains(right ?? "", StringComparison.OrdinalIgnoreCase);
                case ConditionOperator.NotContains:
                    return !(left ?? "").Contains(right ?? "", StringComparison.OrdinalIgnoreCase);
                case ConditionOperator.StartsWith:
                    return (left ?? "").StartsWith(right ?? "", StringComparison.OrdinalIgnoreCase);
                case ConditionOperator.EndsWith:
                    return (left ?? "").EndsWith(right ?? "", StringComparison.OrdinalIgnoreCase);
                case ConditionOperator.GreaterThan:
                case ConditionOperator.LessThan:
                    if (double.TryParse(left, out var ld) && double.TryParse(right, out var rd))
                        return op == ConditionOperator.GreaterThan ? ld > rd : ld < rd;
                    var cmp = string.CompareOrdinal(left ?? "", right ?? "");
                    return op == ConditionOperator.GreaterThan ? cmp > 0 : cmp < 0;
                case ConditionOperator.IsEmpty:
                    return string.IsNullOrWhiteSpace(left);
                case ConditionOperator.IsNotEmpty:
                    return !string.IsNullOrWhiteSpace(left);
                default:
                    throw new InvalidOperationException($"不支持的条件操作符：{op}");
            }
        }

        /// <summary>
        /// 计算两个向量的余弦相似度
        /// </summary>
        private static float CosineSimilarity(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return 0f;
            float dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }
            var denom = MathF.Sqrt(normA) * MathF.Sqrt(normB);
            return denom < 1e-10f ? 0f : dot / denom;
        }

        /// <summary>
        /// 把数据库中存储的向量字节还原为float数组
        /// </summary>
        private static float[] DeserializeEmbedding(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var floats = new float[data.Length / sizeof(float)];
            Buffer.BlockCopy(data, 0, floats, 0, data.Length);
            return floats;
        }

        /// <summary>
        /// 截断文本用于日志展示
        /// </summary>
        private static string Truncate(string text, int maxLength = LogMaxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            return text.Substring(0, maxLength) + "...";
        }
    }
}
