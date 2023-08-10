// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.Workflow")]
    public class WorkflowController : BaseController
    {
        public IActionResult Index()
        {
            return PartialView();
        }

        public IActionResult IndexInner()
        {
            return PartialView();
        }
    }
}
