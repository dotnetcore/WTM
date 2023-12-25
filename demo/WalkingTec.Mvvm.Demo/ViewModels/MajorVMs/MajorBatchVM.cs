using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public partial class MajorBatchVM : BaseBatchVM<Major, Major_BatchEdit>
    {
        public MajorBatchVM()
        {
            ListVM = new MajorListVM();
            LinkedVM = new Major_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class Major_BatchEdit : BaseVM
    {
        [Display(Name = "备注")]
        public new String Remark { get; set; }
        public List<ComboSelectListItem> AllSchools { get; set; }
        [Display(Name = "所属学校")]
        public int? SchoolId { get; set; }
        public List<ComboSelectListItem> AllStudentMajorss { get; set; }
        [Display(Name = "学生")]
        public List<string> SelectedStudentMajorsIDs { get; set; }

        protected override void InitVM()
        {
            AllSchools = DC.Set<School>().GetSelectListItems(Wtm, y => y.SchoolName);
            AllStudentMajorss = DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

}
