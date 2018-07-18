using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public class PrivilegeItemListVM : BasePagedListVM<PrivilegeItemView, PrivilegeItemSearcher>
    {

        protected override IEnumerable<IGridColumn<PrivilegeItemView>> InitGridHeader()
        {
            return new List<GridColumn<PrivilegeItemView>>{
                this.MakeGridHeader(x => x.Text, 200),
            };
        }

        public override IOrderedQueryable<PrivilegeItemView> GetSearchQuery()
        {
            List<PrivilegeItemView> AllItems = new List<PrivilegeItemView>();
            var dps = ConfigInfo.DataPrivilegeSettings.Where(x => x.ModelName == Searcher.TableName).SingleOrDefault();
            if (dps != null)
            {
                AllItems = dps.GetItemList(DC, null).Select(x=> new PrivilegeItemView { Text = x.Text, Value = x.Value, ID = Guid.Parse(x.Value) }).ToList();
            }

            return AllItems.AsQueryable().OrderBy(x => x.Text);
        }
    }

    public class PrivilegeItemView : TopBasePoco
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class PrivilegeItemSearcher : BaseSearcher
    {
        public string TableName { get; set; }
    }
}
