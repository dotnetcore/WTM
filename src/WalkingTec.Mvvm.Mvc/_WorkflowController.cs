using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    [AllRights]
    public class _WorkflowController : BaseController
    {
        [ActionDescription("流程管理")]
        public IActionResult Inner()
        {
            return View();

        }

    }
}
