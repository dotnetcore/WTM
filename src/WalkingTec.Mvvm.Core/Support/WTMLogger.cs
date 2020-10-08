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
        private CS cs = null;
        private LoggerFilterOptions logConfig;

        public WTMLoggerProvider(IOptions<Configs> _configs, IOptions<LoggerFilterOptions> _logConfig)
        {
            if (_configs.Value != null)
            {
                cs = _configs.Value.ConnectionStrings.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault();
                if (cs == null)
                {
                    cs = _configs.Value.ConnectionStrings.Where(x => x.Key.ToLower() == "default").FirstOrDefault();
                }
                if (cs == null)
                {
                    cs = _configs.Value.ConnectionStrings.FirstOrDefault();
                }
                logConfig = _logConfig.Value;
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new WTMLogger(categoryName, cs, logConfig);
        }
        public void Dispose() { }
    }

    public class WTMLogger : ILogger
    {
        private readonly string categoryName;
        private CS cs;
        private LoggerFilterOptions logConfig;

        public WTMLogger(string categoryName, CS cs, LoggerFilterOptions logConfig)
        {
            this.categoryName = categoryName;
            this.cs = cs;
            this.logConfig = logConfig;
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
                level = LogLevel.Error;
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

                if (cs != null)
                {
                    using var dc = cs.CreateDC();
                    if (dc != null)
                    {
                        dc.AddEntity<ActionLog>(log);
                        dc.SaveChanges();
                    }
                }
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
