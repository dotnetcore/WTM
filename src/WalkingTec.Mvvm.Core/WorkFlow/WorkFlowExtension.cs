/*************************************************************************************
 *
 * 文 件 名:	WorkFlowExtension
 * 描    述: 
 * 
 * 版    本:	V0.1
 * 创 建 者:	粗制乱造 (QQ:195593710)
 * 创建时间:	2024/3/18 8:20:17
 * ==================================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ==================================================
*************************************************************************************/
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public static class WorkFlowExtension
    {

        /// <summary>
        /// 记录流程动作
        /// </summary>
        /// <param name="context"></param>
        /// <param name="recode"></param>
        /// <returns></returns>
        public static void WorkFlowApproveRecord(this ActivityExecutionContext context, WorkFlowApproveRecord recode)
        {
            //获取流程记录
            var data=context.WorkflowInstance.Variables.Get<Dictionary<string, WorkFlowApproveRecord>>("WorkFlowApproveRecord");
            if (data == null)
            {
                data = new();
            }

            if (data.ContainsKey(context.ActivityId))
            {
                data[context.ActivityId] = recode;
            }
            else
            {
                data.Add(context.ActivityId, recode);
            }
            context.WorkflowInstance.Variables.Set("WorkFlowApproveRecord", data);
        }

        /// <summary>
        /// 读取流程审批记录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static WorkFlowApproveRecord WorkFlowApproveRecord(this ActivityExecutionContext context)
        {
            WorkFlowApproveRecord record = null;
            var data = context.WorkflowInstance.Variables.Get<Dictionary<string, WorkFlowApproveRecord>>("WorkFlowApproveRecord");
            if (data != null&&data.ContainsKey(context.ActivityId))
            {
                record=data[context.ActivityId];
            }
            return record;
        }

        //写入当前流程数据
        public static void WorkFlowActivityRecord(this ActivityExecutionContext context, WtmApproveInput input)
        {
            if (input != null)
            {

                context.WorkflowInstance.Variables.Set("WorkFlowActivityRecord", new WorkFlowActivityRecord() {
                    ActivityId = context.ActivityId,
                    Action = input.Action,
                    Name = input.CurrentUser.Name,
                    Remark = input.Remark,
                    ITCode = input.CurrentUser.ITCode,
                    UserId = input.CurrentUser.UserId,
                    TenantCode = input.CurrentUser.TenantCode,
                });
            }
        }

        //获取当前流程数据
        public static WorkFlowActivityRecord WorkFlowActivityRecord(this ActivityExecutionContext context)
        {
            return context.WorkflowInstance.Variables.Get<WorkFlowActivityRecord>("WorkFlowActivityRecord");
        }
    }
}
