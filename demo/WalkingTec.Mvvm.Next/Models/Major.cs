using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.ReactDemo.Models
{

    public enum MajorTypeEnum
    {
        [Display(Name = "必修")]
        Required,
        [Display(Name = "选修")]
        Optional
    }

    public class Major : BasePoco
    {
        [Display(Name = "专业编码")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]{3,3}$", ErrorMessage = "{0}必须是3位数字")]
        public string MajorCode { get; set; }

        [Display(Name = "专业名称")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string MajorName { get; set; }

        [Required(ErrorMessage = "{0}是必填项")]
        [Display(Name = "专业类别")]
        public MajorTypeEnum? MajorType { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "所属学校")]
        public int? SchoolId { get; set; }


        [Display(Name = "所属学校")]
        public School School { get; set; }

        [Display(Name = "学生")]
        public List<StudentMajor> StudentMajors { get; set; }

    }
}
