using System;
using AldursLab.WurmApi;
using JetBrains.Annotations;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Infrastructure.Logging
{
    public class ThreadMarshalledLogMessageProcessor : ILogMessageProcessor
    {
        readonly IEventMarshaller eventMarshaller;
        readonly ILogMessageProcessor logMessageProcessor;

        public ThreadMarshalledLogMessageProcessor([NotNull] IEventMarshaller eventMarshaller,
            [NotNull] ILogMessageProcessor logMessageProcessor)
        {
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            if (logMessageProcessor == null) throw new ArgumentNullException("logMessageProcessor");
            this.eventMarshaller = eventMarshaller;
            this.logMessageProcessor = logMessageProcessor;
        }

        public void Handle(LogLevel level, string message, Exception exception)
        {
            eventMarshaller.Marshal(() => logMessageProcessor.Handle(level, message, exception));
        }
    }
}