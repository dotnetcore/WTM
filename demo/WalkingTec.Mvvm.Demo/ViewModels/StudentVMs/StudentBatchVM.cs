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
        [Display(Name = "密码")]
        public String Password { get; set; }
        [Display(Name = "性别")]
        public GenderEnum? Sex { get; set; }
        [Display(Name = "邮编")]
        public String ZipCode { get; set; }
        public List<ComboSelectListItem> AllStudentMajors { get; set; }
        [Display(Name = "专业")]
        public List<Guid> SelectedStudentMajorIDs { get; set; }
        
        public DateTime? EnRollDate { get; set; }

        protected override async Task InitVM()
        {
            AllStudentMajors = await DC.Set<Major>().GetSelectListItems(Wtm, y => y.MajorName);
        }

    }

}
