using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.ViewModels.ShipperInfoVMs
{
    public class ShipperInfoBatchVM : BaseBatchVM<ShipperInfo, ShipperInfo_BatchEdit>
    {
        public ShipperInfoBatchVM()
        {
            ListVM = new ShipperInfoListVM();
            LinkedVM = new ShipperInfo_BatchEdit();
        }

        protected override bool CheckIfCanDelete(Guid id, out string errorMessage)
        {
            errorMessage = null;
			return true;
        }
    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class ShipperInfo_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
