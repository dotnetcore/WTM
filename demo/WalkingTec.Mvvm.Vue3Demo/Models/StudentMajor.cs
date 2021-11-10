using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.ReactDemo.Models
{
    [MiddleTable]
    public class StudentMajor : BasePoco
    {
        public Major Major { get; set; }
        public Student Student { get; set; }

        public Guid MajorId { get; set; }

        [Required()]
        public string StudentId { get; set; }
    }
}
