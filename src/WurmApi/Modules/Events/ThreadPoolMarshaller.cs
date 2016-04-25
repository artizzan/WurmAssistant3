using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Events
{
    class ThreadPoolMarshaller : IWurmApiEventMarshaller
    {
        readonly IWurmApiLogger logger;

        public ThreadPoolMarshaller([NotNull] IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
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