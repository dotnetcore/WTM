using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// Agent对话模式的一步交互记录（发给模型的请求、模型的响应、工具执行结果），
    /// 原样保留完整JSON供前端调试面板展示
    /// </summary>
    public class AgentChatTraceItem
    {
        /// <summary>工具循环轮次，从1开始</summary>
        public int Round { get; set; }
        /// <summary>记录类型：llm-request / llm-response / tool-call / user</summary>
        public string Kind { get; set; }
        /// <summary>可读标题，如“第1轮 · 发送给大模型”</summary>
        public string Title { get; set; }
        /// <summary>该步骤耗时（毫秒）</summary>
        public long ElapsedMs { get; set; }
        /// <summary>完整内容（格式化JSON或文本）</summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// Agent对话模式的执行结果：最终回答与全部交互轨迹
    /// </summary>
    public class AgentChatResult
    {
        /// <summary>模型的最终回答文本</summary>
        public string Answer { get; set; }
        /// <summary>本次问答的全部交互轨迹</summary>
        public List<AgentChatTraceItem> Trace { get; set; } = new List<AgentChatTraceItem>();
    }

    /// <summary>
    /// Agent可调用的工具：定义（名称/描述/参数Schema）+ 执行器。
    /// 知识库检索、反射发现的业务查询API等都以统一的工具形式注册给大模型
    /// </summary>
    public class AgentTool
    {
        /// <summary>工具定义</summary>
        public LlmToolDefinition Definition { get; set; }
        /// <summary>执行器：接收模型给出的参数，返回将回填给模型的结果对象</summary>
        public Func<JsonElement, CancellationToken, Task<object>> Executor { get; set; }
    }

    /// <summary>
    /// Agent对话服务：把系统能力（知识库检索、业务查询API等）注册为工具交给大模型（function calling），
    /// 模型自主决定调用哪些工具、什么顺序、调用几次，服务端执行工具并把结果回填继续对话，
    /// 直到模型给出最终回答。无需预先编排流程。
    /// </summary>
    public class AgentChatService
    {
        /// <summary>
        /// 工具调用循环的最大轮数，防止模型无限要求调用工具
        /// </summary>
        private const int MaxRounds = 8;

        /// <summary>
        /// 工具返回内容中单个分块正文的最大长度，超出截断以控制Token消耗
        /// </summary>
        private const int MaxChunkLength = 1500;

        //宽松编码器：序列化时不把中文转义成\uXXXX，保证调试面板可读
        private static readonly JsonSerializerOptions Pretty = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        //发给模型的工具结果用紧凑格式（省token；带缩进的格式仅用于右侧明细面板展示）
        private static readonly JsonSerializerOptions Compact = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// 默认系统提示词：平衡立场——业务数据问题组合调用查询工具，常识性问题直接回答；
        /// 存在写操作工具时要求先确认再执行
        /// </summary>
        public const string DefaultSystemPrompt =
            "你是WTM框架的智能助手。系统已把可用的业务接口自动注册为工具（可按条件查询学生、文档、日志等业务数据的列表），" +
            "并提供了知识库语义检索工具。回答业务数据相关问题时，按需组合调用工具获取事实依据——" +
            "例如先查询具体对象的基础信息，再检索知识库中的相关标准，最后把工具返回的数据与自身知识综合后作答；" +
            "工具查不到的内容要如实说明。一般常识性问题可直接回答，不必调用工具。" +
            "若系统提供了新增/修改/删除等写操作工具，涉及信息不全时先向用户追问，执行前简要说明将要做什么。";

        /// <summary>
        /// 执行一次Agent对话：携带历史与全部工具循环调用大模型，
        /// 模型每次要求调用工具时执行并把结果回填，最终返回回答与完整轨迹
        /// </summary>
        /// <param name="wtm">WTM上下文（提供LLM服务、知识库数据与数据权限）</param>
        /// <param name="history">历史消息（role/content），首次为空列表</param>
        /// <param name="userMessage">本轮用户提问</param>
        /// <param name="extraTools">额外注册的工具（如反射发现的业务查询API）；知识库检索工具自动附加</param>
        /// <param name="systemPrompt">自定义系统提示词，空用默认平衡提示词</param>
        /// <param name="topK">知识库检索返回分块数上限，空用默认3</param>
        /// <param name="threshold">知识库检索相似度阈值，空用默认0.1</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>最终回答与全部交互轨迹</returns>
        public static async Task<AgentChatResult> ChatAsync(WTMContext wtm, List<Dictionary<string, string>> history, string userMessage, IReadOnlyList<AgentTool> extraTools = null, string systemPrompt = null, int? topK = null, double? threshold = null, Func<string, JsonElement, Task<object>> internalApiCall = null, CancellationToken cancellationToken = default)
        {
            var llm = wtm?.ServiceProvider?.GetService(typeof(LlmService)) as LlmService
                ?? new LlmService(wtm?.ConfigInfo?.Llm ?? new LlmOption());
            var runtime = AgentRuntimeServices.FromWtm(wtm);
            runtime.InternalApiCall = internalApiCall;
            var result = new AgentChatResult();
            result.Trace.Add(new AgentChatTraceItem { Kind = "user", Title = "用户提问", Content = userMessage });

            //组装消息历史：系统提示词（追加当前日期，模型自身没有实时时间概念）+ 传入的对话历史（仅保留文本消息）+ 本轮问题。
            //日期只精确到天：一天内前缀不变，不影响命中LLM前缀缓存
            var week = new[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" }[(int)DateTime.Now.DayOfWeek];
            var systemContent = (string.IsNullOrWhiteSpace(systemPrompt) ? DefaultSystemPrompt : systemPrompt)
                + $"\n当前日期：{DateTime.Now:yyyy年M月d日} {week}。涉及“今天/现在/最新”等问题时注意参考该日期。";
            var messages = new List<object>
            {
                new { role = "system", content = systemContent }
            };
            foreach (var m in history ?? new List<Dictionary<string, string>>())
            {
                if (m == null || string.IsNullOrEmpty(m.GetValueOrDefault("role")) || string.IsNullOrEmpty(m.GetValueOrDefault("content")))
                    continue;
                if (m["role"] == "user" || m["role"] == "assistant")
                    messages.Add(new { role = m["role"], content = m["content"] });
            }
            messages.Add(new { role = "user", content = userMessage });

            //工具注册：知识库检索 + 已发布为技能的编排流程 + 外部传入的业务接口工具
            var tools = new List<AgentTool> { BuildKnowledgeTool(runtime, topK ?? 3, threshold ?? 0.1) };
            tools.AddRange(BuildFlowTools(wtm, internalApiCall));
            if (extraTools != null)
                tools.AddRange(extraTools.Where(t => t?.Definition != null && t.Executor != null));
            var toolDefs = tools.Select(t => t.Definition).ToList();

            for (int round = 1; round <= MaxRounds; round++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //轨迹：完整记录发给模型的内容（消息历史+工具定义）；Schema解析失败时降级为原文，避免拖垮整个请求
                var requestPreview = JsonSerializer.Serialize(new
                {
                    messages = messages.Select(m => SafeJsonElement(m)).ToList(),
                    tools = toolDefs.Select(t => new
                    {
                        type = "function",
                        function = new { name = t.Name, description = t.Description, parameters = SafeJsonElement(t.ParametersJsonSchema) }
                    })
                }, Pretty);
                result.Trace.Add(new AgentChatTraceItem { Round = round, Kind = "llm-request", Title = $"第{round}轮 · 发送给大模型（消息历史+{toolDefs.Count}个工具定义）", Content = requestPreview });

                var watch = Stopwatch.StartNew();
                var resp = await llm.ChatWithToolsAsync(messages, toolDefs);
                watch.Stop();
                //缓存命中信息（系统提示词+工具定义+历史前缀固定不变，可命中DeepSeek的自动前缀缓存）
                var cacheInfo = resp.CacheHitTokens > 0 && resp.PromptTokens > 0
                    ? $"，缓存命中{resp.CacheHitTokens}/{resp.PromptTokens}tokens"
                    : "";
                result.Trace.Add(new AgentChatTraceItem
                {
                    Round = round,
                    Kind = "llm-response",
                    Title = $"第{round}轮 · 大模型返回（{watch.ElapsedMilliseconds}ms{cacheInfo}）",
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Content = resp.RawResponse,
                });

                //模型不再要求调用工具：文本即最终回答
                if (resp.ToolCalls.Count == 0)
                {
                    result.Answer = resp.Content ?? string.Empty;
                    return result;
                }

                //按协议把assistant的tool_calls消息原样回填历史
                messages.Add(resp.AssistantMessage);

                foreach (var tc in resp.ToolCalls)
                {
                    var tool = tools.FirstOrDefault(t => string.Equals(t.Definition.Name, tc.Name, StringComparison.Ordinal));
                    watch.Restart();
                    object toolResult;
                    try
                    {
                        toolResult = tool == null
                            ? new { error = $"未知工具：{tc.Name}" }
                            : await ExecuteToolSafelyAsync(tool, tc, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        toolResult = new { error = $"工具执行异常：{ex.Message}" };
                    }
                    watch.Stop();
                    //回填给模型用紧凑JSON（固定文字更少更稳定），轨迹展示用带缩进的可读版本
                    var toolJson = JsonSerializer.Serialize(toolResult, Compact);
                    result.Trace.Add(new AgentChatTraceItem
                    {
                        Round = round,
                        Kind = "tool-call",
                        Title = $"第{round}轮 · 执行工具 {tc.Name}，结果回填给大模型",
                        ElapsedMs = watch.ElapsedMilliseconds,
                        Content = $"调用参数：\n{PrettyJsonString(tc.ArgumentsJson)}\n\n工具返回：\n{JsonSerializer.Serialize(toolResult, Pretty)}"
                    });
                    //工具结果以tool消息回填，tool_call_id必须与请求一致
                    messages.Add(new Dictionary<string, object>
                    {
                        ["role"] = "tool",
                        ["tool_call_id"] = tc.Id,
                        ["content"] = toolJson
                    });
                }
            }

            result.Answer = "已达到工具调用轮数上限，未能生成最终回答，请尝试更具体的问题。";
            return result;
        }

        /// <summary>
        /// 把对象或JSON字符串安全转为JsonElement：对象走序列化往返，JSON字符串解析失败时返回null占位
        /// </summary>
        private static JsonElement SafeJsonElement(object obj)
        {
            try
            {
                if (obj is string s)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        return JsonDocument.Parse("null").RootElement.Clone();
                    return JsonDocument.Parse(s).RootElement.Clone();
                }
                return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(obj));
            }
            catch
            {
                return JsonDocument.Parse("null").RootElement.Clone();
            }
        }

        /// <summary>
        /// 解析工具参数并执行，执行异常统一转为带error说明的对象回填给模型
        /// </summary>
        private static async Task<object> ExecuteToolSafelyAsync(AgentTool tool, LlmToolCall tc, CancellationToken ct)
        {
            JsonElement args;
            try
            {
                args = string.IsNullOrWhiteSpace(tc.ArgumentsJson)
                    ? default
                    : JsonDocument.Parse(tc.ArgumentsJson).RootElement.Clone();
            }
            catch
            {
                return new { error = "参数不是有效JSON" };
            }
            return await tool.Executor(args, ct);
        }

        /// <summary>
        /// 把标记为AI技能的编排流程注册为对话工具：
        /// 工具描述取流程说明，参数Schema取开始节点声明的输入变量，
        /// 模型按用户意图自动选择调用；执行结果（最终输出/各节点日志）回填给模型
        /// </summary>
        /// <param name="wtm">WTM上下文</param>
        /// <param name="internalApiCall">内部API调用委托（流程含内部API节点时需要，由Mvc层注入HTTP自调用实现）</param>
        /// <returns>技能工具列表</returns>
        public static List<AgentTool> BuildFlowTools(WTMContext wtm, Func<string, JsonElement, Task<object>> internalApiCall = null)
        {
            var tools = new List<AgentTool>();
            var dc = wtm?.DC;
            if (dc == null)
                return tools;
            try
            {
                AgentTableHelper.EnsureTables(dc);
                var flows = dc.Set<AgentFlow>().AsNoTracking()
                    .Where(x => x.IsValid == true && x.IsSkill == true)
                    .ToList();
                foreach (var flow in flows)
                {
                    AgentGraph graph = null;
                    try { graph = AgentGraph.Parse(flow.GraphJson); }
                    catch { continue; }
                    var start = graph.FindStartNode();
                    if (start == null)
                        continue;

                    //参数Schema按开始节点的输入变量生成（均为字符串）
                    var vars = AgentNodeConfigResolver.Resolve<StartConfig>(start)?.Variables ?? new List<string>();
                    var props = new Dictionary<string, object>();
                    foreach (var v in vars)
                        props[v] = new { type = "string", description = $"流程输入变量 {v}" };
                    var schema = JsonSerializer.Serialize(new
                    {
                        type = "object",
                        properties = props,
                        required = Array.Empty<string>()
                    }, Compact);

                    var flowName = flow.Name;
                    var flowDesc = string.IsNullOrWhiteSpace(flow.Description) ? "自动化流程" : flow.Description;
                    var flowId = flow.ID;
                    tools.Add(new AgentTool
                    {
                        Definition = new LlmToolDefinition
                        {
                            //工具名只允许字母数字下划线连字符，用ID短码保证合法，可读名称放描述里
                            Name = "run_flow_" + flowId.ToString("N")[..8],
                            Description = $"执行已编排的自动化流程「{flowName}」：{flowDesc}。当用户的请求与该流程的用途匹配时调用。",
                            ParametersJsonSchema = schema,
                        },
                        Executor = async (args, ct) =>
                        {
                            var inputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            if (args.ValueKind == JsonValueKind.Object)
                            {
                                foreach (var p in args.EnumerateObject())
                                    inputs[p.Name] = p.Value.ToString();
                            }
                            var runtime = AgentRuntimeServices.FromWtm(wtm);
                            runtime.InternalApiCall = internalApiCall;
                            var result = await AgentExecutor.RunAsync(graph, inputs, runtime, ct);
                            return new
                            {
                                status = result.Status.ToString(),
                                output = result.Output,
                                error = result.Error,
                                totalMs = result.TotalMs,
                                nodeLogs = result.NodeLogs.Select(l => new { l.NodeName, l.NodeType, l.Success, l.Error, l.ElapsedMs }),
                            };
                        },
                    });
                }
            }
            catch
            {
                //流程表不存在等异常时返回空技能列表，不影响对话
            }
            return tools;
        }

        /// <summary>
        /// 构造知识库语义检索工具：查询文本向量化后与已完成文档的分块算余弦相似度，
        /// 按阈值过滤、得分降序取前topK条；分块正文超长时截断
        /// </summary>
        public static AgentTool BuildKnowledgeTool(AgentRuntimeServices runtime, int topK, double threshold)
        {
            return new AgentTool
            {
                Definition = new LlmToolDefinition
                {
                    Name = "knowledge_search",
                    Description = "在系统知识库中按语义检索文档片段，适合查询公司制度、业务标准、业务资料等文档内容，返回片段所属文档标题、正文与相似度得分。",
                    ParametersJsonSchema = "{\"type\":\"object\",\"properties\":{\"query\":{\"type\":\"string\",\"description\":\"检索的问题或关键词\"},\"topK\":{\"type\":\"integer\",\"description\":\"返回的最大片段数，默认3，范围1-10\"}},\"required\":[\"query\"]}"
                },
                Executor = async (args, ct) =>
                {
                    string query = null;
                    int effectiveTopK = topK;
                    if (args.ValueKind == JsonValueKind.Object)
                    {
                        if (args.TryGetProperty("query", out var q) && q.ValueKind == JsonValueKind.String)
                            query = q.GetString();
                        if (args.TryGetProperty("topK", out var k) && k.ValueKind == JsonValueKind.Number && k.TryGetInt32(out var kk))
                            effectiveTopK = Math.Clamp(kk, 1, 10);
                    }
                    if (string.IsNullOrWhiteSpace(query))
                        return new { error = "缺少必填参数query" };
                    return await SearchKnowledgeAsync(runtime, query, effectiveTopK, threshold, ct);
                }
            };
        }

        /// <summary>
        /// 知识库语义检索工具：查询文本向量化后与已完成文档的分块算余弦相似度，
        /// 按阈值过滤、得分降序取前topK条；分块正文超长时截断
        /// </summary>
        private static async Task<object> SearchKnowledgeAsync(AgentRuntimeServices runtime, string query, int topK, double threshold, CancellationToken ct)
        {
            var queryVector = await runtime.Embedding(query);
            var chunks = runtime.Chunks.ToList();
            var hits = chunks
                .Select(c => new
                {
                    Title = c.Doc?.Title,
                    c.Content,
                    Score = Math.Round(CosineSimilarity(queryVector, DeserializeEmbedding(c.Embedding)), 4)
                })
                .Where(x => x.Score >= threshold)
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => new
                {
                    x.Title,
                    Content = x.Content != null && x.Content.Length > MaxChunkLength ? x.Content.Substring(0, MaxChunkLength) + "…" : x.Content,
                    x.Score
                })
                .ToList();
            if (hits.Count == 0)
                return new { message = "未在知识库中检索到相关内容", results = hits };
            return new { message = $"检索到{hits.Count}条相关内容", results = hits };
        }

        /// <summary>
        /// 把数据库中存储的向量字节还原为float数组
        /// </summary>
        private static float[] DeserializeEmbedding(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return Array.Empty<float>();
            var vec = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, vec, 0, bytes.Length);
            return vec;
        }

        /// <summary>
        /// 计算两个向量的余弦相似度；任一为空或零向量时返回0
        /// </summary>
        private static double CosineSimilarity(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length == 0 || a.Length != b.Length)
                return 0;
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            if (na == 0 || nb == 0)
                return 0;
            return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
        }

        /// <summary>
        /// 尝试把JSON字符串格式化展示，解析失败原样返回
        /// </summary>
        private static string PrettyJsonString(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, Pretty);
            }
            catch
            {
                return json ?? "";
            }
        }
    }
}
