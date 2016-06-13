using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using NLog;

namespace AldursLab.WurmAssistant3.Areas.Logging.Modules
{
    class LogMessageDumpStub : ILogMessageDump
    {
        public void HandleEvent(LogLevel nlogLevel, string message, Exception exception, string category)
        {
        }
    }
}
