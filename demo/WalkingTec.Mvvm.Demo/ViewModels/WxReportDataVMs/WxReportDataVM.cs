using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.WxReportDataVMs
{
    public partial class WxReportDataVM : BaseCRUDVM<WxReportData>
    {
        public List<ComboSelectListItem> AllFrameworkUsers { get; set; }

        public WxReportDataVM()
        {
            SetInclude(x => x.FrameworkUser);
        }

        protected override void InitVM()
        {
            AllFrameworkUsers = DC.Set<FrameworkUser>().GetSelectListItems(Wtm, y => y.Name);
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
