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
    [AllRights]
    [AuthorizeJwtWithCookie]
    public class _WorkflowController : BaseController
    {
        [ActionDescription("流程管理")]
        public IActionResult Inner()
        {
            return View();

        }

        //public async Task<IActionResult> StartWorkflowAsync(string workflowName, string contextId)
        //{
        //    try
        //    {
        //        var workflowId = DC.Set<Elsa_WorkflowDefinition>().Where(x => x.Name == workflowName).Select(x => x.DefinitionId).FirstOrDefault();
        //        var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowLaunchpad>();
        //        var workflow = await lp.FindStartableWorkflowAsync(workflowId, contextId: contextId,tenantId:Wtm.LoginUserInfo.CurrentTenant);
        //        if (workflow != null)
        //        {
        //            workflow.WorkflowInstance.Variables.Set("Submitter", Wtm.LoginUserInfo.ITCode);
        //            var rv = await lp.ExecuteStartableWorkflowAsync(workflow);
        //            return Ok(rv);
        //        }
        //    }
        //    catch { }
        //    return Ok(new RunWorkflowResult(null, null, null, false));
        //}

        //[HttpPost]
        //public async Task<IActionResult> ContinueWorkflowAsync(string workflowName, string actionName, string remark,string tag=null,string entityId=null)
        //{
        //    try
        //    {
        //        var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowLaunchpad>();
        //        var context = new WorkflowsQuery(nameof(WtmApproveActivity), new WtmApproveBookmark(Wtm.LoginUserInfo.ITCode, workflowName,tag), null,null,entityId,Wtm.LoginUserInfo.CurrentTenant);
        //        if (context != null)
        //        {
        //            //不直接使用Wtm.LoginUserInfo，否则elsa会把所有信息序列化保存到WorkflowInstances表中
        //            LoginUserInfo li = new LoginUserInfo();
        //            li.ITCode = Wtm.LoginUserInfo.ITCode;
        //            li.Name = Wtm.LoginUserInfo.Name;
        //            li.UserId = Wtm.LoginUserInfo.UserId;
        //            li.PhotoId = Wtm.LoginUserInfo.PhotoId;
        //            li.Groups = Wtm.LoginUserInfo.Groups;
        //            li.Roles = Wtm.LoginUserInfo.Roles;
        //            li.TenantCode = Wtm.LoginUserInfo.CurrentTenant;
        //            var collectedWorkflows = await lp.CollectAndExecuteWorkflowsAsync(context, new WorkflowInput { Input = new WtmApproveInput { Action = actionName, CurrentUser = li, Remark = remark } });
        //            return Ok(collectedWorkflows?.FirstOrDefault());
        //        }
        //    }
        //    catch { }
        //    return Ok(null);
        //}


        public IActionResult GetWorkflowUsers(string[] itcode)
        {
            if (ConfigInfo.HasMainHost)
            {
                return Request.RedirectCall(Wtm, "/_Workflow/GetWorkflowUsers").Result;
            }
            string sql = "select itcode,name from frameworkusers $where$ order by itcode";
            string where = "";
            if(itcode != null && itcode.Length > 0)
            {
                where += " where itcode in (";
                foreach (var item in itcode)
                {
                    where += $"'{item}',";
                }
                where += "'placeholder')";
            }
            sql = sql.Replace("$where$", where);
            var table = DC.RunSQL(sql);
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            foreach (DataRow row in table.Rows)
            {
                rv.Add(new ComboSelectListItem
                {
                    Value = row[0]?.ToString(),
                    Text = row[1].ToString()
                });
            }
            return Ok(rv);
        }

        public async Task<IActionResult> GetTimeLine(string flowname,string entitytype,string entityid)
        {

            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var log = Wtm.ServiceProvider.GetRequiredService<IWorkflowExecutionLogStore>();
            var workflowId = DC.Set<Elsa_WorkflowInstance>().CheckEqual(flowname, x => x.Name).CheckEqual(entitytype, x => x.ContextType).CheckEqual(entityid, x => x.ContextId)
                .Select(x => x.ID).FirstOrDefault();
            var instance = await lp.FindByIdAsync(workflowId);
            var specification = new Elsa.Persistence.Specifications.WorkflowExecutionLogRecords.WorkflowInstanceIdSpecification(workflowId);
            var orderBy = OrderBySpecification.OrderBy<WorkflowExecutionLogRecord>(x => x.Timestamp);
            var records = await log.FindManyAsync(specification, orderBy).ToList();
            var test = new { a = 1 };
            var rv =records.Where(x => x.ActivityType == nameof(WtmApproveActivity) && x.EventName != "Executing" && x.EventName != "Resuming")
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
            foreach (var record in rv)
            {                
                var ad = instance.ActivityData[record.Id];
                object approved,remark,approvers;
                ad.TryGetValue(nameof(WtmApproveActivity.ApprovedBy), out approved);
                ad.TryGetValue(nameof(WtmApproveActivity.Remark), out remark);
                ad.TryGetValue(nameof(WtmApproveActivity.ApproveUsersFullText), out approvers);
                record.Approved = (approved as string) ?? "";
                record.Approvers = (approvers as List<string>)?.ToSepratedString() ??"";
                if(record.Action != "等待审批")
                {
                    record.Remark = (remark as string) ?? "";
                }
            }
            return Ok(rv);
        }

        public async Task<IActionResult> GetWorkflow(string flowname, string entitytype, string entityid)
        {
            var workflowId = DC.Set<Elsa_WorkflowInstance>().CheckEqual(flowname, x => x.Name).CheckEqual(entitytype, x => x.ContextType).CheckEqual(entityid, x => x.ContextId)
                .Select(x => x.ID).FirstOrDefault();
            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var instance = await lp.FindByIdAsync(workflowId);
            return Ok(instance);
        }

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
