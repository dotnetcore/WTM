using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class ApproveInfo
    {
        public string FlowName{get;set;}
        public string FlowInstanceId { get; set; }
        public List<LoginUserInfo> Approvers { get; set; }
        public LoginUserInfo ApprovedBy { get; set; }
        public string SubmitBy { get; set; }
        public string TagName { get; set; }
        public Type EntityType { get; set; }
        public string EntityId { get; set; }
    }
}
