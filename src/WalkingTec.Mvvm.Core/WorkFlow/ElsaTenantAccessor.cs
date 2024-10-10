using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    public class ElsaTenantAccessor : ITenantAccessor
    {
        readonly IHttpContextAccessor _hca;

        public ElsaTenantAccessor(IHttpContextAccessor hca)
        {
            _hca = hca;
        }

        public Task<string> GetTenantIdAsync(CancellationToken cancellationToken = default)
        {

            string t = null;
            var wtm = _hca.HttpContext.RequestServices.GetRequiredService<WTMContext>();
            if (wtm?.LoginUserInfo != null)
            {
                t = wtm?.LoginUserInfo?.CurrentTenant;
            }
            else
            {
                if (_hca.HttpContext.Request.Cookies.ContainsKey("workflowtenant"))
                {
                    t = _hca.HttpContext.Request.Cookies["workflowtenant"];
                }
            }
            return Task.FromResult(t);
        }
    }
}
