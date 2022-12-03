using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Models
{
    public class Department : TreePoco<Department>
    {
        [Display(Name = "部门名称")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string DepName { get; set; }
    }
}
