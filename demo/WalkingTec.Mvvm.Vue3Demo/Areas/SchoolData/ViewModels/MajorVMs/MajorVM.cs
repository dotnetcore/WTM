using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.MajorVMs
{
    public partial class MajorVM : BaseCRUDVM<Major>
    {

        public MajorVM()
        {
            SetInclude(x => x.School);
            SetInclude(x => x.StudentMajors);
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
