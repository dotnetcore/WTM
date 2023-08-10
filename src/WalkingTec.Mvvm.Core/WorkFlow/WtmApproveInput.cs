using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class WtmApproveInput
    {
        public string Action { get; set; }
        public LoginUserInfo CurrentUser { get; set; }
        public string Remark { get; set; }
    }
}
