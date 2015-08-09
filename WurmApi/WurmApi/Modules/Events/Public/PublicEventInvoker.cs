using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Events.Public
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

            LoopDelayMillis = 100;

            schedulingTask = new Task(RunSchedule, TaskCreationOptions.LongRunning);
            schedulingTask.Start();
        }

        public int LoopDelayMillis { get; set; }

        public PublicEvent Create([NotNull] Action action, TimeSpan invocationMinDelay)
        {
            if (action == null) throw new ArgumentNullException("action");
            var e = new PublicEventImpl(this);
            events.TryAdd(e, new EventManager(e, invocationMinDelay, action));
            return e;
        }

        internal void Detach(PublicEvent publicEvent)
        {
            EventManager em;
            events.TryRemove(publicEvent, out em);
        }

        internal void Trigger(PublicEvent publicEvent)
        {
            EventManager em;
            if (events.TryGetValue(publicEvent, out em))
            {
                em.Pending = 1;
            }
        }

        public void TriggerInstantly<TEventArgs>(EventHandler<TEventArgs> handler, object source, TEventArgs args) where TEventArgs : EventArgs
        {
            eventMarshaller.Marshal(() =>
            {
                try
                {
                    if (handler != null)
                        handler(source, args);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error,
                        "EventMarshaller has thrown an unhandled exception on instant handler invocation", this,
                        exception);
                }
            });
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

        ~PublicEventInvoker()
        {
            stop = true;
        }

        void RunSchedule()
        {
            while (true)
            {
                //Trace.WriteLine("public event invoker schedule: " + DateTime.Now.ToString("O"));
                if (stop) return;

                var toTrigger = events.Values.ToArray().Where(manager => manager.ShouldInvoke);
                //Trace.WriteLine("to trigger count: " + toTrigger.Count());
                foreach (var eventManager in toTrigger)
                {
                    var invoke = Interlocked.CompareExchange(ref eventManager.Pending, 0, 1) == 1;
                    if (invoke)
                    {
                        //Trace.WriteLine("Invoking: " + eventManager.PublicEvent);
                        try
                        {
                            eventMarshaller.Marshal(eventManager.Action);
                            //Trace.WriteLine("Invoked: " + eventManager.PublicEvent);
                            //Trace.WriteLine(eventMarshaller.GetType().FullName);
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
            public EventManager([NotNull] PublicEvent publicEvent, TimeSpan betweenDelay, [NotNull] Action action)
            {
                if (publicEvent == null) throw new ArgumentNullException("publicEvent");
                if (action == null) throw new ArgumentNullException("action");
                PublicEvent = publicEvent;
                BetweenDelay = betweenDelay;
                Action = action;
            }

            public PublicEvent PublicEvent { get; private set; }
            public TimeSpan BetweenDelay { get; private set; }
            public Action Action { get; private set; }

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

            public bool ShouldInvoke
            {
                get
                {
                    return Pending == 1
                           && lastInvoke < DateTime.Now - BetweenDelay;
                }
            }
        }

        public string GetEventInfoString(PublicEventImpl publicEventImpl)
        {
            EventManager em;
            events.TryGetValue(publicEventImpl, out em);
            return em != null ? em.Action.MethodInformationToString() : "NULL";
        }
    }
}