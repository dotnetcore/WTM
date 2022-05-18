using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;


namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs
{
    public partial class FrameworkTenantSearcher : BaseSearcher
    {
        [Display(Name = "_Admin.TenantCode")]
        public String TCode { get; set; }
        [Display(Name = "_Admin.TenantName")]
        public String TName { get; set; }
        [Display(Name = "_Admin.TenantDomain")]
        public String TDomain { get; set; }

        protected override void InitVM()
        {
        }

    }
}
