using System;
using MyBills.Core;

namespace MyBills.Domain.Interfaces
{
    public interface ILogRepository
    {
        void WriteLog(LogLevel level, string method, string msg, Exception ex = null, string userName = null);
    }
}
