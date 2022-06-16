using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core.Test
{
    [MiddleTable]
    public class StudentMajorNoFK : BasePoco
    {
        [NotMapped]
        public MajorNoFK Major { get; set; }
        public string MajorId { get; set; }
        [NotMapped]
        public StudentNoFK Student { get; set; }
        public string StudentId { get; set; }

    }

}
