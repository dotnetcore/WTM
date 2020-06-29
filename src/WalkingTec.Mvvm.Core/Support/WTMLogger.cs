using Microsoft.Extensions.Logging;
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
            return builder.AddProvider(new WTMLoggerProvider());
        }
    }

    public class WTMLoggerProvider : ILoggerProvider
    {
        private CS cs = null;
        public ILogger CreateLogger(string categoryName)
        {
            if (cs == null)
            {
                var config = GlobalServices.GetRequiredService<Configs>();
                cs = config.ConnectionStrings.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault();
                if (cs == null)
                {
                    cs = config.ConnectionStrings.Where(x => x.Key.ToLower() == "default").FirstOrDefault();
                }
                if (cs == null)
                {
                    cs = config.ConnectionStrings.FirstOrDefault();
                }
            }
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
            if (this.categoryName != "WalkingTec.Mvvm.Core.ActionLog")
            {
                return false;
            }
            var levels = GlobalServices.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();
            var l = levels.CurrentValue.Rules.Where(x => x.ProviderName == "WTM").Select(x => x.LogLevel).FirstOrDefault();
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
                if (cs != null)
                {
                    using (var dc = cs.CreateDC())
                    {
                        try
                        {
                            dc.AddEntity<ActionLog>(state as ActionLog);
                            dc.SaveChanges();
                        }
                        catch { }
                    }
                }
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
