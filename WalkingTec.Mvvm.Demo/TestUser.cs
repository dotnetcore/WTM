using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.bbb;

namespace WalkingTec.Mvvm.Demo.aaa
{
    public class TestUser : BasePoco
    {

        [Display(Name = "性别")]
        public SexEnum? Sex { get; set; }

        [Display(Name = "手机")]
        [StringLength(30, ErrorMessage = "{0}最多输入{1}个字符")]
        public string CellPhone { get; set; }

        [Display(Name = "座机")]
        [StringLength(30, ErrorMessage = "{0}最多输入{1}个字符")]
        public string HomePhone { get; set; }

        [Display(Name = "住址")]
        [StringLength(200, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Address { get; set; }

        [Display(Name = "邮编")]
        [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "{0}必须是6位数字")]
        public string ZipCode { get; set; }

        [Display(Name = "照片")]
        public Guid? PhotoId { get; set; }

        [Display(Name = "照片")]
        public FileAttachment Photo { get; set; }

        [Display(Name = "角色")]
        public TestRole Role { get; set; }

        [Display(Name = "角色")]
        public Guid? RoleId { get; set; }

    }
}
