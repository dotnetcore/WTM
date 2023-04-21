using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.HospitalVMs
{
    public partial class HospitalVM : BaseCRUDVM<Hospital>
    {
        public List<TreeSelectListItem> AllLocations { get; set; }

        public HospitalVM()
        {
            SetInclude(x => x.Location);
        }

        protected override async Task InitVM()
        {
            AllLocations = await DC.Set<City>().GetTreeSelectListItems(Wtm, y => y.Name);
        }

        public override async Task DoAdd()
        {           
            await base.DoAdd();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            await base.DoEdit(updateAllFields);
        }

        public override async Task DoDelete()
        {
            await base.DoDelete();
        }
    }
}
