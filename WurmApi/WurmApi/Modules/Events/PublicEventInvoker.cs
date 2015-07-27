using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Events
{
    class PublicEventInvoker : IPublicEventInvoker, IDisposable
    {
        readonly IEventMarshaller eventMarshaller;
        readonly ILogger logger;
        readonly Task schedulingTask;
        volatile bool stop = false;

        readonly ConcurrentDictionary<PublicEvent, EventManager> events = new ConcurrentDictionary<PublicEvent, EventManager>(); 

        public PublicEventInvoker([NotNull] IEventMarshaller eventMarshaller, [NotNull] ILogger logger)
        {
            if (eventMarshaller == null) throw new ArgumentNullException("eventMarshaller");
            if (logger == null) throw new ArgumentNullException("logger");
            this.eventMarshaller = eventMarshaller;
            this.logger = logger;

            LoopDelayMillis = 500;

            schedulingTask = new Task(RunSchedule, TaskCreationOptions.LongRunning);
            schedulingTask.Start();
        }

        public int LoopDelayMillis { get; set; }

        public PublicEvent Create([NotNull] Action action, TimeSpan invocationMinDelay)
        {
            if (action == null) throw new ArgumentNullException("action");
            var e = new PublicEvent(action);
            events.TryAdd(e,
                new EventManager() {PublicEvent = e, BetweenDelay = invocationMinDelay});
            return e;
        }

        public void Clear(PublicEvent publicEvent)
        {
            EventManager em;
            events.TryRemove(publicEvent, out em);
        }

        public void Signal(PublicEvent publicEvent)
        {
            EventManager em;
            if (events.TryGetValue(publicEvent, out em))
            {
                em.Pending = 1;
            }
        }

        public void Dispose()
        {
            stop = true;
            schedulingTask.Wait();
            schedulingTask.Dispose();
        }

        void RunSchedule()
        {
            while (true)
            {
                if (stop) return;

                var toTrigger = events.Values.ToArray().Where(manager => manager.ShouldInvoke);
                foreach (var eventManager in toTrigger)
                {
                    var invoke = Interlocked.CompareExchange(ref eventManager.Pending, 0, 1) == 1;
                    if (invoke)
                    {
                        try
                        {
                            eventMarshaller.Marshal(eventManager.PublicEvent.Action);
                        }
                        catch (Exception exception)
                        {
                            logger.Log(LogLevel.Error,
                                "EventMarshaller has thrown an unhandled exception on handler invocation", this,
                                exception);
                        }
                        eventManager.LastInvoke = DateTime.Now;
                    }
                }

                Thread.Sleep(LoopDelayMillis);
            }
        }

        class EventManager
        {
            public PublicEvent PublicEvent;
            public TimeSpan BetweenDelay;

            public volatile int Pending = 0;
            public DateTime LastInvoke;

            public bool ShouldInvoke
            {
                get
                {
                    return Pending == 1
                           && LastInvoke < DateTime.Now - BetweenDelay;
                }
            }
        }
    }
}