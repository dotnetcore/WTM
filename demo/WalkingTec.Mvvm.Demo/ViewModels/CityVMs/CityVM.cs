using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CityVMs
{
    public partial class CityVM : BaseCRUDVM<City>
    {
        public List<TreeSelectListItem> AllParents { get; set; }
        public CityChildrenDetailListVM CityChildrenList { get; set; }
        public CityChildrenDetailListVM1 CityChildrenList1 { get; set; }

        public CityVM()
        {
            SetInclude(x => x.Parent);
            CityChildrenList = new CityChildrenDetailListVM();
            CityChildrenList.DetailGridPrix = "Entity.Children";
            CityChildrenList1 = new CityChildrenDetailListVM1();
            CityChildrenList1.DetailGridPrix = "Entity.Children";
        }

        protected override void InitVM()
        {
            AllParents = DC.Set<City>().GetTreeSelectListItems(Wtm, y => y.Name);
            CityChildrenList.CopyContext(this);
            CityChildrenList1.CopyContext(this);
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
