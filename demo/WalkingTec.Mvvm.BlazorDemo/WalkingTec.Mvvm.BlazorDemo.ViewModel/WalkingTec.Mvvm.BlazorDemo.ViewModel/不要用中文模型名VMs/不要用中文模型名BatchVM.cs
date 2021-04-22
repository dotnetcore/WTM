using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.不要用中文模型名VMs
{
    public partial class 不要用中文模型名BatchVM : BaseBatchVM<不要用中文模型名, 不要用中文模型名_BatchEdit>
    {
        public 不要用中文模型名BatchVM()
        {
            ListVM = new 不要用中文模型名ListVM();
            LinkedVM = new 不要用中文模型名_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class 不要用中文模型名_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
