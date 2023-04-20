using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.VueDemo.BasicData.ViewModels.SchoolVMs
{
    public partial class SchoolVM : BaseCRUDVM<School>
    {

        public SchoolVM()
        {
        }

        protected override async Task InitVM()
        {
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
            base.DoDelete();
        }
    }
}
