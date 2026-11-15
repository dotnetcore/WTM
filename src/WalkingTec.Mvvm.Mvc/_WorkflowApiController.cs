using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Elsa;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Open.Linq.AsyncExtensions;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Core.WorkFlow;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 审批流 API 控制器，提供工作流相关的人员/小组/角色查询、审批时间线、流程实例与我的审批记录查询等接口
    /// </summary>
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("_[controller]")]
    [ActionDescription("_Admin.WorkflowApi")]
    [AllRights]
    public class WorkflowApiController : BaseApiController
    {

        /// <summary>
        /// 根据用户 ITCode 数组查询工作流可选择的用户列表
        /// </summary>
        /// <param name="itcode">用户 ITCode 数组</param>
        /// <returns>用户名称与 ITCode 组成的键值对列表</returns>
        [HttpGet("[action]")]
        [NoLog]
        [Public]
        public IActionResult GetWorkflowUsers([FromQuery]string[] itcode)
        {



                // 分布式部署时，当前节点非主节点，将请求转发到主节点统一处理
                if (ConfigInfo.HasMainHost)
                {
                    return Request.RedirectCall(Wtm, "/_WorkflowApi/GetWorkflowUsers").Result;
                }
                // 忽略全局查询过滤器，改为按当前租户手动过滤，查询指定 ITCode 的用户
                var tenant = Wtm.LoginUserInfo?.CurrentTenant;
                var rv = Wtm.BaseUserQuery.IgnoreQueryFilters().CheckContain(itcode.ToList(), x => x.ITCode).Where(x => x.TenantCode == tenant)
                    .Select(x => new { x.ITCode, x.Name })
                    .OrderBy(x => x.ITCode)
                    .ToList().ToListItems(x => x.Name, x => x.ITCode);

                return Ok(rv);

        }

        /// <summary>
        /// 根据 ID 数组查询工作流可选择的小组列表
        /// </summary>
        /// <param name="ids">小组 ID 数组</param>
        /// <returns>小组名称与 ID 组成的键值对列表</returns>
        [HttpGet("[action]")]
        [Public]
        [NoLog]
        public IActionResult GetWorkflowGroups([FromQuery] string[] ids)
        {
                // 分布式部署时，当前节点非主节点，将请求转发到主节点统一处理
                if (ConfigInfo.HasMainHost)
                {
                    return Request.RedirectCall(Wtm, "/_WorkflowApi/GetWorkflowGroups").Result;
                }
                var tenant = Wtm.LoginUserInfo?.CurrentTenant;
                var rv = Wtm.DC.Set<FrameworkGroup>().CheckIDs(ids.ToList())
                    .Select(x => new { x.ID, x.GroupName })
                    .OrderBy(x => x.GroupName)
                    .ToList().ToListItems(x => x.GroupName, x => x.ID);

                return Ok(rv);
        }

        /// <summary>
        /// 根据 ID 数组查询指定小组的管理员列表
        /// </summary>
        /// <param name="ids">小组 ID 数组</param>
        /// <returns>小组管理员名称与小组 ID 组成的键值对列表</returns>
        [HttpGet("[action]")]
        [Public]
        [NoLog]
        public IActionResult GetWorkflowGroupManagers([FromQuery] string[] ids)
        {
            // 分布式部署时，当前节点非主节点，将请求转发到主节点统一处理
            if (ConfigInfo.HasMainHost)
            {
                return Request.RedirectCall(Wtm, "/_WorkflowApi/GetWorkflowGroupManagers").Result;
            }
            var tenant = Wtm.LoginUserInfo?.CurrentTenant;
            var rv = Wtm.DC.Set<FrameworkGroup>().CheckIDs(ids.ToList())
                .Select(x => new { x.ID, x.Manager })
                .OrderBy(x => x.Manager)
                .ToList().ToListItems(x => x.Manager, x => x.ID);

            return Ok(rv);
        }

        /// <summary>
        /// 查询指定用户所属各小组的管理员列表
        /// </summary>
        /// <param name="itcode">用户 ITCode</param>
        /// <returns>该用户所属各小组的管理员名称列表</returns>
        [HttpGet("[action]")]
        [Public]
        [NoLog]
        public IActionResult GetWorkflowMyGroupManagers([FromQuery] string itcode)
        {
            // 分布式部署时，当前节点非主节点，将请求转发到主节点统一处理
            if (ConfigInfo.HasMainHost)
            {
                return Request.RedirectCall(Wtm, "/_WorkflowApi/GetWorkflowMyGroupManagers").Result;
            }
            var tenant = Wtm.LoginUserInfo?.CurrentTenant;
            var rv = Wtm.DC.Set<FrameworkUserGroup>().Where(x=>x.UserCode == itcode)
                .Join(DC.Set<FrameworkGroup>(), x => x.GroupCode, y => y.GroupCode, (x, y) => y.Manager)
                .ToList().ToListItems(x => x, x => x);

            return Ok(rv);
        }

        /// <summary>
        /// 根据 ID 数组查询工作流可选择的角色列表
        /// </summary>
        /// <param name="ids">角色 ID 数组</param>
        /// <returns>角色名称与 ID 组成的键值对列表</returns>
        [HttpGet("[action]")]
        [Public]
        [NoLog]
        public IActionResult GetWorkflowRoles([FromQuery] string[] ids)
        {


                // 分布式部署时，当前节点非主节点，将请求转发到主节点统一处理
                if (ConfigInfo.HasMainHost)
                {
                    return Request.RedirectCall(Wtm, "/_WorkflowApi/GetWorkflowRoles").Result;
                }
                var tenant = Wtm.LoginUserInfo?.CurrentTenant;
                var rv = Wtm.DC.Set<FrameworkRole>().CheckIDs(ids.ToList())
                    .Select(x => new { x.ID, x.RoleName })
                    .OrderBy(x => x.RoleName)
                    .ToList().ToListItems(x => x.RoleName, x => x.ID);

                return Ok(rv);

        }


        /// <summary>
        /// 获取指定业务单据的审批时间线，包含流程发起、各审批节点的动作/审批人/意见以及流程结束时间
        /// </summary>
        /// <param name="flowname">流程名称</param>
        /// <param name="entitytype">业务实体类型(模型类型的全名)</param>
        /// <param name="entityid">业务实体的 ID</param>
        /// <returns>按时间排列的审批时间线列表</returns>
        [HttpGet("[action]")]
        [NoLog]
        public async Task<IActionResult> GetTimeLine(string flowname,string entitytype,string entityid)
        {
            List<ApproveTimeLine> all = [];
            // 获取 Elsa 工作流实例存储与执行日志存储服务
            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var log = Wtm.ServiceProvider.GetRequiredService<IWorkflowExecutionLogStore>();
            // 按流程名称、实体类型和实体 ID 查询关联的全部工作流实例(同一单据可能多次发起流程)
            var workflowIds = DC.Set<Elsa_WorkflowInstance>().CheckEqual(flowname, x => x.Name).CheckEqual(entitytype, x => x.ContextType).CheckEqual(entityid, x => x.ContextId)
                .OrderBy(x=>x.CreatedAt).Select(x => x.ID).ToList();
            // 逐个实例解析审批执行日志，组装时间线
            foreach (var workflowId in workflowIds)
            {
                var instance = await lp.FindByIdAsync(workflowId);
                if (instance != null)
                {
                    // 按工作流实例 ID 过滤执行日志，并按时间戳排序
                    var specification = new Elsa.Persistence.Specifications.WorkflowExecutionLogRecords.WorkflowInstanceIdSpecification(workflowId);
                    var orderBy = OrderBySpecification.OrderBy<WorkflowExecutionLogRecord>(x => x.Timestamp);
                    var records = await log.FindManyAsync(specification, orderBy).ToList();
                    // 筛选审批活动(WtmApproveActivity)的日志记录："Executed" 事件表示进入等待审批，其余事件名对应该次审批的结论
                    var rv = records.Where(x => x.ActivityType == nameof(WtmApproveActivity) && x.EventName != "Executing" && x.EventName != "Resuming" && x.EventName != "Suspended")
                        .Select(x =>
                        new ApproveTimeLine
                        {
                            Id = x.ActivityId,
                            Time = x.Timestamp.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToString("yyyy-MM-dd HH:mm:ss", null),
                            Action = x.EventName == "Executed" ? "等待审批" : x.Data.ContainsKey("Outcomes") ? x.Data["Outcomes"].Values<string>().FirstOrDefault() : "",
                            Remark = "",
                            Approvers = "",
                            Approved = ""
                        }
                    ).ToList();
                    // 剔除没有审批动作的记录
                    rv = rv.Where(x => string.IsNullOrEmpty(x.Action) == false).ToList();
                    // 从流程实例的活动数据中补充审批人、审批意见等信息
                    foreach (var record in rv)
                    {
                        var ad = instance.ActivityData[record.Id];
                        object approved, remark, approvers;
                        ad.TryGetValue(nameof(WtmApproveActivity.ApprovedBy), out approved);
                        ad.TryGetValue(nameof(WtmApproveActivity.Remark), out remark);
                        ad.TryGetValue(nameof(WtmApproveActivity.ApproveUsersFullText), out approvers);
                        record.Approved = (approved as string) ?? "";
                        record.Approvers = (approvers as List<string>)?.ToSepratedString() ?? "";
                        if (record.Action != "等待审批")
                        {
                            record.Remark = (remark as string) ?? "";
                        }
                    }
                    // 在时间线头部插入流程发起记录(提交人及发起时间)
                    ApproveTimeLine first = new ApproveTimeLine
                    {
                        Action = "_start",
                        Approved = instance.Variables.Get("Submitter")?.ToString() ?? "",
                        Approvers = "",
                        Id = "",
                        Remark = "",
                        Time = instance.CreatedAt.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToString("yyyy-MM-dd HH:mm:ss", null)
                    };
                    rv.Insert(0, first);
                    // 流程已结束时，在时间线尾部追加流程结束记录
                    if (instance.FinishedAt != null)
                    {
                        ApproveTimeLine last = new ApproveTimeLine
                        {
                            Action = "_finish",
                            Approved = "",
                            Approvers = "",
                            Id = "",
                            Remark = "",
                            Time = instance.FinishedAt.Value.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToString("yyyy-MM-dd HH:mm:ss", null)
                        };
                        rv.Add(last);
                    }
                    all.AddRange(rv);
                }
            }
            return Ok(all);
        }

        /// <summary>
        /// 获取指定业务单据当前关联的工作流实例(Elsa WorkflowInstance)，用于前端展示流程运行进度
        /// </summary>
        /// <param name="flowname">流程名称</param>
        /// <param name="entitytype">业务实体类型</param>
        /// <param name="entityid">业务实体的 ID</param>
        /// <returns>工作流实例对象；不存在时返回 null</returns>
        [HttpGet("[action]")]
        [NoLog]
        public async Task<IActionResult> GetWorkflow(string flowname, string entitytype, string entityid)
        {
            var workflowId = DC.Set<Elsa_WorkflowInstance>().CheckEqual(flowname, x => x.Name).CheckEqual(entitytype, x => x.ContextType).CheckEqual(entityid, x => x.ContextId)
                .Select(x => x.ID).FirstOrDefault();
            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var instance = await lp.FindByIdAsync(workflowId);
            return Ok(instance);
        }

        /// <summary>
        /// 分页查询当前用户名下(按用户、角色或小组分配)的审批记录
        /// </summary>
        /// <param name="flowname">流程名称</param>
        /// <param name="entitytype">业务实体类型</param>
        /// <param name="entityid">业务实体的 ID</param>
        /// <param name="tag">审批状态标记(用于区分待办、已办等)</param>
        /// <param name="page">页码，从 1 开始，默认为 1</param>
        /// <param name="take">每页记录数，默认为 20</param>
        /// <returns>按发起时间倒序排列的审批记录分页列表</returns>
        [HttpGet("[action]")]
        [NoLog]
        public async Task<IActionResult> GetMyApprove(string flowname, string entitytype, string entityid,string tag,int page=1,int take=20)
        {
            // 将当前用户的角色/小组 ID 加上 r: / g: 前缀，与 FrameworkWorkflow.UserCode 的存储格式对齐，从而匹配按角色或小组分配的审批任务
            var roleids = Wtm.LoginUserInfo.Roles.Select(x => "r:" + x.ID).ToList();
            var groupids = Wtm.LoginUserInfo.Groups.Select(x => "g:" + x.ID).ToList();
            var rv = await DC.Set<FrameworkWorkflow>()
                .Where(x => x.UserCode == Wtm.LoginUserInfo.ITCode
                    || roleids.Contains(x.UserCode)
                    || groupids.Contains(x.UserCode))
                .Where(x=>x.TenantCode == Wtm.LoginUserInfo.CurrentTenant)
                .CheckEqual(flowname, x => x.WorkflowName)
                .CheckEqual(entitytype, x => x.ModelType)
                .CheckEqual(entityid, x => x.ModelID)
                .CheckEqual(tag, x => x.Tag)
                .OrderByDescending(x=>x.StartTime)
                .Skip((page-1)*take).Take(take)
                .ToListAsync();
            return Ok(rv);
        }

    }
}

