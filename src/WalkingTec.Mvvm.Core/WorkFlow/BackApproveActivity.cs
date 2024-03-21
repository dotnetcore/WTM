/*************************************************************************************
 *
 * 文 件 名:	BackApproveActivity
 * 描    述: 
 * 
 * 版    本:	V0.1
 * 创 建 者:	粗制乱造 (QQ:195593710)
 * 创建时间:	2024/3/16 17:11:18
 * ==================================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ==================================================
*************************************************************************************/
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Metadata;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
   [Trigger(
   Category = "工作流",
   DisplayName = "审批(回退)",
   Description = "审批流程中用于回退的中间节点，一般用于开始和不需要向上回退的节点。", 
   Outcomes = new[] { "提交" })]
    public class BackApproveActivity : Elsa.Services.Activity, IActivityPropertyOptionsProvider
    {

        private WTMContext _wtm;
        public BackApproveActivity(WTMContext wtm)
        {
            _wtm = wtm;
        }

        public object GetOptions(PropertyInfo property)
        {
            return null;
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var ardata = context.WorkFlowActivityRecord();

            //获取上一节点数据,如果全为空则为起始节点
            if (context.Input == null && ardata == null)
            {
                string submitter=context.WorkflowInstance.Variables.Get("Submitter")?.ToString();

                context.WorkFlowApproveRecord(new WorkFlowApproveRecord()
                {
                    Approved = submitter,
                    ApproveUsersFullText = [submitter],
                    ApprovedBy=submitter
                });

                return Outcome("提交");
            }


            if (context.Input != null)
            {
                WtmApproveInput input = context.Input as WtmApproveInput;
                //记录当前流程信息
                context.WorkFlowActivityRecord(input);
                ardata = context.WorkFlowActivityRecord();
            }

            //记录提交人信息,用于后续回退做准备
            if (ardata?.Action == "提交" || ardata?.Action == "同意")
            {
                context.WorkFlowApproveRecord(new WorkFlowApproveRecord()
                {
                    ApproveUsersFullText = [ardata.Name],
                    Approved = ardata.ITCode,
                    ApprovedBy = ardata.Name + "(" + ardata.ITCode.ToLower() + ")"
                });
            }

            if (ardata.Action != "回退")
            {
                return Outcome("提交");
            }

            var ar = context.WorkFlowApproveRecord();
            var approved = ar.Approved;
            if (approved != null)
            {
                _wtm.DC.Set<FrameworkWorkflow>().Add(new FrameworkWorkflow
                {
                    ActivityId = context.ActivityId,
                    WorkflowId = context.WorkflowInstance.Id,
                    UserCode = approved,
                    WorkflowName = context.WorkflowExecutionContext.WorkflowBlueprint.Name,
                    ModelID = context.ContextId,
                    ModelType = context.WorkflowExecutionContext.WorkflowContext?.GetType()?.FullName,
                    TenantCode = context.WorkflowInstance.TenantId,
                    Submitter = context.WorkflowInstance.Variables.Get("Submitter")?.ToString(),
                    StartTime = DateTime.Now,
                    Tag = "提交"
                });
                _wtm.DC.SaveChanges();

                context.JournalData.Add("ApproveUsersFullText", ar.ApproveUsersFullText ?? [approved]);
            }

            return Suspend();
        }



        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            var ardata = context.WorkFlowActivityRecord();

            if (context.Input != null)
            {
                WtmApproveInput input = context.Input as WtmApproveInput;
                //记录当前流程信息
                context.WorkFlowActivityRecord(input);
                ardata = context.WorkFlowActivityRecord();
            }

            if (ardata?.Action == "提交")
            {
                var exist = _wtm.DC.Set<FrameworkWorkflow>().IgnoreQueryFilters()
                        .Where(x =>
                        x.WorkflowId == context.WorkflowInstance.Id
                    && x.ActivityId == context.ActivityId
                    && x.TenantCode == context.WorkflowInstance.TenantId).ToList();
                _wtm.DC.Set<FrameworkWorkflow>().RemoveRange(exist);
                _wtm.DC.SaveChanges();


                var record = context.WorkFlowApproveRecord();

                //写入Elsa日志
                context.JournalData.Add("ApprovedBy", record.ApprovedBy);
                context.JournalData.Add("ApproveUsersFullText", record.ApproveUsersFullText ?? []);

                return Outcome(ardata?.Action);
            }

            return Suspend();
        }
    }
}
