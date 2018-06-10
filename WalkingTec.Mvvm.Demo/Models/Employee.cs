using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class Employee : BasePoco
    {
        [Display(Name = "所属公司")]
        public Guid CompanyID { get; set; }
        public Company Company { get; set; }

        [Display(Name = "员工姓名")]
        [StringLength(10)]
        public string Name { get; set; }

        [Display(Name = "员工年龄")]
        public int Age { get; set; }

        [Display(Name = "员工性别")]
        public SexEnum Sex { get; set; }
    }
}
