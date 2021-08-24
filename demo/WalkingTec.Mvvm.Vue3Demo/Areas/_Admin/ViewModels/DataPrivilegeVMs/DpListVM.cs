// WTM默认页面 Wtm buidin page
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

            var dps = Wtm.DataPrivilegeSettings.Where(x => x.ModelName == Searcher.TableName).SingleOrDefault();
            if (dps != null)
            {
                return dps.GetItemList(Wtm, Searcher.Filter).Select(x => new DpView { ID = x.Value.ToString(), Name = x.Text }).AsQueryable().OrderBy(x => x.Name);
            }
            else
            {
                return new List<DpView>().AsQueryable().OrderBy(x => x.Name);
            }
        }

        public override IOrderedQueryable<DpView> GetBatchQuery()
        {
            var dps = Wtm.DataPrivilegeSettings.Where(x => x.ModelName == Searcher.TableName).SingleOrDefault();
            if (dps != null)
            {
                return dps.GetItemList(Wtm, null,Ids).Select(x => new DpView { ID = x.Value.ToString(), Name = x.Text }).AsQueryable().OrderBy(x => x.Name);
            }
            else
            {
                return new List<DpView>().AsQueryable().OrderBy(x => x.Name);
            }
        }
    }

    public class DpView : TopBasePoco
    {
        public new string ID { get; set; }

        [Display(Name = "_Admin.DataPrivilegeName")]
        public string Name { get; set; }
    }

    public class DpSearcher : BaseSearcher
    {
        public string TableName { get; set; }
        [Display(Name = "_Admin.DataPrivilegeName")]
        public string Filter { get; set; }
    }

}
