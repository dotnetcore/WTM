using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.ISOTypeVMs
{
    public partial class ISOTypeBatchVM : BaseBatchVM<ISOType, ISOType_BatchEdit>
    {
        public ISOTypeBatchVM()
        {
            ListVM = new ISOTypeListVM();
            LinkedVM = new ISOType_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class ISOType_BatchEdit : BaseVM
    {

        protected override async Task InitVM()
        {
        }

    }

}
