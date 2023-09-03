using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elsa.Models;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class ResultWrap
    {
        public InstanceWrap WorkflowInstance { get; set; }

        public string ActivityId { get; set; }
        public Exception Exception { get; set; }
        public bool Executed { get; set; }
    }
}
