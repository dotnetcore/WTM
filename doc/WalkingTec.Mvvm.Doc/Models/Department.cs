using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.Models
{
    public class Department : TreePoco<Department>
    {
        [Display(Name = "DepName")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string DepName { get; set; }

    }
}
