using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    /// <summary>
    /// Handles new log events. All logging appliances should forward log messages to this interface.
    /// </summary>
    public interface ILogMessageHandler
    {
        void HandleEvent(NLog.LogLevel nlogLevel, string message, Exception exception);
    }
}