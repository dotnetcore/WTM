using Elsa;
using Elsa.Activities.Workflows.Workflow;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications;
using Elsa.Server.Api.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetBox.Extensions;
using NodaTime;
using Open.Linq.AsyncExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Core.WorkFlow;

namespace WalkingTec.Mvvm.Mvc
{
    [AuthorizeJwtWithCookie]
    [AllRights]
    public class _WorkflowController : BaseController
    {
        [ActionDescription("流程管理")]
        public IActionResult Inner()
        {
            if (Wtm.LoginUserInfo.Roles.Any(x => x.RoleCode == "001") ||
                Wtm.LoginUserInfo.Roles.Any(x => x.RoleName == "流程管理员"))
            {
                if (Wtm.LoginUserInfo.TenantCode != null)
                {
                    Response.Cookies.Append("workflowtenant", Wtm.LoginUserInfo.TenantCode);
                }
                else
                {
                    Response.Cookies.Delete("workflowtenant");
                }
                return View();
            }
            return Forbid();
        }

    }
}
