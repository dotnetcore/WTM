using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Metadata;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    [Trigger(
   Category = "工作流",
   DisplayName = "审批",
   Description = "审批流程", Outcomes = new[] { "同意", "拒绝" })]
    public class WtmApproveActivity : Elsa.Services.Activity, IActivityPropertyOptionsProvider
    {
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            Label = "审批人ITCode")]
        public List<string> ApproveUsers { get; set; }

        [ActivityInput(IsBrowsable = false, DefaultSyntax = "Json")]
        public List<string> ApproveUsersFullText { get; set; }


        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "角色审批")]
        public ICollection<string> ApproveRoles { get; set; }


        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            Hint = "请选择",
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "部门审批")]
        public ICollection<string> ApproveGroups { get; set; }


        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "部门主管审批")]
        public ICollection<string> ApproveManagers { get; set; }

        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "特殊审批")]
        public ICollection<string> ApproveSpecials { get; set; }

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
            if (property.Name == "ApproveRoles")
            {
                try
                {
                    var check = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowRoles").Result;
                    if (check.Data != null)
                    {
                        rv = check.Data;
                    }
                }
                catch { }
                return rv;
            }
            if (property.Name == "ApproveGroups")
            {
                try
                {
                    var check = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowGroups").Result;
                    if (check.Data != null)
                    {
                        rv = check.Data;
                    }
                }
                catch { }
                return rv;
            }
            if (property.Name == "ApproveManagers")
            {
                try
                {
                    var check = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowGroups").Result;
                    if (check.Data != null)
                    {
                        rv = check.Data;
                    }
                }
                catch { }
                rv.ForEach(x => x.Text += "主管");
                return rv;
            }
            if (property.Name == "ApproveSpecials")
            {
                rv.Add(new ComboSelectListItem { Text = "流程发起者", Value = "1" });
                rv.Add(new ComboSelectListItem { Text = "流程发起者所在部门主管", Value = "2" });
                return rv;
            }
            return rv;
        }


        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var entity = context.GetWorkflowContext();
            var query = "";
            ApproveUsersFullText = new List<string>();
            List<ComboSelectListItem> users = new List<ComboSelectListItem>();
            List<ComboSelectListItem> roles = new List<ComboSelectListItem>();
            List<ComboSelectListItem> groups = new List<ComboSelectListItem>();
            List<ComboSelectListItem> managers = new List<ComboSelectListItem>();
            if (ApproveUsers == null)
            {
                ApproveUsers = new List<string>();
            }
            //添加特殊审批
            if (ApproveSpecials?.Count > 0)
            {
                var submitter = context.WorkflowInstance.Variables.Get("Submitter")?.ToString();
                foreach (var item in ApproveSpecials)
                {
                    //发起者自身
                    if (item == "1")
                    {
                        if (string.IsNullOrEmpty(submitter) == false)
                        {
                            ApproveUsers.Add(submitter);
                        }
                    }
                    else if (item == "2")
                    {
                        query = $"itcode={submitter}";

                        var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowMyGroupManagers?{query}").Result;
                        users = names.Data ?? new List<ComboSelectListItem>();
                        if (users.Count > 0)
                        {
                            ApproveUsers.Add(users.FirstOrDefault()?.Text);
                        }
                    }
                }
            }
            //添加部门管理员到ApproveUsers
            if (ApproveManagers?.Count > 0)
            {
                query = "";
                foreach (var item in ApproveManagers)
                {
                    query += $"ids={item}&";
                }
                query += "1=1";
                var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowGroupManagers?{query}").Result;
                managers = names.Data ?? new List<ComboSelectListItem>();
                foreach (var item in managers)
                {
                    if (string.IsNullOrEmpty(item.Text) == false)
                    {
                        ApproveUsers.Add(item.Text);
                    }
                }
            }

            if (ApproveUsers?.Count > 0)
            {
                query = "";
                foreach (var item in ApproveUsers)
                {
                    query += $"itcode={item}&";
                }
                query += "1=1";
                var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowUsers?{query}").Result;
                users = names.Data ?? new List<ComboSelectListItem>();
                ApproveUsersFullText.AddRange(users.Select(x => $"{x.Text}({x.Value})").ToList());
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
            }
            if (ApproveRoles?.Count > 0)
            {
                query = "";
                foreach (var item in ApproveRoles)
                {
                    query += $"ids={item}&";
                }
                query += "1=1";
                var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowRoles?{query}").Result;
                roles = names.Data ?? new List<ComboSelectListItem>();
                ApproveUsersFullText.AddRange(roles.Select(x => $"{x.Text}").ToList());
                foreach (var item in ApproveRoles)
                {
                    _wtm.DC.Set<FrameworkWorkflow>().Add(new FrameworkWorkflow
                    {
                        ActivityId = context.ActivityId,
                        WorkflowId = context.WorkflowInstance.Id,
                        UserCode = "r:" + item,
                        WorkflowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                        ModelID = context.ContextId,
                        ModelType = context.WorkflowExecutionContext.WorkflowContext?.GetType()?.FullName,
                        TenantCode = context.WorkflowInstance.TenantId,
                        Submitter = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                        StartTime = DateTime.Now,
                        Tag = this.Tag
                    });
                }
            }
            if (ApproveGroups?.Count > 0)
            {
                query = "";
                foreach (var item in ApproveGroups)
                {
                    query += $"ids={item}&";
                }
                query += "1=1";
                var names = _wtm.CallAPI<List<ComboSelectListItem>>("", $"{_wtm.HostAddress}/_workflowapi/GetWorkflowGroups?{query}").Result;
                groups = names.Data ?? new List<ComboSelectListItem>();
                ApproveUsersFullText.AddRange(groups.Select(x => $"{x.Text}").ToList());
                foreach (var item in ApproveGroups)
                {
                    _wtm.DC.Set<FrameworkWorkflow>().Add(new FrameworkWorkflow
                    {
                        ActivityId = context.ActivityId,
                        WorkflowId = context.WorkflowInstance.Id,
                        UserCode = "g:" + item,
                        WorkflowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                        ModelID = context.ContextId,
                        ModelType = context.WorkflowExecutionContext.WorkflowContext?.GetType()?.FullName,
                        TenantCode = context.WorkflowInstance.TenantId,
                        Submitter = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                        StartTime = DateTime.Now,
                        Tag = this.Tag
                    });
                }
            }
            _wtm.DC.SaveChanges();
            try
            {
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
                        Approvers = users.Select(x => new LoginUserInfo { ITCode = x.Value.ToString(), Name = x.Text }).ToList(),
                        ApproveRoles = roles.Select(x => new Support.Json.SimpleRole { ID = Guid.Parse(x.Value.ToString()), RoleName = x.Text }).ToList(),
                        ApproveGroups = groups.Select(x => new Support.Json.SimpleGroup { ID = Guid.Parse(x.Value.ToString()), GroupName = x.Text }).ToList(),
                    };
                    notify.OnStart(info);
                }
            }
            catch { }
            return Suspend();
        }

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            WtmApproveInput input = context.Input as WtmApproveInput;
            Remark = input?.Remark;
            ApprovedBy = input.CurrentUser.Name + "(" + input.CurrentUser.ITCode.ToLower() + ")";
            if (input?.Action == "同意" || input?.Action == "拒绝")
            {
                var exist = _wtm.DC.Set<FrameworkWorkflow>().IgnoreQueryFilters()
                    .Where(x =>
                    x.WorkflowId == context.WorkflowInstance.Id
                && x.ActivityId == context.ActivityId
                && x.TenantCode == context.WorkflowInstance.TenantId).ToList();
                _wtm.DC.Set<FrameworkWorkflow>().RemoveRange(exist);
                _wtm.DC.SaveChanges();
                try
                {
                    var notify = _wtm.ServiceProvider.GetRequiredService<IApproveNotification>();
                    if (notify != null)
                    {
                        ApproveInfo info = new ApproveInfo
                        {
                            ApprovedBy = input.CurrentUser,
                            EntityId = context.ContextId,
                            EntityType = context.WorkflowExecutionContext.WorkflowContext?.GetType(),
                            FlowInstanceId = context.WorkflowInstance.Id,
                            FlowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                            SubmitBy = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                            TagName = this.Tag,
                        };
                        notify.OnResume(info);
                    }
                }
                catch { }

                return Outcome(input?.Action);

            }
            else
            {
                return Suspend();
            }
        }

    }
}
