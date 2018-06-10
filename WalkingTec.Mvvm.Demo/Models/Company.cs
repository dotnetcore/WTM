using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class Company : BasePoco
    {
        [Display(Name = "公司名称")]
        [StringLength(10)]
        public string Name { get; set; }

        [Display(Name = "公司描述")]
        [StringLength(200)]
        public string Description { get; set; }

        [Display(Name = "员工列表")]
        public List<Employee> EmployeeList { get; set; }
    }
}
