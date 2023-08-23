using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    public interface IWorkflow
    {
        Guid? Workflow_Id { get; set; }
        string Workflow_Status { get; set; }
        DateTime? Workflow_StartTime { get; set; }
    }
}
