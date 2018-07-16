using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.Models
{
    public enum SexEnum
    {
        [Display(Name = "男")]
        Male = 0,
        [Display(Name = "女")]
        Female = 1
    }
    public class Student : BasePoco
    {
        [Display(Name = "账号")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string LoginName { get; set; }

        [Display(Name = "密码")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(32)]
        public string Password { get; set; }

        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Email { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Name { get; set; }

        [Display(Name = "性别")]
        public SexEnum? Sex { get; set; }

        [Display(Name = "手机")]
        [RegularExpression("^[1][3,4,5,7,8][0-9]{9}$", ErrorMessage = "{0}格式错误")]
        public string CellPhone { get; set; }

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

        [Display(Name = "是否有效")]
        public bool IsValid { get; set; }

        [Display(Name = "日期")]
        public DateTime? EnRollDate { get; set; }

        [Display(Name = "日期区间")]
        public DateTime? EnRollDateRange { get; set; }

        [Display(Name = "年份区间")]
        public int? EnYearRange { get; set; }

        [Display(Name = "月份区间")]
        public int? EnMonthRange { get; set; }

        [Display(Name = "时间区间")]
        public string EnTimeRange { get; set; }

        [Display(Name = "英文日历")]
        public string EnTimeRange1 { get; set; }

        [Display(Name = "公历节日")]
        public string EnTimeRange2 { get; set; }

        [Display(Name = "标注日期")]
        public string EnTimeRange3 { get; set; }

        [Display(Name = "事件")]
        public string EnTimeRange4 { get; set; }

        [Display(Name = "专业")]
        public List<StudentMajor> Majors { get; set; }
    }
}
