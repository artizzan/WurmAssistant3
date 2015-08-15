using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Logging
{
    public class LoggerFactory
    {
        readonly ILogMessageProcessor logMessageProcessor;
        readonly IEventMarshaller eventMarshaller;
        readonly ThreadMarshalledLogMessageProcessor threadMarshalledLogMessageProcessor;

        public LoggerFactory([NotNull] ILogMessageProcessor logMessageProcessor,
            [NotNull] IEventMarshaller eventMarshaller)
        {
            if (logMessageProcessor == null) throw new ArgumentNullException("logMessageProcessor");
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            this.logMessageProcessor = logMessageProcessor;
            this.eventMarshaller = eventMarshaller;

            threadMarshalledLogMessageProcessor = new ThreadMarshalledLogMessageProcessor(eventMarshaller, logMessageProcessor);
        }

        public Logger Create(string category = null)
        {
            return new Logger(logMessageProcessor, category);
        }

        public Logger CreateWithGuiThreadMarshaller(string category = null)
        {
            return new Logger(threadMarshalledLogMessageProcessor, category);
        }
    }
}
