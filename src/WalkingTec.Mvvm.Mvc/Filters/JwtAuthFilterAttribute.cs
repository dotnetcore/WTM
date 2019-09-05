using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    public class JwtAuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is IBaseController))
            {
                base.OnActionExecuting(context);
                return;
            }
            if (context.Controller is BaseController)
            {
                base.OnActionExecuting(context);
                return;
            }

            if (!(context.ActionDescriptor is ControllerActionDescriptor actionDescriptor))
            {
                base.OnActionExecuting(context);
                return;
            }

            var isAuthorize = actionDescriptor.MethodInfo.IsDefined(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false)
                           || actionDescriptor.ControllerTypeInfo.IsDefined(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);

            if (!isAuthorize)
            {
                base.OnActionExecuting(context);
                return;
            }

            var tokenHeader = context.HttpContext.GetJwtToken();

            if (!string.IsNullOrEmpty(tokenHeader))
            {
                var user = context.HttpContext.User;

                try
                {
                    string userId;
                    if (user != null)
                    {
                        userId = user.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    }
                    else
                    {
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var token = jwtHandler.ReadJwtToken(tokenHeader);
                        userId = token.Id;
                    }
                    if (string.IsNullOrEmpty(userId))
                    {
                        base.OnActionExecuting(context);
                        return;
                    }

                    JwtHelper.Signin(userId, context.HttpContext);
                }
                catch
                {
                }

                base.OnActionExecuting(context);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
