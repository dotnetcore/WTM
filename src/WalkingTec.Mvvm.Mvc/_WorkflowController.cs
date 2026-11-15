using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Open.Linq.AsyncExtensions;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 审批流管理页面控制器，提供流程设计器等流程管理页面的入口
    /// </summary>
    [AuthorizeJwtWithCookie]
    [AllRights]
    public class _WorkflowController : BaseController
    {
        /// <summary>
        /// 流程管理主页面；仅限流程管理员(角色代码 001 或角色名称"流程管理员")访问，
        /// 通过权限校验后将当前用户的租户代码写入 Cookie 供工作流页面使用
        /// </summary>
        /// <returns>流程管理页面视图；无权限时返回 Forbid(403)</returns>
        [ActionDescription("流程管理")]
        public IActionResult Inner()
        {
            // 校验当前用户是否为流程管理员
            if (Wtm.LoginUserInfo.Roles.Any(x => x.RoleCode == "001") ||
                Wtm.LoginUserInfo.Roles.Any(x => x.RoleName == "流程管理员"))
            {
                // 将当前租户代码写入 Cookie 供工作流页面使用；无租户时删除该 Cookie，避免残留旧值
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
