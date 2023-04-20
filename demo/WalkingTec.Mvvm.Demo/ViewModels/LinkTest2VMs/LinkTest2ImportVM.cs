using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTest2VMs
{
    public partial class LinkTest2TemplateVM : BaseTemplateVM
    {
        public ExcelPropety name_Excel = ExcelPropety.CreateProperty<LinkTest2>(x => x.name);

	    protected override async Task InitVM()
        {
        }

    }

    public class LinkTest2ImportVM : BaseImportVM<LinkTest2TemplateVM, LinkTest2>
    {

    }

}
