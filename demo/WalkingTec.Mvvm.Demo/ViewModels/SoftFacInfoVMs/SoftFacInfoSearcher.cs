using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SoftFacInfoVMs
{
    public partial class SoftFacInfoSearcher : BaseSearcher
    {
        [Display(Name = "exe名称")]
        public String IsoName { get; set; }
        [Display(Name = " 版本号")]
        public String EXEVerSion { get; set; }
        [Display(Name = " 附加信息")]
        public String Description { get; set; }
        public List<ComboSelectListItem> AlliSOTypess { get; set; }
        [Display(Name = "ISO版本")]
        public List<Guid> SelectediSOTypesIDs { get; set; }

        protected override void InitVM()
        {
            AlliSOTypess = DC.Set<ISOType>().GetSelectListItems(Wtm, y => y.IsoName);
        }

    }
}
