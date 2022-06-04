using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;


namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs
{
    public partial class FrameworkTenantBatchVM : BaseBatchVM<FrameworkTenant, FrameworkTenant_BatchEdit>
    {
        public FrameworkTenantBatchVM()
        {
            ListVM = new FrameworkTenantListVM();
            LinkedVM = new FrameworkTenant_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class FrameworkTenant_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
