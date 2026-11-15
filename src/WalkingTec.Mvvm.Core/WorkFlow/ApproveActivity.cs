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
    /// <summary>
    /// WTM 基于 Elsa 工作流引擎封装的审批活动节点。
    /// 执行时汇总各类审批人并生成待审批记录，随后挂起流程；
    /// 审批人提交同意或拒绝后流程恢复并流转到对应分支。
    /// </summary>
    [Trigger(
   Category = "工作流",
   DisplayName = "审批",
   Description = "审批流程", Outcomes = new[] { "同意", "拒绝" })]
    public class WtmApproveActivity : Elsa.Services.Activity, IActivityPropertyOptionsProvider
    {
        /// <summary>
        /// 直接指定的审批人 ITCode 列表
        /// </summary>
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid","JavaScript" },
            IsDesignerCritical = true,
            Label = "审批人ITCode")]
        public List<string> ApproveUsers { get; set; }

        /// <summary>
        /// 审批人完整显示文本列表（用户为“姓名(ITCode)”格式，角色/部门为名称），运行时计算，不在设计器中显示
        /// </summary>
        [ActivityInput(IsBrowsable = false, DefaultSyntax = "Json")]
        public List<string> ApproveUsersFullText { get; set; }


        /// <summary>
        /// 按角色审批的角色 ID 列表
        /// </summary>
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "角色审批")]
        public ICollection<string> ApproveRoles { get; set; }


        /// <summary>
        /// 按部门审批的部门 ID 列表（部门内成员均可审批）
        /// </summary>
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            Hint = "请选择",
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "部门审批")]
        public ICollection<string> ApproveGroups { get; set; }


        /// <summary>
        /// 按部门主管审批的部门 ID 列表（实际审批人为所选部门的主管）
        /// </summary>
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "部门主管审批")]
        public ICollection<string> ApproveManagers { get; set; }

        /// <summary>
        /// 特殊审批类型列表（1 表示流程发起者自身，2 表示流程发起者所在部门的主管）
        /// </summary>
        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = "Json",
            SupportedSyntaxes = new[] { "Json", "Liquid" },
            IsDesignerCritical = true,
            OptionsProvider = typeof(WtmApproveActivity),
            Label = "特殊审批")]
        public ICollection<string> ApproveSpecials { get; set; }

        /// <summary>
        /// 审批备注（审批人提交审批意见时写入）
        /// </summary>
        [ActivityInput(IsBrowsable = false, Label = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 实际审批人显示文本（“姓名(ITCode)”格式，审批时写入）
        /// </summary>
        [ActivityInput(IsBrowsable = false, Label = "审批人")]
        public string ApprovedBy { get; set; }

        /// <summary>
        /// 审批节点标签，用于区分同一流程中的多个审批步骤
        /// </summary>
        [ActivityInput(
           UIHint = ActivityInputUIHints.SingleLine,
           Label = "标签")]
        public string Tag { get; set; }

        /// <summary>
        /// WTM 上下文，提供数据库访问、内部 API 调用等服务
        /// </summary>
        private WTMContext _wtm;
        /// <summary>
        /// 构造函数，注入 WTM 上下文
        /// </summary>
        /// <param name="wtm">WTM 上下文对象</param>
        public WtmApproveActivity(WTMContext wtm)
        {
            _wtm = wtm;
        }


        /// <summary>
        /// 为设计器中各审批配置属性提供下拉选项数据
        /// </summary>
        /// <param name="property">需要提供选项的活动属性</param>
        /// <returns>属性对应的下拉选项列表；获取失败时返回空列表</returns>
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


        /// <summary>
        /// 活动执行入口：汇总各类审批人（指定用户、角色、部门、部门主管、特殊审批），
        /// 为每个审批人/角色/部门生成待审批记录（FrameworkWorkflow）写入数据库，
        /// 发送审批开始通知后挂起流程，等待审批操作
        /// </summary>
        /// <param name="context">Elsa 活动执行上下文</param>
        /// <returns>挂起结果，表示流程暂停等待审批输入</returns>
        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            // 获取流程上下文关联的业务实体
            var entity = context.GetWorkflowContext();
            var query = "";
            // 初始化审批人完整显示文本列表，后续按用户/角色/部门分别填充
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

            // 指定用户审批：查询审批人姓名，并为每个审批人生成一条待审批记录
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
            // 角色审批：以“r:角色ID”作为 UserCode 生成待审批记录
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
            // 部门审批：以“g:部门ID”作为 UserCode 生成待审批记录
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
            // 保存所有待审批记录
            _wtm.DC.SaveChanges();
            // 尝试发送审批开始通知（未注册通知服务或通知失败时忽略）
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

        /// <summary>
        /// 审批人提交审批后恢复执行：记录审批人和备注，删除本活动的全部待审批记录，
        /// 发送审批恢复通知，并按“同意”或“拒绝”流转到对应输出分支
        /// </summary>
        /// <param name="context">Elsa 活动执行上下文</param>
        /// <returns>同意或拒绝时返回对应输出分支的结果，其他输入则继续返回挂起结果</returns>
        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            // 获取审批人提交的输入，记录审批备注和审批人显示文本
            WtmApproveInput input = context.Input as WtmApproveInput;
            Remark = input?.Remark;
            ApprovedBy = input.CurrentUser.Name + "(" + input.CurrentUser.ITCode.ToLower() + ")";
            if (input?.Action == "同意" || input?.Action == "拒绝")
            {
                // 删除当前流程实例、当前活动对应的所有待审批记录（忽略查询过滤器，包含软删除数据）
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
