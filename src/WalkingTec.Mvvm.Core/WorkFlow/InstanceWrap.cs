using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elsa.Models;
using NodaTime;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class InstanceWrap : WorkflowInstance
    {
        public new DateTime CreatedAt { get; set; }
        public new DateTime? LastExecutedAt { get; set; }
        public new DateTime? FinishedAt { get; set; }
        public new DateTime? CancelledAt { get; set; }
        public new DateTime? FaultedAt { get; set; }

    }
}
