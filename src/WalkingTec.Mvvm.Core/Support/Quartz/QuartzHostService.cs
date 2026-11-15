using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

namespace WalkingTec.Mvvm.Core.Support.Quartz
{
    /// <summary>
    /// Quartz 宿主服务：随宿主启动时扫描程序集中所有继承自 WtmJob 的作业类型，按特性配置构建作业与触发器并注册到调度器；宿主停止时关闭调度器
    /// </summary>
    public class QuartzHostService : IHostedService
    {
        // 全局服务提供者，通过 JobDataMap 传入作业供其解析服务
        private readonly IServiceProvider _sp;
        // 全局数据，用于遍历所有程序集
        private readonly GlobalData _gd;
        // Quartz 调度器
        private IScheduler _scheduler;
        /// <summary>
        /// 初始化 Quartz 宿主服务
        /// </summary>
        /// <param name="sp">服务提供者，用于作业内部解析服务</param>
        /// <param name="gd">全局数据，用于扫描作业类型</param>
        public QuartzHostService(IServiceProvider sp,GlobalData gd)
        {
            _sp = sp;
            _gd = gd;
        }

        /// <summary>
        /// 启动调度器：扫描所有程序集中继承自 WtmJob 的作业类型，依据其特性构建作业与触发器并注册到调度器，最后启动调度
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>启动任务</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            foreach (var ass in _gd.AllAssembly)
            {
                try
                {
                    // 查找当前程序集中所有继承自 WtmJob 的具体作业类型（排除基类本身）
                    var t = ass.GetExportedTypes().Where(x => typeof(WtmJob).IsAssignableFrom(x) && x.Name != "WtmJob").ToList();
                    int count = 1;
                    foreach (var st in t)
                    {
                        var ci = st.GetConstructor(Type.EmptyTypes);
                        var job = ci?.Invoke(null) as WtmJob;
                        if (job != null)
                        {
                           // job.Sp = _sp;
                           var attrs = st.GetCustomAttributes(true);
                            // 未标记名称/分组特性时，使用类型名和自动生成的组名作为默认值
                            string jobName = st.Name;
                            string groupName = "wtmgroup"+count;
                            var nameAttr = attrs.Where(x => x is QuartzJobAttribute).FirstOrDefault() as QuartzJobAttribute;
                            var groupAttr = attrs.Where(x => x is QuartzGroupAttribute).FirstOrDefault() as QuartzGroupAttribute;
                            var repeatAttr = attrs.Where(x => x is QuartzRepeatAttribute).FirstOrDefault() as QuartzRepeatAttribute;
                            var startAttr = attrs.Where(x => x is QuartzStartAtAttribute).FirstOrDefault() as QuartzStartAtAttribute;
                            if (nameAttr != null)
                            {
                                jobName = nameAttr.Name;
                            }
                            if(groupAttr != null)
                            {
                                groupName = groupAttr.Group;
                            }
                            // 通过 JobDataMap 把服务提供者传入作业，供作业内部解析服务
                            JobDataMap jobDataMap = new JobDataMap();
                            jobDataMap.Add("Sp", _sp);
                            // 创建作业
                            IJobDetail j = JobBuilder.Create(st)
                              .WithIdentity(jobName, groupName)
                              .UsingJobData(jobDataMap)
                              .Build();
                            // 创建触发器，每60s执行一次
                            var builder = TriggerBuilder.Create()
                              .WithIdentity("wtmtrigger" + count, "group1");
                            // 触发器策略：优先使用 Cron 表达式；否则使用简单调度（支持延迟启动、按次数重复或永久重复）
                            if (startAttr != null)
                            {
                                builder = builder.WithCronSchedule(startAttr.Cron);
                            }
                            else {
                                if (repeatAttr != null && repeatAttr.DelaySeconds > 0)
                                {
                                    builder = builder.StartAt(DateTime.Now.AddSeconds(repeatAttr.DelaySeconds));
                                }
                                else
                                {
                                    builder = builder.StartNow();
                                }
                                if(repeatAttr != null)
                                {
                                    if(repeatAttr.IsForever == true)
                                    {
                                        builder = builder.WithSimpleSchedule(x => x.WithIntervalInSeconds(repeatAttr.IntervalInSeconds).RepeatForever());                             
                                    }
                                    else
                                    {
                                        builder = builder.WithSimpleSchedule(x => x.WithRepeatCount(repeatAttr.Repeat).WithIntervalInSeconds(repeatAttr.IntervalInSeconds));
                                    }
                                }
                             }
                            var trigger = builder.Build();
                            await _scheduler.ScheduleJob(j, trigger);
                            count++;
                        }
                    }
                }
                catch { }
            }
            // 开始运行
            await _scheduler.Start(cancellationToken);
        }

        /// <summary>
        /// 停止并关闭 Quartz 调度器
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>停止任务</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
