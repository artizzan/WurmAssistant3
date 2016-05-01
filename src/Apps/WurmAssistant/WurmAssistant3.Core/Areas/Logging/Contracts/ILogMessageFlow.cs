using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts
{
    public interface ILogMessageFlow
    {
        int ErrorCount { get; }
        int WarnCount { get; }

        event EventHandler<EventArgs> ErrorCountChanged;

        event EventHandler<LogMessageEventArgs> EventLogged;
    }
}