using System;
using System.Threading.Tasks;
using Quartz;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Quartz;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    [QuartzRepeat(3600,0,true)]
    public class JwtRefreshJob : WtmJob
    {
        public override  Task Execute(IJobExecutionContext context)
        {
            try
            {
                var sql = $"DELETE FROM {this.Wtm.DC.GetTableName<PersistedGrant>()} WHERE Expiration<'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                this.Wtm.DC.RunSQL(sql);
                Wtm.DoLog("清理过期的refreshToken", ActionLogTypesEnum.Job);
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
