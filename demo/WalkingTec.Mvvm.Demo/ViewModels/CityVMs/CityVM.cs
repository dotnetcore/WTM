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

        public CityListVM list { get; set; }
        public CitySearcher se { get; set; }
        public CityVM()
        {
            SetInclude(x => x.Parent);
            CityChildrenList = new CityChildrenDetailListVM();
            CityChildrenList.DetailGridPrix = "Entity.Children";
            CityChildrenList1 = new CityChildrenDetailListVM1();
            CityChildrenList1.DetailGridPrix = "Entity.Children";
            list = new CityListVM();
            se = new CitySearcher();
        }

        protected override async Task InitVM()
        {
            AllParents = await DC.Set<City>().GetTreeSelectListItems(Wtm, y => y.Name);
            CityChildrenList.CopyContext(this);
            CityChildrenList1.CopyContext(this);
            list.CopyContext(this);
            se.CopyContext(this);
            await se.DoInit();
        }
    }
}
