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
    public class MajorBatchVM : BaseBatchVM<Major, Major_BatchEdit>
    {
        public MajorBatchVM()
        {
            ListVM = new MajorListVM();
            LinkedVM = new Major_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class Major_BatchEdit : BaseVM
    {
        public List<ComboSelectListItem> AllSchools { get; set; }
        [Display(Name = "所属学校")]
        public int? SchoolId { get; set; }

        protected override void InitVM()
        {
            AllSchools = DC.Set<School>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.SchoolName);
        }

    }

}
