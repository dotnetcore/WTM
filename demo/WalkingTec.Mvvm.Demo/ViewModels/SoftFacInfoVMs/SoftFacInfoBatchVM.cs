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
    public partial class SoftFacInfoBatchVM : BaseBatchVM<SoftFacInfo, SoftFacInfo_BatchEdit>
    {
        public SoftFacInfoBatchVM()
        {
            ListVM = new SoftFacInfoListVM();
            LinkedVM = new SoftFacInfo_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class SoftFacInfo_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
