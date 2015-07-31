using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Events
{
    class ThreadPoolMarshaller : IEventMarshaller
    {
        readonly ILogger logger;

        public ThreadPoolMarshaller([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public void Marshal(Action action)
        {
            Task.Factory.StartNew(action)
                .ContinueWith(
                    task =>
                        logger.Log(LogLevel.Error, "Event handler has caused an unhandled exception", "WurmApi",
                            task.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}