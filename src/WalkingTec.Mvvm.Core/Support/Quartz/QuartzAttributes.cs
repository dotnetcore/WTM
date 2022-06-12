using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Support.Quartz
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzJobAttribute : Attribute
    {
        public string Name { get; set; }
        public QuartzJobAttribute(string name)
        {
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzGroupAttribute : Attribute
    {
        public string Group { get; set; }
        public QuartzGroupAttribute(string group)
        {
            this.Group = group;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzRepeatAttribute : Attribute
    {
        public int Repeat { get; set; }
        public bool IsForever { get; set; }
        public int IntervalInSeconds { get; set; }

        public int DelaySeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">间隔时间，单位秒</param>
        /// <param name="repeat">重复的次数</param>
        /// <param name="isForever">是否一直重复</param>
        /// <param name="delaySeconds">延迟多少秒开始第一次运行</param>
        public QuartzRepeatAttribute(int interval,int repeat, bool isForever,int delaySeconds=10)
        {
            this.Repeat = repeat;
            this.IsForever = isForever;
            this.IntervalInSeconds = interval;
            this.DelaySeconds = delaySeconds;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzStartAtAttribute : Attribute
    {
        public string Cron { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cron">表示复杂格式的日期，详见https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html#introduction</param>
        public QuartzStartAtAttribute(string cron)
        {
            this.Cron = cron;
        }
    }
}
