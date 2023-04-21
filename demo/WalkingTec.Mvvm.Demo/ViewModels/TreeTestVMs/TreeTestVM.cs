using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.TreeTestVMs
{
    public partial class TreeTestVM : BaseCRUDVM<TreeTest>
    {
        public List<ComboSelectListItem> AllParents { get; set; }
        public TreeTestListVM ListVM { get; set; }
        public TreeTestVM()
        {
            SetInclude(x => x.Parent);
        }

        protected override async Task InitVM()
        {
            AllParents = await DC.Set<TreeTest>().GetSelectListItems(Wtm, y => y.Name);
            ListVM = Wtm.CreateVM<TreeTestListVM>();
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
