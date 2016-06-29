using System;
using NLog;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    /// <summary>
    /// Handles new log events. All logging appliances should forward log messages to this interface.
    /// </summary>
    public interface ILogMessageDump
    {
        void HandleEvent(LogLevel nlogLevel, string message, Exception exception, string category);
    }
}