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
        [Display(Name = "Male")]
        Male = 0,
        [Display(Name = "Female")]
        Female = 1
    }
    public class Student : BasePoco
    {
        [Display(Name = "Account")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string LoginName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(32)]
        public string Password { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Name { get; set; }

        [Display(Name = "Sex")]
        public SexEnum? Sex { get; set; }

        [Display(Name = "CellPhone")]
        [RegularExpression("^[1][3,4,5,7,8][0-9]{9}$", ErrorMessage = "{0}formaterror")]
        public string CellPhone { get; set; }

        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "{0}stringmax{1}")]
        public string Address { get; set; }

        [Display(Name = "Zip")]
        [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "{0}formaterror")]
        public string ZipCode { get; set; }

        [Display(Name = "Photo")]
        public Guid? PhotoId { get; set; }

        [Display(Name = "Photo")]
        public FileAttachment Photo { get; set; }

        [Display(Name = "IsValid")]
        public bool IsValid { get; set; }

        [Display(Name = "Date")]
        public DateTime? EnRollDate { get; set; }

        [Display(Name = "DateRange")]
        public DateRange EnRollDateRange { get; set; }

        [Display(Name = "YearRange")]
        public DateRange EnYearRange { get; set; }

        [Display(Name = "MonthRange")]
        public DateRange EnMonthRange { get; set; }

        [Display(Name = "TimeRange")]
        public string EnTimeRange { get; set; }

        [Display(Name = "EditTime")]
        public DateRange EnTimeRange0 { get; set; }

        [Display(Name = "EnglishC")]
        public string EnTimeRange1 { get; set; }

        [Display(Name = "Holiday")]
        public string EnTimeRange2 { get; set; }

        [Display(Name = "Mark")]
        public string EnTimeRange3 { get; set; }

        [Display(Name = "Event")]
        public DateRange EnTimeRange4 { get; set; }

        [Display(Name = "Major")]
        public List<StudentMajor> Majors { get; set; }

        public Department Department { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
