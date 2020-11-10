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

        protected override bool CheckIfCanDelete(object id, out string errorMessage)
        {
            Guid? checkid = Guid.Parse(id.ToString());
            var check = DC.Set<FrameworkUserGroup>().Any(x => x.GroupId == checkid);
            if (check == true)
            {
                errorMessage = Program._localizer["CannotDelete", Program._localizer["Group"]];
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
