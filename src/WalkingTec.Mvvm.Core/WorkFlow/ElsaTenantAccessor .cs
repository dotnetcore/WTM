using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// Elsa 租户访问器，为工作流引擎提供当前租户 ID：
    /// 已登录时取登录用户所属租户，未登录时从 workflowtenant Cookie 中读取
    /// </summary>
    public class ElsaTenantAccessor : ITenantAccessor
    {
        /// <summary>
        /// HTTP 上下文访问器，用于获取当前请求信息
        /// </summary>
        readonly IHttpContextAccessor _hca;

        /// <summary>
        /// 构造函数，注入 HTTP 上下文访问器
        /// </summary>
        /// <param name="hca">HTTP 上下文访问器</param>
        public ElsaTenantAccessor(IHttpContextAccessor hca)
        {
            _hca = hca;
        }

        /// <summary>
        /// 异步获取当前租户 ID：优先使用当前登录用户所属租户，
        /// 未登录时从 workflowtenant Cookie 中读取
        /// </summary>
        /// <param name="cancellationToken">取消操作的令牌</param>
        /// <returns>当前租户 ID；无法确定时返回 null</returns>
        public Task<string> GetTenantIdAsync(CancellationToken cancellationToken = default)
        {

            string t = null;
            var wtm = _hca.HttpContext.RequestServices.GetRequiredService<WTMContext>();
            // 已登录时优先使用当前登录用户所属租户
            if (wtm?.LoginUserInfo != null)
            {
                t = wtm?.LoginUserInfo?.CurrentTenant;
            }
            else
            {
                // 未登录（如工作流设计器匿名访问）时从 workflowtenant Cookie 中读取租户标识
                if (_hca.HttpContext.Request.Cookies.ContainsKey("workflowtenant"))
                {
                    t = _hca.HttpContext.Request.Cookies["workflowtenant"];
                }
            }
            return Task.FromResult(t);
        }
    }
}
