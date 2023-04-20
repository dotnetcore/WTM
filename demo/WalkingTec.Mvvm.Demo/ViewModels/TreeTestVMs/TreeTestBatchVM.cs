using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.TreeTestVMs
{
    public partial class TreeTestBatchVM : BaseBatchVM<TreeTest, TreeTest_BatchEdit>
    {
        public TreeTestBatchVM()
        {
            ListVM = new TreeTestListVM();
            LinkedVM = new TreeTest_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class TreeTest_BatchEdit : BaseVM
    {

        protected override async Task InitVM()
        {
        }

    }

}
