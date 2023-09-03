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
    }
}
