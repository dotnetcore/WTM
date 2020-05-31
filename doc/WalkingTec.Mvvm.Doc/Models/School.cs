using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.Models
{
    public enum SchoolTypeEnum
    {
        [Display(Name = "Public")]
        PUB,
        [Display(Name = "Private")]
        PRI
    }

    public class School : BasePoco
    {
        [Display(Name = "SchoolCode")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]{3,3}$", ErrorMessage = "{0}必须是3位数字")]
        [StringLength(3)]
        public string SchoolCode { get; set; }

        [Display(Name = "SchoolName")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string SchoolName { get; set; }

        [Display(Name = "SchoolType")]
        [Required(ErrorMessage = "{0}是必填项")]
        public SchoolTypeEnum? SchoolType { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Major")]
        public List<Major> Majors { get; set; }
        [Display(Name = "Photo")]
        public List<SchoolPhoto> Photos { get; set; }
    }

    public class SchoolPhoto : TopBasePoco, ISubFile
    {
        public Guid SchoolId { get; set; }
        public School School { get; set; }

        public Guid FileId { get; set; }
        public FileAttachment File { get; set; }
        public int order { get; set; }
    }

}
