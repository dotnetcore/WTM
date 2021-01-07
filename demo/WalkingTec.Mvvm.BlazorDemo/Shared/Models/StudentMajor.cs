using System;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Demo.Models
{
    [MiddleTable]
    public class StudentMajor : BasePoco
    {
        public Major Major { get; set; }
        public Student Student { get; set; }

        public Guid MajorId { get; set; }

        public string StudentId { get; set; }
    }
}
