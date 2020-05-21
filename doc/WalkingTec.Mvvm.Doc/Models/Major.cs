using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.Models
{
    public class Major : BasePoco
    {
        [Display(Name = "MajorCode")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]{3,3}$", ErrorMessage = "{0}必须是3位数字")]
        [StringLength(3)]
        public string MajorCode { get; set; }

        [Display(Name = "MajorName")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string MajorName { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "School")]
        public School School { get; set; }

        [Display(Name = "Student")]
        public List<StudentMajor> Students { get; set; }
        [Display(Name = "Student")]
        public List<StudentMajor> Students2 { get; set; }

    }
}
