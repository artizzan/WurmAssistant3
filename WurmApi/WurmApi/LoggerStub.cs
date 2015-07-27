using System;

namespace AldurSoft.WurmApi
{
    public class LoggerStub : ILogger
    {
        public void Log(LogLevel level, string message, object source, Exception exception)
        {
        }
    }
}