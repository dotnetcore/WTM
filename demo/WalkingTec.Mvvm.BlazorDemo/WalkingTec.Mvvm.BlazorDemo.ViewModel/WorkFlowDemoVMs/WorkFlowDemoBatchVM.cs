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
    public partial class WorkFlowDemoBatchVM : BaseBatchVM<WorkFlowDemo, WorkFlowDemo_BatchEdit>
    {
        public WorkFlowDemoBatchVM()
        {
            ListVM = new WorkFlowDemoListVM();
            LinkedVM = new WorkFlowDemo_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class WorkFlowDemo_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
