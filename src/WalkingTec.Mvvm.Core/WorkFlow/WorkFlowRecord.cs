/*************************************************************************************
 *
 * 文 件 名:	WorkFlowApproveRecord
 * 描    述: 
 * 
 * 版    本:	V0.1
 * 创 建 者:	粗制乱造 (QQ:195593710)
 * 创建时间:	2024/3/18 8:31:58
 * ==================================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ==================================================
*************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// 流程审批记录
    /// </summary>
    public class WorkFlowApproveRecord
    {
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approved { get; set; }
        /// <summary>
        /// 下一步审批
        /// </summary>
        public List<string> ApproveUsersFullText { get; set; } = [];
        /// <summary>
        /// 审批信息
        /// </summary>
        public string ApprovedBy { get; set; }

    }

    /// <summary>
    /// 工作流节点记录
    /// </summary>
    public class WorkFlowActivityRecord
    {
        //流程ID
        public string ActivityId { get; set; }
        /// <summary>
        /// 流程动作
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string ITCode { get; set; }

        /// <summary>
        /// 租(客)户代码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 审批信息
        /// </summary>
        public string Remark { get; set; }
    }
}
