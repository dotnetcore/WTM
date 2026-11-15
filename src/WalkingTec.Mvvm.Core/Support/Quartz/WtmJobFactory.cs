using System;
using Quartz;
using Quartz.Spi;

namespace WalkingTec.Mvvm.Core.Support.Quartz
{
    /// <summary>
    /// WTM 定时作业工厂：Quartz 调度器通过它创建作业实例
    /// </summary>
    public class WtmJobFactory : IJobFactory
    {
        /// <summary>
        /// 创建指定作业明细对应的作业实例
        /// </summary>
        /// <param name="bundle">触发器触发时携带的作业明细等信息</param>
        /// <param name="scheduler">当前调度器</param>
        /// <returns>新创建的作业实例</returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var rv = Activator.CreateInstance(bundle.JobDetail.GetType()) as IJob;
            return rv;
        }

        /// <summary>
        /// 回收作业实例（未实现，作业实例由调用方管理）
        /// </summary>
        /// <param name="job">待回收的作业实例</param>
        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
