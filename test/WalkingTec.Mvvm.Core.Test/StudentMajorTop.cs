using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core.Test
{
    [MiddleTable]
    public class StudentMajorTop : TopBasePoco
    {
        public MajorTop Major { get; set; }
        public StudentTop Student { get; set; }

        public Guid MajorId { get; set; }

        public Guid StudentId { get; set; }
    }

}
