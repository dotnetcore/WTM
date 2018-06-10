using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class TestRole : BasePoco
    {
        [Display(Name = "角色编号")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]{3,3}$", ErrorMessage = "{0}必须是3位数字")]
        [StringLength(6)]
        public string RoleCode { get; set; }

        [Display(Name = "角色名称")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string RoleName { get; set; }

        [Display(Name = "备注")]
        public string RoleRemark { get; set; }

        [NotMapped]
        public int test { get; set; }

        [NotMapped]
        public string abc { get; set; }

    }
}
