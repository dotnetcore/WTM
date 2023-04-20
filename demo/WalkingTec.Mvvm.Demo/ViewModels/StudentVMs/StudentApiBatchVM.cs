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
    public partial class StudentApiBatchVM : BaseBatchVM<Student, StudentApi_BatchEdit>
    {
        public StudentApiBatchVM()
        {
            ListVM = new StudentApiListVM();
            LinkedVM = new StudentApi_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class StudentApi_BatchEdit : BaseVM
    {
        [Display(Name = "账号")]
        public String ID { get; set; }
        [Display(Name = "邮箱")]
        public String Email { get; set; }
        [Display(Name = "住址")]
        public String Address { get; set; }
        [Display(Name = "日期")]
        public DateRange EnRollDate { get; set; }

        protected override async Task InitVM()
        {
        }

    }

}
