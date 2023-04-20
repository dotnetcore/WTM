using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.MajorVMs
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
        public String Remark { get; set; }
        public List<ComboSelectListItem> AllSchools { get; set; }
        [Display(Name = "所属学校")]
        public int? SchoolId { get; set; }

        protected override async Task InitVM()
        {
            AllSchools = await DC.Set<School>().GetSelectListItems(Wtm, y => y.SchoolName);
        }
    }

}
