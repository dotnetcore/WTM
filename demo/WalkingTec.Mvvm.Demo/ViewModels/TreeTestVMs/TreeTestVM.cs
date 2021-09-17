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

        protected override void InitVM()
        {
            //AllParents = DC.Set<TreeTest>().GetSelectListItems(Wtm, y => y.Name);
            ListVM = Wtm.CreateVM<TreeTestListVM>();
        }

        public override void DoAdd()
        {           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
