using System;

namespace AldursLab.WurmApi
{
    public class WurmApiLoggerStub : IWurmApiLogger
    {
        public void Log(LogLevel level, string message, object source, Exception exception)
        {
        }
    }
}