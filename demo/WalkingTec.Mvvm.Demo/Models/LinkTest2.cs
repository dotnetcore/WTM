using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class LinkTest2 : TopBasePoco
    {
        public string name { get; set; }
        public List<LinkTestStudent> LinkStudent { get; set; }
    }

    [MiddleTable]
    public class LinkTestStudent : BasePoco
    {
        public LinkTest2 LinkTest { get; set; }
        public Student Student { get; set; }

        public Guid LinkTestId { get; set; }

        [Required()]
        public string StudentId { get; set; }
    }
}
