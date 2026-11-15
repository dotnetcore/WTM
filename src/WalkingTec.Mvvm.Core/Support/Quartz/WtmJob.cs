using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace WalkingTec.Mvvm.Core.Support.Quartz
{
    /// <summary>
    /// WTM 定时作业基类：封装 WTM 上下文的按需初始化与释放，业务作业继承本类并重写 Execute 方法编写业务逻辑
    /// </summary>
    public class WtmJob : IJob, IDisposable
    {
        // 懒加载创建的服务作用域
        private IServiceScope _ss;
        // 懒加载创建的 WTM 上下文
        private WTMContext _wtm;
        /// <summary>
        /// 当前 WTM 上下文：首次访问时创建独立的服务作用域并从中解析上下文
        /// </summary>
        protected WTMContext Wtm
        {
            get
            {
                // 懒加载：首次访问时创建服务作用域并初始化 WTM 上下文
                if (_wtm == null)
                {
                    _ss = Sp.CreateScope();
                    _wtm = _ss.ServiceProvider.GetRequiredService<WTMContext>();
                    _wtm.SetServiceProvider(_ss.ServiceProvider);
                }
                return _wtm;
            }
        }

        /// <summary>
        /// 服务提供者：由调度器在构建作业时通过 JobDataMap 注入，用于作业内部解析服务
        /// </summary>
        public IServiceProvider Sp { get; set; }

        /// <summary>
        /// 作业执行入口：默认空实现，由派生类重写以编写具体的业务逻辑
        /// </summary>
        /// <param name="context">Quartz 的作业执行上下文</param>
        /// <returns>执行任务</returns>
        public virtual async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => { });
        }

        /// <summary>
        /// 释放 WTM 上下文和服务作用域
        /// </summary>
        public void Dispose()
        {
            _wtm?.Dispose();
            _wtm = null;
            _ss?.Dispose();
            _ss = null;
        }
    }
}
