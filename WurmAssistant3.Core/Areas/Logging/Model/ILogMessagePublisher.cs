using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    /// <summary>
    /// Publishes messages to interested parties. All components interested in log events, should use this interface.
    /// </summary>
    public interface ILogMessagePublisher
    {
        int ErrorCount { get; }

        event EventHandler<EventArgs> ErrorCountChanged;

        event EventHandler<LogMessageEventArgs> EventLogged;
    }
}