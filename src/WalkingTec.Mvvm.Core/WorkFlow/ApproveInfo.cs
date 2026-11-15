using System;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// 审批信息载体，在审批流程开始或审批完成恢复时传递给 IApproveNotification 通知实现
    /// </summary>
    public class ApproveInfo
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName{get;set;}
        /// <summary>
        /// 流程实例 ID
        /// </summary>
        public string FlowInstanceId { get; set; }
        /// <summary>
        /// 本次等待审批的用户列表
        /// </summary>
        public List<LoginUserInfo> Approvers { get; set; }
        /// <summary>
        /// 本次等待审批的角色列表
        /// </summary>
        public List<SimpleRole> ApproveRoles { get; set; }
        /// <summary>
        /// 本次等待审批的部门列表
        /// </summary>
        public List<SimpleGroup> ApproveGroups { get; set; }
        /// <summary>
        /// 实际执行审批的用户信息（审批完成时才有值）
        /// </summary>
        public LoginUserInfo ApprovedBy { get; set; }
        /// <summary>
        /// 流程提交人 ITCode
        /// </summary>
        public string SubmitBy { get; set; }
        /// <summary>
        /// 审批节点标签名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 流程关联的业务实体类型
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 流程关联的业务实体 ID
        /// </summary>
        public string EntityId { get; set; }
    }
}
