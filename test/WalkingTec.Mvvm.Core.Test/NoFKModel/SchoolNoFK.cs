using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Test
{

    [SoftKey(nameof(SchoolNoFK.SchoolCode))]
    public class SchoolNoFK : BasePoco
    {
        [Display(Name = "学校编码")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]{3,3}$", ErrorMessage = "{0}必须是3位数字")]
        [CanNotEdit]
        public string SchoolCode { get; set; }

        [Display(Name = "学校名称")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string SchoolName { get; set; }

        [Display(Name = "学校类型")]
        [Required(ErrorMessage = "{0}是必填项")]
        public SchoolTypeEnum? SchoolType { get; set; }

        [Display(Name = "备注")]
        [Required]
        public string Remark { get; set; }

        [Display(Name = "专业")]
        [NotMapped]
        [SoftFK(nameof(MajorNoFK.SchoolId))]
        public List<MajorNoFK> Majors { get; set; }

        [Display(Name = "专业")]
        [NotMapped]
        public List<Major> Majors2 { get; set; }
    }

}
