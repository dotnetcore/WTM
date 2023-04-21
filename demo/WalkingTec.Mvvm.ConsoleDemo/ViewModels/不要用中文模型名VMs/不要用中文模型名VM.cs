using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.不要用中文模型名VMs
{
    public partial class 不要用中文模型名VM : BaseCRUDVM<不要用中文模型名>
    {

        public 不要用中文模型名VM()
        {
        }

        protected override Task InitVM()
        {
            return Task.CompletedTask;
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
