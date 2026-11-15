using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Core
{

    /// <summary>
    /// WTM 日志扩展类，提供 WTM 日志提供程序的注册方法
    /// </summary>
    public static class WTMeLoggerExtensions
    {
        /// <summary>
        /// 注册 WTM 日志提供程序，使框架日志写入数据库
        /// </summary>
        /// <param name="builder">日志构建器</param>
        /// <returns>传入的日志构建器</returns>
        public static ILoggingBuilder AddWTMLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WTMLoggerProvider>());
            return builder;
        }
    }

    /// <summary>
    /// WTM 日志提供程序，创建将日志写入数据库的 WTMLogger 实例
    /// </summary>
    [ProviderAlias("WTM")]
    public class WTMLoggerProvider : ILoggerProvider
    {
        private IServiceProvider sp = null;
        private LoggerFilterOptions logConfig;

        /// <summary>
        /// 构造函数，读取当前日志过滤配置并保存服务提供者
        /// </summary>
        /// <param name="_logConfig">日志过滤配置监视器</param>
        /// <param name="sp">服务提供者</param>
        public WTMLoggerProvider( IOptionsMonitor<LoggerFilterOptions> _logConfig, IServiceProvider sp)
        {
            this.sp = sp;
            logConfig = _logConfig.CurrentValue;
        }

        /// <summary>
        /// 按日志分类名称创建 WTM 日志器
        /// </summary>
        /// <param name="categoryName">日志分类名称</param>
        /// <returns>WTM 日志器实例</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new WTMLogger(categoryName, logConfig,sp);
        }
        /// <summary>
        /// 释放资源，无需要释放的内容
        /// </summary>
        public void Dispose() { }
    }

    /// <summary>
    /// WTM 日志器，实现 ILogger 接口，将符合条件的日志写入数据库的 ActionLog 表
    /// </summary>
    public class WTMLogger : ILogger
    {
        private readonly string categoryName;
        private IServiceProvider sp;
        private LoggerFilterOptions logConfig;

        /// <summary>
        /// 构造函数，初始化日志分类名称、过滤规则与服务提供者
        /// </summary>
        /// <param name="categoryName">日志分类名称</param>
        /// <param name="logConfig">日志过滤规则配置</param>
        /// <param name="sp">服务提供者</param>
        public WTMLogger(string categoryName, LoggerFilterOptions logConfig, IServiceProvider sp)
        {
            this.categoryName = categoryName;
            this.logConfig = logConfig;
            this.sp = sp;
        }

        /// <summary>
        /// 判断指定日志级别是否允许记录，依据 WTM 提供程序的过滤规则判断
        /// </summary>
        /// <param name="logLevel">待判断的日志级别</param>
        /// <returns>允许记录返回 true，否则返回 false</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            // 无过滤配置或 VueCliMiddleware 分类的日志一律不记录
            if(logConfig == null || categoryName == "VueCliMiddleware")
            {
                return false;
            }
            // 从过滤规则中查找 WTM 提供程序下匹配当前分类的最低日志级别，未命中则视为不记录
            var level = logConfig.Rules.Where(x =>
                x.ProviderName == "WTM" &&
                    (
                      (x.CategoryName != null &&  categoryName.ToLower().StartsWith(x.CategoryName.ToLower()) ) ||
                      categoryName == "WalkingTec.Mvvm.Core.ActionLog"
                    )
                )
                .Select(x => x.LogLevel).FirstOrDefault();
            if (level == null)
            {
                level = LogLevel.None;
            }
            if (logLevel >= level)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 记录日志：将普通日志转换为 ActionLog 并写入数据库；若状态对象本身是 ActionLog 则直接写入
        /// </summary>
        /// <typeparam name="TState">日志状态数据的类型</typeparam>
        /// <param name="logLevel">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="state">日志状态数据</param>
        /// <param name="exception">关联的异常对象，可为空</param>
        /// <param name="formatter">将状态数据格式化为文本的委托</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                ActionLog log = null;
                if (typeof(TState) != typeof(ActionLog))
                {
                    // 将日志级别映射为框架日志类型：Trace/Debug 映射为 Debug，Error/Warning/Critical 映射为 Exception，其余为 Normal
                    ActionLogTypesEnum ll = ActionLogTypesEnum.Normal;
                    if (logLevel == LogLevel.Trace || logLevel == LogLevel.Debug)
                    {
                        ll = ActionLogTypesEnum.Debug;
                    }
                    if (logLevel == LogLevel.Error || logLevel == LogLevel.Warning || logLevel == LogLevel.Critical)
                    {
                        ll = ActionLogTypesEnum.Exception;
                    }

                    log = new ActionLog
                    {
                        Remark = formatter?.Invoke(state, exception),
                        CreateTime = DateTime.Now,
                        ActionTime = DateTime.Now,
                        ActionName = "WtmLog",
                        ModuleName = "WtmLog",
                        LogType = ll
                    };
                }
                else
                {
                    log = state as ActionLog;
                }

                WTMContext wtm = null;
                // 优先使用当前 HTTP 请求中的 WTM 上下文，无 HTTP 上下文（如后台任务）时新建作用域获取
                var hc = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (hc == null)
                {
                    using (var scope = sp.CreateScope())
                    {
                        wtm = scope.ServiceProvider.GetRequiredService<WTMContext>();
                    }
                }
                else
                {
                    wtm = hc.RequestServices.GetRequiredService<WTMContext>();
                }
                if (wtm != null)
                {
                    try
                    {
                        // 创建用于记录日志的数据上下文并写入数据库，任何失败均静默忽略
                        using (var dc = wtm.CreateDC(true,logerror:false))
                        {
                            if (dc != null)
                            {
                                try
                                {
                                    log.TenantCode = wtm.LoginUserInfo?.CurrentTenant;
                                    dc.AddEntity<ActionLog>(log);
                                    dc.SaveChanges();
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 开启日志作用域，本实现不支持作用域，直接返回 null
        /// </summary>
        /// <typeparam name="TState">作用域状态数据的类型</typeparam>
        /// <param name="state">作用域状态数据</param>
        /// <returns>始终返回 null</returns>
        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
