using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;


namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs
{
    public partial class FrameworkTenantTemplateVM : BaseTemplateVM
    {
        [Display(Name = "_Admin.TenantCode")]
        public ExcelPropety TCode_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.TCode);
        [Display(Name = "_Admin.TenantName")]
        public ExcelPropety TName_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.TName);
        [Display(Name = "_Admin.TenantDb")]
        public ExcelPropety TDb_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.TDb);
        [Display(Name = "_Admin.TenantDbType")]
        public ExcelPropety TDbType_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.TDbType);
        [Display(Name = "_Admin.TenantDbContext")]
        public ExcelPropety DbContext_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.DbContext);
        [Display(Name = "_Admin.TenantDomain")]
        public ExcelPropety TDomain_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.TDomain);
        [Display(Name = "_Admin.TenantEnableSub")]
        public ExcelPropety EnableSub_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.EnableSub);
        [Display(Name = "Sys.Enable")]
        public ExcelPropety Enabled_Excel = ExcelPropety.CreateProperty<FrameworkTenant>(x => x.Enabled);

        protected override void InitVM()
        {
        }

    }

    public class FrameworkTenantImportVM : BaseImportVM<FrameworkTenantTemplateVM, FrameworkTenant>
    {

    }

}
