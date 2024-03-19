using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;

namespace WalkingTec.Mvvm.Demo.ViewModels.DataTableVMs
{
    public class ChartVM : BaseVM
    {
        public ActionLogSearcher Searchertest { get; set; }

        protected override void InitVM()
        {
            Searchertest = new ActionLogSearcher();
            Searchertest.CopyContext(this);
            Searchertest.DoInit();
        }
    }
}
