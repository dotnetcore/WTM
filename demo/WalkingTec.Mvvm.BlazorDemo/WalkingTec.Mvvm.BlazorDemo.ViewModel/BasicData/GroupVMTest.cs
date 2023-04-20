using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData
{
    public class GroupVMTest : BaseVM
    {
        public string EntityId { get; set; } 
        public CityVM vm1 { get; set; } = new CityVM();

        public CityVM vm2 { get; set; } = new CityVM();

        protected override async Task InitVM()
        {
            if(EntityId != null)
            {
                vm1 = Wtm.CreateVM<CityVM>(EntityId);
                vm2 = Wtm.CreateVM<CityVM>(vm1.Entity.ParentId);
            }
            base.InitVM();
        }
    }
}
