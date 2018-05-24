using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupBatchVM : BaseBatchVM<FrameworkGroup, BaseVM>
    {
        public FrameworkGroupBatchVM()
        {
            ListVM = new FrameworkGroupListVM();
        }

        protected override bool CheckIfCanDelete(Guid id, out string errorMessage)
        {
            var check = DC.Set<FrameworkUserGroup>().Any(x => x.GroupId == id);
            if (check == true)
            {
                errorMessage = "用户组已被使用，无法删除";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }
    }
}
