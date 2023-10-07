using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class ApproveTimeLine
    {
        public string Id { get; set; }
        public string Time { get; set; }
        public string Action { get; set; }
        public string Remark { get; set; }
        public string Approvers { get; set; }
        public string Approved { get; set; }

        public string Message
        {
            get
            {
                string rv = "";
                switch (Action)
                {
                    case "等待审批":
                        rv = "等待" + this.Approvers + "审批";
                        break;
                    case "同意":
                    case "拒绝":
                        rv = this.Approved + this.Action;
                        break;
                    case "_start":
                        rv = this.Approvers + "提交审批";
                        break;
                    case "_finish":
                        rv = "流程结束";
                        break;
                }
                return rv;
            }
        }
    }
}
