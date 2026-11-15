using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Agent;
using WalkingTec.Mvvm.Core.Support.AiCode;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Support;

//注意：本控制器不能放在WalkingTec.Mvvm.Mvc命名空间下——
//框架构建模块列表(GetAllModules)会跳过该命名空间的控制器，导致菜单管理无法识别本模块及配置接口权限
namespace WalkingTec.Mvvm.AgentApi
{
    /// <summary>
    /// Agent编排API控制器，提供流程的增删改查、执行、运行历史，
    /// 以及设计器需要的知识库文档列表和默认大模型信息。
    /// 各接口均可在菜单管理中按角色配置权限（未配置时严格模式下不可访问）
    /// </summary>
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/agent")]
    [ActionDescription("MenuKey.AgentFlow")]
    public class AgentApiController : BaseApiController
    {
        /// <summary>
        /// 分页查询流程列表（只返回有效流程，按更新时间倒序）
        /// </summary>
        /// <param name="name">名称关键字，模糊匹配</param>
        /// <param name="page">页码，从1开始</param>
        /// <param name="limit">每页条数</param>
        /// <returns>流程列表与总数</returns>
        [ActionDescription("查询流程列表")]
        [NoLog]
        [HttpGet("flows")]
        public IActionResult GetFlows(string name = null, int page = 1, int limit = 20)
        {
            AgentTableHelper.EnsureTables(DC);
            var query = DC.Set<AgentFlow>().Where(x => x.IsValid == true);
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Name.Contains(name));
            var count = query.Count();
            var data = query.OrderByDescending(x => x.UpdateTime ?? x.CreateTime)
                .Skip((page - 1) * limit).Take(limit)
                .Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.Description,
                    x.CreateTime,
                    x.CreateBy,
                    x.UpdateTime,
                    x.UpdateBy
                })
                .ToList();
            return Ok(new { Data = data, Count = count });
        }

        /// <summary>
        /// 分页查询流程列表（标准ListVM管道，供WtmTable的doSearch调用）
        /// </summary>
        /// <param name="searcher">搜索器</param>
        /// <returns>标准列表JSON</returns>
        [ActionDescription("分页查询流程")]
        [NoLog]
        [HttpPost("flows/search")]
        public IActionResult SearchFlows(Core.Support.Agent.AgentFlowSearcher searcher)
        {
            AgentTableHelper.EnsureTables(DC);
            var vm = Wtm.CreateVM<Core.Support.Agent.AgentFlowListVM>();
            vm.Searcher = searcher;
            return Content(vm.GetJson(), "application/json");
        }

        /// <summary>
        /// 获取单个流程（含流程图JSON与发布方式），用于编辑器加载
        /// </summary>
        /// <param name="id">流程ID</param>
        /// <returns>流程对象，不存在返回404</returns>
        [ActionDescription("获取流程")]
        [NoLog]
        [HttpGet("flows/{id}")]
        public IActionResult GetFlow(Guid id)
        {
            AgentTableHelper.EnsureTables(DC);
            var flow = DC.Set<AgentFlow>().Where(x => x.ID == id && x.IsValid == true).FirstOrDefault();
            if (flow == null)
                return NotFound();
            return Ok(new { flow.ID, flow.Name, flow.Description, GraphJson = flow.GraphJson, flow.IsSkill, flow.IsScheduled, flow.ScheduleCron, flow.ScheduleInputsJson });
        }

        /// <summary>
        /// 保存流程：id为空时新建，否则更新；
        /// 保存前校验流程图可解析且包含开始节点
        /// </summary>
        /// <param name="input">流程数据（id可选、名称、说明、流程图JSON）</param>
        /// <returns>保存后的流程ID</returns>
        [ActionDescription("保存流程")]
        [HttpPost("flows")]
        public IActionResult SaveFlow([FromBody] AgentFlowInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorJson());
            AgentTableHelper.EnsureTables(DC);

            var graph = AgentGraph.Parse(input.GraphJson);
            if (graph.Nodes.Count == 0)
                return BadRequest("流程图不能为空");
            if (graph.FindStartNode() == null)
                return BadRequest("流程图缺少开始节点");
            if (string.IsNullOrWhiteSpace(input.Name))
                return BadRequest("流程名称不能为空");

            AgentFlow flow;
            if (input.Id == null || input.Id == Guid.Empty)
            {
                flow = new AgentFlow
                {
                    ID = Guid.NewGuid(),
                    Name = input.Name,
                    Description = input.Description,
                    GraphJson = input.GraphJson,
                    IsSkill = input.IsSkill,
                    IsScheduled = input.IsScheduled,
                    ScheduleCron = input.ScheduleCron,
                    ScheduleInputsJson = input.ScheduleInputsJson,
                    CreateTime = DateTime.Now,
                    CreateBy = Wtm.LoginUserInfo?.ITCode,
                    IsValid = true
                };
                DC.Set<AgentFlow>().Add(flow);
            }
            else
            {
                flow = DC.Set<AgentFlow>().Where(x => x.ID == input.Id && x.IsValid == true).FirstOrDefault();
                if (flow == null)
                    return NotFound();
                flow.Name = input.Name;
                flow.Description = input.Description;
                flow.GraphJson = input.GraphJson;
                flow.IsSkill = input.IsSkill;
                flow.IsScheduled = input.IsScheduled;
                flow.ScheduleCron = input.ScheduleCron;
                flow.ScheduleInputsJson = input.ScheduleInputsJson;
                flow.UpdateTime = DateTime.Now;
                flow.UpdateBy = Wtm.LoginUserInfo?.ITCode;
                DC.UpdateProperty(flow, x => x.Name);
                DC.UpdateProperty(flow, x => x.Description);
                DC.UpdateProperty(flow, x => x.GraphJson);
                DC.UpdateProperty(flow, x => x.IsSkill);
                DC.UpdateProperty(flow, x => x.IsScheduled);
                DC.UpdateProperty(flow, x => x.ScheduleCron);
                DC.UpdateProperty(flow, x => x.ScheduleInputsJson);
                DC.UpdateProperty(flow, x => x.UpdateTime);
                DC.UpdateProperty(flow, x => x.UpdateBy);
            }
            DC.SaveChanges();
            return Ok(flow.ID);
        }

        /// <summary>
        /// 删除流程（软删除，保留运行历史）
        /// </summary>
        /// <param name="id">流程ID</param>
        /// <returns>操作结果</returns>
        [ActionDescription("删除流程")]
        [HttpPost("flows/{id}/delete")]
        public IActionResult DeleteFlow(Guid id)
        {
            AgentTableHelper.EnsureTables(DC);
            var flow = DC.Set<AgentFlow>().Where(x => x.ID == id && x.IsValid == true).FirstOrDefault();
            if (flow == null)
                return NotFound();
            flow.IsValid = false;
            flow.UpdateTime = DateTime.Now;
            flow.UpdateBy = Wtm.LoginUserInfo?.ITCode;
            DC.UpdateProperty(flow, x => x.IsValid);
            DC.UpdateProperty(flow, x => x.UpdateTime);
            DC.UpdateProperty(flow, x => x.UpdateBy);
            DC.SaveChanges();
            return Ok(true);
        }

        /// <summary>
        /// 列出内部API节点可选的系统接口（与Agent对话工具同源），附参数Schema供设计器生成参数表单
        /// </summary>
        /// <returns>接口工具列表</returns>
        [ActionDescription("内部接口列表")]
        [AllRights]
        [NoLog]
        [HttpGet("innertools")]
        public IActionResult InnerTools()
        {
            var tools = AgentApiToolDiscovery.Discover(ConfigInfo?.Agent?.AllowAllTools == true)
                .Where(t => t.Kind == "query")
                .Select(t => new { t.Definition.Name, t.Definition.Description, t.Definition.ParametersJsonSchema, t.Url });
            return Ok(tools);
        }

        /// <summary>
        /// 代码节点语法检查：编译代码（复用AI代码编译器，含黑名单拦截），
        /// 成功时提取方法名、参数（名称+类型）与返回值类型
        /// </summary>
        /// <param name="input">代码内容</param>
        /// <returns>编译结果与提取的方法签名</returns>
        [ActionDescription("代码语法检查")]
        [NoLog]
        [HttpPost("flows/code/check")]
        public IActionResult CheckCode([FromBody] AgentCodeCheckInput input)
        {
            var code = input?.Code?.Trim();
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("代码不能为空");
            //提取方法名：public static XXX 名字(
            var m = System.Text.RegularExpressions.Regex.Match(code, @"(?:public\s+)?(?:static\s+)?[\w<>,\?\[\]\s]+\s+(\w+)\s*\(", System.Text.RegularExpressions.RegexOptions.Compiled);
            if (m.Success == false)
                return Ok(new { Success = false, Diagnostics = new[] { "未找到函数定义，请写一个完整的C#函数，如 public double Calc(double x) { ... }" } });
            var methodName = m.Groups[1].Value;

            var compile = AiCodeCompiler.Compile(code, methodName, null);
            if (compile.Success == false)
                return Ok(new { Success = false, compile.Diagnostics, compile.BlacklistHit });

            var method = compile.Method;
            return Ok(new
            {
                Success = true,
                MethodName = methodName,
                ReturnType = TypeName(method.ReturnType),
                Params = method.GetParameters().Select(p => new { p.Name, Type = TypeName(p.ParameterType) }),
            });
        }

        /// <summary>
        /// 代码节点AI生成：按用户描述生成单个C#函数
        /// </summary>
        /// <param name="input">功能描述</param>
        /// <returns>生成的代码</returns>
        [ActionDescription("代码AI生成")]
        [HttpPost("flows/code/generate")]
        public async Task<IActionResult> GenerateCode([FromBody] AgentCodeCheckInput input)
        {
            if (string.IsNullOrWhiteSpace(input?.Prompt))
                return BadRequest("请描述要生成的函数功能");
            var llm = Wtm?.ServiceProvider?.GetService(typeof(LlmService)) as LlmService
                ?? new LlmService(ConfigInfo?.Llm ?? new LlmOption());
            var system = "你是C#代码生成器。只输出一个完整的C#静态方法定义，不要类和命名空间包装，不要解释。" +
                "要求：方法为 public static，参数和返回值使用简单类型（double/int/string/bool/DateTime），" +
                "只能使用 System 基础类型和 System.Linq，不要访问数据库/网络/文件。" +
                "输出格式：直接给出方法代码，用```csharp包裹。";
            var code = await llm.ChatAsync(system, input.Prompt);
            //剥离markdown围栏
            code = System.Text.RegularExpressions.Regex.Replace(code?.Trim() ?? "", "^```[a-zA-Z#]*\\s*|\\s*```$", "");
            return Ok(code.Trim());
        }

        private static string TypeName(Type t)
        {
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            var name = nt.Name;
            var map = new Dictionary<string, string> {
                ["Int32"] = "int", ["Int64"] = "long", ["Double"] = "double", ["Single"] = "float",
                ["Boolean"] = "bool", ["String"] = "string", ["Decimal"] = "decimal", ["DateTime"] = "DateTime",
            };
            return map.TryGetValue(name, out var simple) ? simple + (Nullable.GetUnderlyingType(t) != null ? "?" : "") : name;
        }

        /// <summary>
        /// 执行流程：解析流程图、按输入变量运行引擎，
        /// 结果（含各节点日志）持久化为AgentRun后返回
        /// </summary>
        /// <param name="id">流程ID</param>
        /// <param name="input">输入变量</param>
        /// <returns>运行结果（runId、状态、输出、节点日志、总耗时）</returns>
        [ActionDescription("执行流程")]
        [HttpPost("flows/{id}/run")]
        public async Task<IActionResult> RunFlow(Guid id, [FromBody] AgentRunInput input)
        {
            AgentTableHelper.EnsureTables(DC);
            var flow = DC.Set<AgentFlow>().Where(x => x.ID == id && x.IsValid == true).FirstOrDefault();
            if (flow == null)
                return NotFound();

            var graph = AgentGraph.Parse(flow.GraphJson);
            var runtime = AgentRuntimeServices.FromWtm(Wtm);
            //注入内部API调用能力：以当前请求身份HTTP自调用（继承权限校验与数据权限）
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
            var authorization = Request.Headers.Authorization.ToString();
            var allowWrite = ConfigInfo?.Agent?.AllowAllTools == true;
            runtime.InternalApiCall = (toolName, args) =>
            {
                var tool = AgentApiToolDiscovery.FindTool(toolName, allowWrite);
                if (tool == null)
                    throw new InvalidOperationException($"未找到内部接口 {toolName}");
                return AgentApiToolDiscovery.InvokeAsync(tool, args, baseUrl, authorization);
            };
            var result = await AgentExecutor.RunAsync(graph, input?.Inputs, runtime);

            //运行记录与业务操作分开保存，记录保存失败不影响结果返回
            try
            {
                var run = new AgentRun
                {
                    ID = Guid.NewGuid(),
                    FlowId = flow.ID,
                    FlowName = flow.Name,
                    Status = result.Status,
                    InputJson = JsonSerializer.Serialize(input?.Inputs ?? new Dictionary<string, string>()),
                    OutputJson = result.Output,
                    LogsJson = JsonSerializer.Serialize(result.NodeLogs),
                    Error = result.Error,
                    DurationMs = result.TotalMs,
                    CreateTime = DateTime.Now,
                    CreateBy = Wtm.LoginUserInfo?.ITCode,
                    IsValid = true
                };
                DC.Set<AgentRun>().Add(run);
                DC.SaveChanges();
                return Ok(new { RunId = run.ID, result.Status, result.Output, result.NodeLogs, result.Error, result.TotalMs });
            }
            catch
            {
                return Ok(new { RunId = (Guid?)null, result.Status, result.Output, result.NodeLogs, result.Error, result.TotalMs });
            }
        }

        /// <summary>
        /// 分页查询流程的运行历史（不含节点日志，按时间倒序）
        /// </summary>
        /// <param name="id">流程ID</param>
        /// <param name="page">页码，从1开始</param>
        /// <param name="limit">每页条数</param>
        /// <returns>运行记录列表与总数</returns>
        [ActionDescription("查询运行历史")]
        [NoLog]
        [HttpGet("flows/{id}/runs")]
        public IActionResult GetRuns(Guid id, int page = 1, int limit = 20)
        {
            AgentTableHelper.EnsureTables(DC);
            var query = DC.Set<AgentRun>().Where(x => x.FlowId == id && x.IsValid == true);
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreateTime)
                .Skip((page - 1) * limit).Take(limit)
                .Select(x => new
                {
                    x.ID,
                    x.FlowName,
                    x.Status,
                    x.DurationMs,
                    x.CreateTime,
                    x.CreateBy,
                    x.Error,
                    OutputPreview = x.OutputJson != null && x.OutputJson.Length > 100 ? x.OutputJson.Substring(0, 100) + "..." : x.OutputJson
                })
                .ToList();
            return Ok(new { Data = data, Count = count });
        }

        /// <summary>
        /// 获取单次运行详情（含输入、输出与节点日志）
        /// </summary>
        /// <param name="runId">运行记录ID</param>
        /// <returns>运行详情，不存在返回404</returns>
        [ActionDescription("获取运行详情")]
        [NoLog]
        [HttpGet("runs/{runId}")]
        public IActionResult GetRun(Guid runId)
        {
            AgentTableHelper.EnsureTables(DC);
            var run = DC.Set<AgentRun>().Where(x => x.ID == runId).FirstOrDefault();
            if (run == null)
                return NotFound();
            List<AgentNodeLog> logs = new List<AgentNodeLog>();
            try
            {
                if (!string.IsNullOrEmpty(run.LogsJson))
                    logs = JsonSerializer.Deserialize<List<AgentNodeLog>>(run.LogsJson) ?? new List<AgentNodeLog>();
            }
            catch { }
            return Ok(new
            {
                run.ID,
                run.FlowId,
                run.FlowName,
                run.Status,
                run.DurationMs,
                run.CreateTime,
                run.CreateBy,
                run.InputJson,
                Output = run.OutputJson,
                run.Error,
                NodeLogs = logs
            });
        }

        /// <summary>
        /// 获取知识库文档列表（已完成向量化），供知识库节点选择文档
        /// </summary>
        /// <returns>文档列表（ID、标题、状态、分块数）</returns>
        [ActionDescription("知识库文档列表")]
        [AllRights]
        [NoLog]
        [HttpGet("kbdocs")]
        public IActionResult GetKbDocs()
        {
            var docs = DC.Set<KnowledgeDoc>()
                .Select(x => new { x.ID, x.Title, x.Status, x.ChunkCount })
                .ToList();
            return Ok(docs);
        }

        /// <summary>
        /// 获取系统默认大模型信息，供LLM节点展示默认值
        /// </summary>
        /// <returns>模型名称与启用状态</returns>
        [ActionDescription("默认模型信息")]
        [AllRights]
        [NoLog]
        [HttpGet("llminfo")]
        public IActionResult GetLlmInfo()
        {
            var llm = ConfigInfo?.Llm;
            return Ok(new { Model = llm?.Model, Enabled = llm != null && !string.IsNullOrEmpty(llm.ChatApiUrl) });
        }
    }

    /// <summary>
    /// 保存流程的请求体
    /// </summary>
    public class AgentFlowInput
    {
        /// <summary>流程ID，为空表示新建</summary>
        public Guid? Id { get; set; }
        /// <summary>流程名称</summary>
        public string Name { get; set; }
        /// <summary>流程说明</summary>
        public string Description { get; set; }
        /// <summary>流程图JSON</summary>
        public string GraphJson { get; set; }
        /// <summary>是否发布为AI助手技能（对话中模型可自动调用）</summary>
        public bool IsSkill { get; set; }
        /// <summary>是否定时运行</summary>
        public bool IsScheduled { get; set; }
        /// <summary>定时运行的Quartz Cron表达式，为空默认每小时</summary>
        public string ScheduleCron { get; set; }
        /// <summary>定时运行时的默认输入变量JSON</summary>
        public string ScheduleInputsJson { get; set; }
    }

    /// <summary>
    /// 代码节点的检查/生成请求体
    /// </summary>
    public class AgentCodeCheckInput
    {
        /// <summary>代码内容（语法检查用）。加JsonStringConverter防止框架LTGT过滤剥掉比较运算符</summary>
        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringConverter))]
        public string Code { get; set; }
        /// <summary>功能描述（AI生成用）</summary>
        public string Prompt { get; set; }
    }

    /// <summary>
    /// 执行流程的请求体
    /// </summary>
    public class AgentRunInput
    {
        /// <summary>输入变量（与开始节点声明的变量对应）</summary>
        public Dictionary<string, string> Inputs { get; set; }
    }

    /// <summary>
    /// Agent对话模式的请求体
    /// </summary>
    public class AgentChatInput
    {
        /// <summary>历史消息（role: user/assistant, content），首次为空</summary>
        public List<Dictionary<string, string>> History { get; set; }
        /// <summary>本轮用户提问</summary>
        public string Message { get; set; }
        /// <summary>知识库检索返回分块数上限，空用默认值</summary>
        public int? TopK { get; set; }
        /// <summary>知识库检索相似度阈值，空用默认值</summary>
        public double? Threshold { get; set; }
        /// <summary>自定义系统提示词，空用后端默认的平衡提示词</summary>
        public string SystemPrompt { get; set; }
    }
}
