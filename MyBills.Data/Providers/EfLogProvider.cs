using System;
using Microsoft.Extensions.Logging;
using MyBills.Data.Contexts;

namespace MyBills.Data.Providers
{
    /// <summary>
    /// A custom provider to log generated SQL calls from Entity Framework to the database
    /// </summary>
    public class EfLogProvider : ILoggerProvider
    {
        /// <summary>
        /// The logger factory.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new MyBillsLogger();
        }

        public void Dispose()
        { }

        private class MyBillsLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                SqlLog.LogSql(formatter(state, exception));
            }
            
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        } 
    }
}
