using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Agent;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Support;

namespace WalkingTec.Mvvm.AgentApi
{
    /// <summary>
    /// Agent对话API控制器（与Agent编排的AgentApiController分开，各自作为独立模块出现在菜单管理中）。
    /// 提供Agent对话（工具调用）与工具列表诊断接口；
    /// Chat接口受菜单权限控制：在菜单管理中为角色勾选“Agent对话”接口动作后可用，
    /// 前端AI助手控件按该权限决定是否显示
    /// </summary>
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/agent")]
    [ActionDescription("Agent对话")]
    public class AgentChatApiController : BaseApiController
    {
        /// <summary>
        /// 列出Agent对话模式注册的全部工具（知识库检索+自动发现的接口工具），便于排查工具注册情况
        /// </summary>
        /// <returns>工具范围与工具名称、描述、对应接口URL</returns>
        [ActionDescription("对话工具列表")]
        [HttpGet("chat/tools")]
        public IActionResult ChatTools()
        {
            var scope = ConfigInfo?.Agent?.ToolScope ?? "Query";
            var apiTools = AgentApiToolDiscovery.Discover(ConfigInfo?.Agent?.AllowAllTools == true)
                .Select(t => new { t.Definition.Name, Kind = t.Kind, t.Definition.Description, t.Url, Param = t.ParamType?.Name });
            return Ok(new { ToolScope = scope, ApiTools = apiTools, KnowledgeTool = "knowledge_search" });
        }

        /// <summary>
        /// Agent对话模式：自动把系统内的接口注册为工具（范围由appsettings的Agent.ToolScope控制：
        /// Query=仅查询，All=含增删改），知识库语义检索同样以工具形式提供，
        /// 大模型按需组合调用后综合回答；返回最终回答与全部交互轨迹
        /// </summary>
        /// <param name="input">对话输入（历史消息、本轮提问、系统提示词）</param>
        /// <returns>最终回答与交互轨迹</returns>
        [ActionDescription("Agent对话")]
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AgentChatInput input)
        {
            if (string.IsNullOrWhiteSpace(input?.Message))
                return BadRequest("提问内容不能为空");

            //以当前请求的身份HTTP自调用接口，工具执行天然继承权限校验与数据权限
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
            var authorization = Request.Headers.Authorization.ToString();
            var apiTools = AgentApiToolDiscovery.Discover(ConfigInfo?.Agent?.AllowAllTools == true)
                .Select(t => new AgentTool
                {
                    Definition = t.Definition,
                    Executor = (args, ct) => AgentApiToolDiscovery.InvokeAsync(t, args, baseUrl, authorization)
                })
                .ToList();
            //Excel生成工具（支持服务端按查询工具取数，数据不经过大模型）
            apiTools.Add(AgentExcelTool.Build(Wtm, baseUrl, authorization));

            //内部API调用委托：流程技能含内部API节点时以当前用户身份HTTP自调用
            var allowWrite0 = ConfigInfo?.Agent?.AllowAllTools == true;
            Func<string, System.Text.Json.JsonElement, Task<object>> internalApiCall = (toolName, args) =>
            {
                var tool = AgentApiToolDiscovery.FindTool(toolName, allowWrite0)
                    ?? throw new InvalidOperationException($"未找到内部接口 {toolName}");
                return AgentApiToolDiscovery.InvokeAsync(tool, args, baseUrl, authorization);
            };

            var result = await AgentChatService.ChatAsync(
                Wtm, input.History, input.Message, apiTools, input.SystemPrompt, input.TopK, input.Threshold, internalApiCall);

            //对话历史保存到服务端（框架登出会清空浏览器本地存储，历史以服务端为准）；
            //保存失败不影响对话返回
            try
            {
                AgentTableHelper.EnsureTables(DC);
                var user = Wtm.LoginUserInfo?.ITCode ?? "guest";
                var now = DateTime.Now;
                DC.Set<AgentChatMessage>().AddRange(new[]
                {
                    new AgentChatMessage { ID = Guid.NewGuid(), UserCode = user, Role = "user", Content = input.Message, CreateTime = now },
                    new AgentChatMessage { ID = Guid.NewGuid(), UserCode = user, Role = "assistant", Content = result.Answer, CreateTime = now.AddSeconds(1) },
                });
                DC.SaveChanges();
            }
            catch { }

            return Ok(result);
        }

        /// <summary>
        /// 查询当前用户的对话历史（按时间正序的最近N条），供AI助手组件在本地缓存被清后恢复。
        /// 仅读写本人数据（UserCode过滤），登录即可访问（AllRights）；
        /// 面板每次打开都会调用，属高频查询，不记录操作日志（NoLog）
        /// </summary>
        /// <param name="count">返回的最大条数，默认60</param>
        /// <returns>消息列表</returns>
        [AllRights]
        [NoLog]
        [HttpGet("chat/history")]
        public IActionResult ChatHistory(int count = 60)
        {
            AgentTableHelper.EnsureTables(DC);
            var user = Wtm.LoginUserInfo?.ITCode ?? "guest";
            var list = DC.Set<AgentChatMessage>().AsNoTracking()
                .Where(x => x.UserCode == user)
                .OrderByDescending(x => x.CreateTime)
                .Take(Math.Clamp(count, 1, 200))
                .OrderBy(x => x.CreateTime)
                .Select(x => new { x.Role, x.Content, Time = x.CreateTime })
                .ToList();
            return Ok(list);
        }

        /// <summary>
        /// 清空当前用户的服务端对话历史。仅操作本人数据，登录即可访问（AllRights），不记录操作日志（NoLog）
        /// </summary>
        /// <returns>操作结果</returns>
        [AllRights]
        [NoLog]
        [HttpDelete("chat/history")]
        public IActionResult ClearChatHistory()
        {
            AgentTableHelper.EnsureTables(DC);
            var user = Wtm.LoginUserInfo?.ITCode ?? "guest";
            var list = DC.Set<AgentChatMessage>().Where(x => x.UserCode == user).ToList();
            DC.Set<AgentChatMessage>().RemoveRange(list);
            DC.SaveChanges();
            return Ok(true);
        }
    }
}
