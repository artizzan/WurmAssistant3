using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldurSoft.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Logging
{
    class ErrorMonitoredLogger : ILogger, IDisposable
    {
        readonly ILogger logger;
        int errorCount;
        int warningCount;

        PublicEvent onErrorOrWarningLogged;

        public ErrorMonitoredLogger([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            onErrorOrWarningLogged = new PublicNullEvent();
        }

        public event EventHandler<EventArgs> ErrorOrWarningLogged;

        public int ErrorCount
        {
            get { return errorCount; }
            private set { errorCount = value; }
        }

        public int WarningCount
        {
            get { return warningCount; }
            private set { warningCount = value; }
        }

        public int TotalProblemCount
        {
            get { return ErrorCount + WarningCount; }
        }

        public void SetupEvents([NotNull] PublicEventInvoker publicEventInvoker)
        {
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            onErrorOrWarningLogged = publicEventInvoker.Create(() => ErrorOrWarningLogged.SafeInvoke(this), TimeSpan.Zero);
        }

        public void Log(LogLevel level, string message, object source, Exception exception)
        {
            if (level == LogLevel.Error || level == LogLevel.Fatal)
            {
                Interlocked.Increment(ref errorCount);
                onErrorOrWarningLogged.Trigger();
            }
            if (level == LogLevel.Warn)
            {
                Interlocked.Increment(ref warningCount);
                onErrorOrWarningLogged.Trigger();
            }
            logger.Log(level, message, source, exception);
        }

        public void Dispose()
        {
            ErrorOrWarningLogged = null;
            onErrorOrWarningLogged.Detach();
        }
    }
}
