using System;
using MyBills.Core;
using MyBills.Data.Contexts;
using MyBills.Domain.Interfaces;

namespace MyBills.Data.Repositories
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogRepository"/> class.
    /// </summary>
    public class LogRepository : ILogRepository
    {
        /// <summary>
        /// Write a new log to the database
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="method">The source method</param>
        /// <param name="msg">The log message</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="userName">The username that created the message to log</param>
        public void WriteLog(LogLevel level, string method, string msg, Exception ex = null, string userName = null)
        {
            string stackTrace = null;
            if (ex != null)
            {
                stackTrace = ex.ToString();
            }

            using var ctx = new MyBillsContext();
            ctx.Log.Add(new Log
            {
                LogLevel = level,
                TimeStamp = DateTime.Now,
                CurrentMethod = method,
                ErrorMessage = msg,
                StackTrace = stackTrace,
                UserName = userName
            });

            ctx.SaveChanges();
        }
    }
}
