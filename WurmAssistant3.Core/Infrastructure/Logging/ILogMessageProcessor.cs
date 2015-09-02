using System;
using JetBrains.Annotations;
using NLog;

namespace AldursLab.WurmAssistant3.Core.Infrastructure.Logging
{
    public interface ILogMessageProcessor
    {
        void Handle(LogLevel level, string message, [CanBeNull] Exception exception);
    }
}
