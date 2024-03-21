using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.BlazorDemo.Model;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.WorkFlowDemoVMs
{
    public partial class WorkFlowDemoTemplateVM : BaseTemplateVM
    {

	    protected override void InitVM()
        {
        }

    }

    public class WorkFlowDemoImportVM : BaseImportVM<WorkFlowDemoTemplateVM, WorkFlowDemo>
    {

    }

}
