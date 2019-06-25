using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public class DpListVM : BasePagedListVM<DpView,DpSearcher>
    {
        public DpListVM()
        {
            NeedPage = false;
        }

        protected override IEnumerable<IGridColumn<DpView>> InitGridHeader()
        {
            return new List<GridColumn<DpView>>{
                this.MakeGridHeader(x => x.Name),
            };
        }

        public override IOrderedQueryable<DpView> GetSearchQuery()
        {

            var dps = ConfigInfo.DataPrivilegeSettings.Where(x => x.ModelName == Searcher.TableName).SingleOrDefault();
            if (dps != null)
            {
                return dps.GetItemList(DC, LoginUserInfo).Select(x => new DpView { ID = Guid.Parse(x.Value), Name = x.Text }).AsQueryable().OrderBy(x => x.Name);
            }
            else
            {
                return new List<DpView>().AsQueryable().OrderBy(x => x.Name);
            }
        }
    }

    public class DpView : TopBasePoco
    {
        [Display(Name = "名称")]
        public string Name { get; set; }
    }

    public class DpSearcher : BaseSearcher
    {
        public string TableName { get; set; }
    }
    
}
