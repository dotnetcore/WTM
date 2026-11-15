using System;

namespace WalkingTec.Mvvm.Core.Support.Quartz
{
    /// <summary>
    /// 定时作业名称特性：标记一个类为 Quartz 定时作业，并指定作业名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzJobAttribute : Attribute
    {
        /// <summary>
        /// 作业名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 初始化作业名称特性
        /// </summary>
        /// <param name="name">作业名称</param>
        public QuartzJobAttribute(string name)
        {
            this.Name = name;
        }
    }

    /// <summary>
    /// 定时作业分组特性：标记作业所属的分组名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzGroupAttribute : Attribute
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 初始化作业分组特性
        /// </summary>
        /// <param name="group">分组名称</param>
        public QuartzGroupAttribute(string group)
        {
            this.Group = group;
        }
    }

    /// <summary>
    /// 定时作业重复策略特性：定义作业的执行间隔、重复次数、是否永久重复以及首次运行前的延迟时间
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzRepeatAttribute : Attribute
    {
        /// <summary>
        /// 重复执行的次数
        /// </summary>
        public int Repeat { get; set; }
        /// <summary>
        /// 是否永久重复执行
        /// </summary>
        public bool IsForever { get; set; }
        /// <summary>
        /// 两次执行之间的间隔，单位秒
        /// </summary>
        public int IntervalInSeconds { get; set; }

        /// <summary>
        /// 首次运行前延迟的秒数
        /// </summary>
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

    /// <summary>
    /// 定时作业触发时间特性：使用 Cron 表达式定义作业的执行时间计划
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzStartAtAttribute : Attribute
    {
        /// <summary>
        /// Cron 表达式
        /// </summary>
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
