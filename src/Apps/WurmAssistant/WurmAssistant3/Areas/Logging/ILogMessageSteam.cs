using System;
using System.Collections.Generic;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    public interface ILogMessageSteam
    {
        int ErrorCount { get; }
        int WarnCount { get; }

        event EventHandler<EventArgs> ErrorCountChanged;

        event EventHandler<LogMessageEventArgs> EventLogged;

        IEnumerable<LogMessageEventArgs> ConsumeMissedMessages();

    }
}