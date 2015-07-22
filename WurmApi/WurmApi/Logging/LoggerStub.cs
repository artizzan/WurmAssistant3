using System;

namespace AldurSoft.WurmApi.Logging
{
    public class LoggerStub : ILogger
    {
        public void Log(LogLevel level, string message, object source)
        {
        }

        public void Log(LogLevel level, string message, Exception exception, object source)
        {
        }
    }
}