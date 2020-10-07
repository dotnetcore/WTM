using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.BlazorDemo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModels.StudentVMs
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

        protected override void InitVM()
        {
        }

    }

}
