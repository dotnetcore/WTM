using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs
{
    public partial class LinkTestBatchVM : BaseBatchVM<LinkTest, LinkTest_BatchEdit>
    {
        public LinkTestBatchVM()
        {
            ListVM = new LinkTestListVM();
            LinkedVM = new LinkTest_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class LinkTest_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
