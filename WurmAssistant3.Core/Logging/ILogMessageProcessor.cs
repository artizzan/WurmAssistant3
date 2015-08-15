using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;

namespace AldursLab.WurmAssistant3.Core.Logging
{
    public interface ILogMessageProcessor
    {
        void Handle(LogLevel level, string message, [CanBeNull] Exception exception);
    }
}
