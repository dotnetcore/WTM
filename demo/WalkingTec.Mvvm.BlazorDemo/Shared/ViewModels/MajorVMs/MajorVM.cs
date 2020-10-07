using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.BlazorDemo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModels.MajorVMs
{
    public class MajorVM : BaseCRUDVM<Major>
    {
        public List<ComboSelectListItem> AllSchools { get; set; }

        public MajorVM()
        {
            SetInclude(x => x.School);
        }

        protected override void InitVM()
        {
            AllSchools = DC.Set<School>().GetSelectListItems(WtmContext, null, y => y.SchoolName);
        }

    }
}
