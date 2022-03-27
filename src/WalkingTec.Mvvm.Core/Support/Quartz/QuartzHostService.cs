using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

namespace WalkingTec.Mvvm.Core.Support.Quartz
{
    public class QuartzHostService : IHostedService
    {
        private readonly IServiceProvider _sp;
        private readonly GlobalData _gd;
        private IScheduler _scheduler;
        public QuartzHostService(IServiceProvider sp,GlobalData gd)
        {
            _sp = sp;
            _gd = gd;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();
            foreach (var ass in _gd.AllAssembly)
            {
                try
                {
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
                            string jobName = st.Name;
                            string groupName = "group"+count;
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
                            JobDataMap jobDataMap = new JobDataMap();
                            jobDataMap.Add("Sp", _sp);
                            // 创建作业
                            IJobDetail j = JobBuilder.Create(st)
                              .WithIdentity(jobName, groupName)
                              .UsingJobData(jobDataMap)
                              .Build();
                            // 创建触发器，每60s执行一次
                            var builder = TriggerBuilder.Create()
                              .WithIdentity("trigger" + count, "group1");
                            if (startAttr != null)
                            {
                                builder = builder.WithCronSchedule(startAttr.Cron);
                            }
                            else {
                                builder = builder.StartNow();
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
            await Task.Delay(5000);
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
