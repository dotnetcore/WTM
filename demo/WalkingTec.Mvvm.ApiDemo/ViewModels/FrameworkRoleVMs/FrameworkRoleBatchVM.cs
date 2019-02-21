using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleBatchVM : BaseBatchVM<FrameworkRole, FrameworkRole_BatchEdit>
    {
        public FrameworkRoleBatchVM()
        {
            ListVM = new FrameworkRoleListVM();
            LinkedVM = new FrameworkRole_BatchEdit();
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
    public class FrameworkRole_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
