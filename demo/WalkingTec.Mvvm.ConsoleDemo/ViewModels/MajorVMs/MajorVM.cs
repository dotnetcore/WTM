using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public class MajorVM : BaseCRUDVM<Major>
    {
        public List<ComboSelectListItem> AllSchools { get; set; }

        public MajorVM()
        {
            SetInclude(x => x.School);
        }

        protected override async Task InitVM()
        {
            AllSchools = await DC.Set<School>().GetSelectListItems(Wtm, y => y.SchoolName);
        }

    }
}
