// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupBatchVM : BaseBatchVM<FrameworkGroup, BaseVM>
    {
        public FrameworkGroupBatchVM()
        {
            ListVM = new FrameworkGroupListVM();
        }

    }
}
