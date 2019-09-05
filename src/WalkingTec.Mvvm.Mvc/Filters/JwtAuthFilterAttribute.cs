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
                    string key;
                    if (user != null)
                    {
                        key = user.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                    }
                    else
                    {
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var token = jwtHandler.ReadJwtToken(tokenHeader);
                        key = token.Id;
                    }
                    if (string.IsNullOrEmpty(key))
                    {
                        base.OnActionExecuting(context);
                        return;
                    }

                    var memoryCache = GlobalServices.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
                    if (memoryCache.TryGetValue(key, out object cacheObj))
                    {
                        if (cacheObj is LoginUserInfo loginUser)
                        {
                            context.HttpContext.Session?.Set("UserInfo", loginUser);
                        }
                    }
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
