using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
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

        public WTMLoggerProvider(IOptions<Configs> _configs)
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
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new WTMLogger(categoryName, cs);
        }
        public void Dispose() { }
    }

    public class WTMLogger : ILogger
    {
        private readonly string categoryName;
        private CS cs;

        public WTMLogger(string categoryName, CS cs)
        {
            this.categoryName = categoryName;
            this.cs = cs;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var levels = GlobalServices.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();
            var l = levels.CurrentValue.Rules.Where(x => x.ProviderName == "WTM" && (x.CategoryName != null && categoryName.ToLower().StartsWith(x.CategoryName.ToLower()))).Select(x => x.LogLevel).FirstOrDefault();
            if (l == null)
            {
                l = levels.CurrentValue.MinLevel;
            }
            if (logLevel >= l)
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
                        ActionName = this.categoryName,
                        ModuleName = "System",
                        LogType = ll
                    };
                }
                else
                {
                    log = state as ActionLog;
                }

                if (cs != null)
                {
                    using (var dc = cs.CreateDC())
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
