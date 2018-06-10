using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CompanyVMs
{
    public class CompanyTemplateVM : BaseTemplateVM
    {
        [Display(Name = "公司名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<Company>(x => x.Name);
        [Display(Name = "公司描述")]
        public ExcelPropety Description_Excel = ExcelPropety.CreateProperty<Company>(x => x.Description);

	    protected override void InitVM()
        {
        }

    }

    public class CompanyImportVM : BaseImportVM<CompanyTemplateVM, Company>
    {

    }

}
