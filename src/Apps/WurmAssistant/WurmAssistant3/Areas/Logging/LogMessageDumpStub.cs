using System;
using NLog;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    class LogMessageDumpStub : ILogMessageDump
    {
        public void HandleEvent(LogLevel nlogLevel, string message, Exception exception, string category)
        {
        }
    }
}
