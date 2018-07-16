using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.Models
{
    public class StudentMajor : BasePoco
    {
        public Major Major { get; set; }
        public Student Student { get; set; }

        public Guid MajorId { get; set; }

        public Guid StudentId { get; set; }
    }
}
