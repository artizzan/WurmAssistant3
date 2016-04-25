using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Events.Internal
{
    class InternalEventInvoker : IInternalEventInvoker, IDisposable
    {
        readonly IInternalEventAggregator eventAggregator;
        readonly IWurmApiLogger logger;
        readonly IWurmApiEventMarshaller eventMarshaller;

        readonly Task schedulingTask;
        volatile bool stop = false;

        readonly ConcurrentDictionary<InternalEvent, EventManager> events = new ConcurrentDictionary<InternalEvent, EventManager>(); 

        public InternalEventInvoker([NotNull] IInternalEventAggregator eventAggregator, [NotNull] IWurmApiLogger logger,
            [NotNull] IWurmApiEventMarshaller eventMarshaller)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (eventMarshaller == null) throw new ArgumentNullException(nameof(eventMarshaller));
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.eventMarshaller = eventMarshaller;

            LoopDelayMillis = 100;

            schedulingTask = new Task(RunSchedule, TaskCreationOptions.LongRunning);
            schedulingTask.Start();
        }

        public int LoopDelayMillis { get; set; }

        public InternalEvent Create(Func<Message> messageFactory)
        {
            return Create(messageFactory, TimeSpan.Zero);
        }

        public InternalEvent Create([NotNull] Func<Message> messageFactory, TimeSpan invocationMinDelay)
        {
            if (messageFactory == null)
                throw new ArgumentNullException(nameof(messageFactory));
            var e = new InternalEventImpl(this);
            events.TryAdd(e, new EventManager(e, invocationMinDelay, messageFactory));
            return e;
        }

        public void TriggerInstantly<TEventArgs>(EventHandler<TEventArgs> handler, object source, TEventArgs args) where TEventArgs : EventArgs
        {
            eventMarshaller.Marshal(() =>
            {
                try
                {
                    handler?.Invoke(source, args);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error,
                        "EventMarshaller has thrown an unhandled exception on instant handler invocation", this,
                        exception);
                }
            });
        }

        internal void Detach(InternalEvent internalEvent)
        {
            EventManager em;
            events.TryRemove(internalEvent, out em);
        }

        internal void Trigger(InternalEvent internalEvent)
        {
            EventManager em;
            if (events.TryGetValue(internalEvent, out em))
            {
                em.Pending = 1;
            }
        }

        public void Dispose()
        {
            stop = true;
            if (schedulingTask.Wait(10000))
            {
                schedulingTask.Dispose();
            }
            events.Clear();
        }

        ~InternalEventInvoker()
        {
            stop = true;
        }

        void RunSchedule()
        {
            while (true)
            {
                if (stop)
                    return;

                var toTrigger = events.Values.ToArray().Where(manager => manager.ShouldInvoke);
                foreach (var eventManager in toTrigger)
                {
                    var invoke = Interlocked.CompareExchange(ref eventManager.Pending, 0, 1) == 1;
                    if (invoke)
                    {
                        try
                        {
                            eventAggregator.Send(eventManager.MessageFactory());
                        }
                        catch (Exception exception)
                        {
                            logger.Log(LogLevel.Error,
                                "EventAggregator has thrown an unhandled exception on message sending", this,
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
            public EventManager([NotNull] InternalEvent internalEvent, TimeSpan betweenDelay, [NotNull] Func<Message> messageFactory)
            {
                if (internalEvent == null)
                    throw new ArgumentNullException(nameof(internalEvent));
                if (messageFactory == null)
                    throw new ArgumentNullException(nameof(messageFactory));
                InternalEvent = internalEvent;
                BetweenDelay = betweenDelay;
                MessageFactory = messageFactory;
            }

            public InternalEvent InternalEvent { get; private set; }
            public TimeSpan BetweenDelay { get; private set; }
            public Func<Message> MessageFactory { get; private set; }

            public volatile int Pending = 0;
            private DateTime lastInvoke;

            public DateTime LastInvoke
            {
                // locks are not needed, because read/writes are synchronous
                // uncomment if implementation changes
                get
                {
                    //lock (this)
                    {
                        return lastInvoke;
                    }
                }
                set
                {
                    //lock (this)
                    {
                        lastInvoke = value;
                    }
                }
            }

            public bool ShouldInvoke => Pending == 1
                                        && lastInvoke < DateTime.Now - BetweenDelay;
        }
    }
}
