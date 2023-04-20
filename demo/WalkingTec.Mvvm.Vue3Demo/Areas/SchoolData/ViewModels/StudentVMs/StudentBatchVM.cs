using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.StudentVMs
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
        [Display(Name = "性别")]
        public GenderEnum? Sex { get; set; }
        [Display(Name = "日期")]
        public DateRange EnRollDate { get; set; }

        protected override async Task InitVM()
        {
        }

    }

}
