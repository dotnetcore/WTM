using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core.Test
{
    [MiddleTable]
    public class StudentMajorWithOtherID : BasePoco
    {
        public Major Major { get; set; }
        public Student Student { get; set; }

        public string MajorId { get; set; }

        public Guid StudentId { get; set; }
    }

}
