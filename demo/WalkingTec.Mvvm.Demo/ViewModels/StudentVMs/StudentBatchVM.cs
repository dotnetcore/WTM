using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public partial class StudentBatchVM : BaseBatchVM<Student, Student_BatchEdit>
    {
        public StudentBatchVM()
        {
            ListVM = new StudentListVM();
            LinkedVM = new Student_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class Student_BatchEdit : BaseVM
    {
        [Display(Name = "账号")]
        public String ID { get; set; }
        [Display(Name = "密码")]
        public String Password { get; set; }
        [Display(Name = "邮箱")]
        public String Email { get; set; }
        [Display(Name = "姓名")]
        public String Name { get; set; }
        [Display(Name = "性别")]
        public GenderEnum? Sex { get; set; }
        [Display(Name = "手机")]
        public String CellPhone { get; set; }
        [Display(Name = "住址")]
        public String Address { get; set; }
        [Display(Name = "邮编")]
        public String ZipCode { get; set; }
        [Display(Name = "是否有效")]
        public Boolean? IsValid { get; set; }
        [Display(Name = "日期")]
        public DateRange EnRollDate { get; set; }
        public List<ComboSelectListItem> AllStudentMajors { get; set; }
        [Display(Name = "专业")]
        public List<Guid> SelectedStudentMajorIDs { get; set; }

        protected override void InitVM()
        {
            AllStudentMajors = DC.Set<Major>().GetSelectListItems(Wtm, y => y.MajorName);
        }

    }

}
