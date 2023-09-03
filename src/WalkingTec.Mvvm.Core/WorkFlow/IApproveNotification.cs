using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public interface IApproveNotification
    {
        void OnStart(ApproveInfo info);
        void OnResumt(ApproveInfo info);
    }
}
