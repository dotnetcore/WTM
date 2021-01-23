using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.WxReportDataVMs
{
    public partial class WxReportDataSearcher : BaseSearcher
    {
        public List<ComboSelectListItem> AllFrameworkUsers { get; set; }
        [Display(Name = "姓名")]
        public Guid? FrameworkUserId { get; set; }
        [Display(Name = "加油1:升")]
        public Double? JiaYou1 { get; set; }
        [Display(Name = "扩展字段")]
        public String Extend04 { get; set; }
        [Display(Name = "扩展字段")]
        public String Extend05 { get; set; }
        [Display(Name = "扩展字段")]
        public Int32? Extend08 { get; set; }

        protected override void InitVM()
        {
            AllFrameworkUsers = DC.Set<FrameworkUser>().GetSelectListItems(Wtm, y => y.Name);
        }

    }
}
