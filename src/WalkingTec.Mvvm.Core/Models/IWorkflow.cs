using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    public interface IWorkflow
    {
        string Workflow_Id { get; set; }
        string Workflow_Status { get; set; }
        DateTime? Workflow_StartTime { get; set; }
    }

    public abstract class WorkflowPoco : TopBasePoco, IWorkflow
    {
        [StringLength(40)]
        public string Workflow_Id { get; set; }
        [StringLength(40)]
        public string Workflow_Status { get; set; }
        public DateTime? Workflow_StartTime { get; set; }
    }

}
