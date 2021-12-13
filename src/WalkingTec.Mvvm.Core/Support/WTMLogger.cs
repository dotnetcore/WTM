using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Core
{

    public static class WTMeLoggerExtensions
    {
        public static ILoggingBuilder AddWTMLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WTMLoggerProvider>());
            return builder;
        }
    }

    public class WTMLoggerProvider : ILoggerProvider
    {
        private IServiceProvider sp = null;
        private LoggerFilterOptions logConfig;

        public WTMLoggerProvider( IOptionsMonitor<LoggerFilterOptions> _logConfig, IServiceProvider sp)
        {
            this.sp = sp;
            logConfig = _logConfig.CurrentValue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new WTMLogger(categoryName, logConfig,sp);
        }
        public void Dispose() { }
    }

    public class WTMLogger : ILogger
    {
        private readonly string categoryName;
        private IServiceProvider sp;
        private LoggerFilterOptions logConfig;

        public WTMLogger(string categoryName, LoggerFilterOptions logConfig, IServiceProvider sp)
        {
            this.categoryName = categoryName;
            this.logConfig = logConfig;
            this.sp = sp;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if(logConfig == null)
            {
                return false;
            }
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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                ActionLog log = null;
                if (typeof(TState) != typeof(ActionLog))
                {
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
                    using (var dc = wtm.CreateDC(true))
                    {
                        if (dc != null)
                        {
                            try
                            {
                                dc.AddEntity<ActionLog>(log);
                                dc.SaveChanges();
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
