using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Metadata;
using Elsa.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    [Trigger(
   Category = "工作流",
   DisplayName = "审批",
   Description = "审批流程",Outcomes = new[] {"同意","拒绝"})]
    public class WtmApproveActivity : Elsa.Services.Activity, IActivityPropertyOptionsProvider
    {
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            //OptionsProvider = typeof(WtmApproveActivity),            
            Label = "审批人ITCode")]
        public ICollection<string> ApproveUsers { get; set; }

        [ActivityInput(IsBrowsable = false, DefaultSyntax = "Json")]
        public List<string> ApproveUsersFullText { get; set; }

        //[ActivityInput(
        //    UIHint = ActivityInputUIHints.MultiText,
        //    DefaultSyntax = "Json",
        //    SupportedSyntaxes = new[] { "Json", "Liquid" },
        //    DefaultValue = new string[] {"同意", "拒绝"},
        //    Label = "分支",
        //    ConsiderValuesAsOutcomes = true)]
        //public ICollection<string> PossibleOutcomes { get; set; }

        [ActivityInput(IsBrowsable = false, Label = "备注")]
        public string Remark { get; set; }

        [ActivityInput(IsBrowsable = false, Label = "审批人")]
        public string ApprovedBy { get; set; }

        [ActivityInput(
           UIHint = ActivityInputUIHints.SingleLine,
           Label = "标签")]
        public string Tag { get; set; }

        private WTMContext _wtm;
        public WtmApproveActivity(WTMContext wtm)
        {
            _wtm = wtm;
        }


        public object GetOptions(PropertyInfo property)
        {
            var rv = new List<ComboSelectListItem>();
            try
            {
                var check = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowUsers").Result;
                if (check.Data != null)
                {
                    rv = check.Data;
                }
            }
            catch { }
            return rv.Select(x=> new SelectListItem { Text = $"{x.Text}({x.Value})" , Value = x.Value.ToString()}).ToList();
        }


        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var entity = context.GetWorkflowContext();
            var query = "";
            foreach (var item in ApproveUsers)
            {
                query += $"itcode={item}&";
            }
            query += "1=1";
            var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowUsers?{query}").Result;
            ApproveUsersFullText = names.Data.Select(x=> $"{x.Text}({x.Value})").ToList();
            foreach (var item in ApproveUsers)
            {
                _wtm.DC.Set<FrameworkWorkflow>().Add(new FrameworkWorkflow
                {
                    ActivityId = context.ActivityId,
                    WorkflowId = context.WorkflowInstance.Id,
                    UserCode = item,
                    WorkflowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                    ModelID = context.ContextId,
                    ModelType = context.WorkflowExecutionContext.WorkflowContext?.GetType()?.FullName,
                    TenantCode = context.WorkflowInstance.TenantId,
                    Submitter = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                    StartTime = DateTime.Now,
                    Tag = this.Tag
                });
            }
            _wtm.DC.SaveChanges();
            var notify = _wtm.ServiceProvider.GetRequiredService<IApproveNotification>();
            if (notify != null)
            {
                ApproveInfo info = new ApproveInfo
                {
                    EntityId = context.ContextId,
                    EntityType = context.WorkflowExecutionContext.WorkflowContext?.GetType(),
                    FlowInstanceId = context.WorkflowInstance.Id,
                    FlowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                    SubmitBy = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                    TagName = this.Tag,
                    Approvers = names.Data.Select(x => new LoginUserInfo { ITCode = x.Value.ToString(), Name = x.Text }).ToList()
                };
                notify.OnStart(info);
            }
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            WtmApproveInput input = context.Input as WtmApproveInput;           
            Remark = input?.Remark;
            ApprovedBy = input.CurrentUser.Name + "("+ input.CurrentUser.ITCode.ToLower()+")";
            if ((input?.Action =="同意" || input?.Action =="拒绝") && ApproveUsers.Any(x => x.ToLower() == input.CurrentUser.ITCode.ToLower()))
            {
                var exist = _wtm.DC.Set<FrameworkWorkflow>().Where(x=>
                    x.WorkflowId == context.WorkflowInstance.Id
                &&  x.ActivityId == context.ActivityId).ToList();
                _wtm.DC.Set<FrameworkWorkflow>().RemoveRange(exist);
                _wtm.DC.SaveChanges();

                var notify = _wtm.ServiceProvider.GetRequiredService<IApproveNotification>();
                if (notify != null)
                {
                    var query = "";
                    foreach (var item in ApproveUsers)
                    {
                        query += $"itcode={item}&";
                    }
                    query += "1=1";
                    var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowUsers?{query}").Result;
                    ApproveInfo info = new ApproveInfo
                    {
                        ApprovedBy = input.CurrentUser,
                        EntityId = context.ContextId,
                        EntityType = context.WorkflowExecutionContext.WorkflowContext?.GetType(),
                        FlowInstanceId = context.WorkflowInstance.Id,
                        FlowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                        SubmitBy = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                        TagName = this.Tag,
                        Approvers = names.Data.Select(x => new LoginUserInfo { ITCode = x.Value.ToString(), Name = x.Text }).ToList()
                    };
                    notify.OnResume(info);
                }


                return Outcome(input?.Action);

            }
            else
            {
                return Suspend();
            }
        }

    }
}
