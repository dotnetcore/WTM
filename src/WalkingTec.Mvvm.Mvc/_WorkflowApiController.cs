using Elsa;
using Elsa.Activities.Workflows.Workflow;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Server.Api.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetBox.Extensions;
using NodaTime;
using Open.Linq.AsyncExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Core.WorkFlow;

namespace WalkingTec.Mvvm.Mvc
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("_[controller]")]
    [ActionDescription("_Admin.WorkflowApi")]
    [AllRights]
    public class WorkflowApiController : BaseApiController
    {

        [HttpGet("[action]")]
        [Public]
        public IActionResult GetWorkflowUsers([FromQuery]string[] itcode)
        {
            if (ConfigInfo.HasMainHost)
            {
                return Request.RedirectCall(Wtm, "/_WorkflowApi/GetWorkflowUsers").Result;
            }
            var tenant = Wtm.LoginUserInfo?.CurrentTenant;
            var rv = Wtm.BaseUserQuery.IgnoreQueryFilters().CheckContain(itcode.ToList(), x => x.ITCode).Where(x => x.TenantCode == tenant)
                .Select(x => new { x.ITCode, x.Name })
                .OrderBy(x => x.ITCode)
                .ToList().ToListItems(x => x.Name, x => x.ITCode);

            return Ok(rv);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTimeLine(string flowname,string entitytype,string entityid)
        {

            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var log = Wtm.ServiceProvider.GetRequiredService<IWorkflowExecutionLogStore>();
            var workflowId = DC.Set<Elsa_WorkflowInstance>().CheckEqual(flowname, x => x.Name).CheckEqual(entitytype, x => x.ContextType).CheckEqual(entityid, x => x.ContextId)
                .Select(x => x.ID).FirstOrDefault();
            var instance = await lp.FindByIdAsync(workflowId);
            if (instance != null)
            {
                var specification = new Elsa.Persistence.Specifications.WorkflowExecutionLogRecords.WorkflowInstanceIdSpecification(workflowId);
                var orderBy = OrderBySpecification.OrderBy<WorkflowExecutionLogRecord>(x => x.Timestamp);
                var records = await log.FindManyAsync(specification, orderBy).ToList();
                var rv = records.Where(x => x.ActivityType == nameof(WtmApproveActivity) && x.EventName != "Executing" && x.EventName != "Resuming" && x.EventName != "Suspended")
                    .Select(x =>
                    new ApproveTimeLine
                    {
                        Id = x.ActivityId,
                        Time = x.Timestamp.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToString("yyyy-MM-dd HH:mm:ss", null),
                        Action = x.EventName == "Executed" ? "等待审批" : x.Data["Outcomes"].Values<string>().FirstOrDefault(),
                        Remark = "",
                        Approvers = "",
                        Approved = ""
                    }
                ).ToList();
                rv = rv.Where(x => string.IsNullOrEmpty(x.Action) == false).ToList();
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
                ApproveTimeLine first = new ApproveTimeLine
                {
                    Action = "_start",
                    Approved = instance.Variables.Get("Submitter")?.ToString()?? "",
                    Approvers = "",
                    Id = "",
                    Remark = "",
                    Time = instance.CreatedAt.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToString("yyyy-MM-dd HH:mm:ss", null)
                };
                rv.Insert(0, first);
                if(instance.FinishedAt != null)
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
                return Ok(rv);
            }
            return Ok(new List<ApproveTimeLine>());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetWorkflow(string flowname, string entitytype, string entityid)
        {
            var workflowId = DC.Set<Elsa_WorkflowInstance>().CheckEqual(flowname, x => x.Name).CheckEqual(entitytype, x => x.ContextType).CheckEqual(entityid, x => x.ContextId)
                .Select(x => x.ID).FirstOrDefault();
            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var instance = await lp.FindByIdAsync(workflowId);
            return Ok(instance);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMyApprove(string flowname, string entitytype, string entityid,string tag,int page=1,int take=20)
        {
            var rv = await DC.Set<FrameworkWorkflow>()
                .Where(x=>x.UserCode == Wtm.LoginUserInfo.ITCode)
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
