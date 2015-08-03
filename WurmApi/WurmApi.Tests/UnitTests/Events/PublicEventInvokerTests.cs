using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Tests.Builders;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.UnitTests.Events
{
    public class PublicEventInvokerTests : AssertionHelper
    {
        readonly ILogger logger = Mock.Create<ILogger>().RedirectToTraceOut();
        PublicEventInvoker invoker;
        EventAwaiter<EventArgs> eventAwaiter;
            
        [SetUp]
        public void Setup()
        {
            invoker = new PublicEventInvoker(new ThreadPoolMarshaller(logger), logger)
            {
                LoopDelayMillis = 1
            };

            eventAwaiter = new EventAwaiter<EventArgs>();
            Foovent += eventAwaiter.Handle;
        }

        [TearDown]
        public void TearDown()
        {
            invoker.Dispose();
        }

        [Test]
        public void InvokesEvents()
        {
            var handle = invoker.Create(OnFoovent, TimeSpan.Zero);
            Expect(eventAwaiter.Invocations.Count(), EqualTo(0));

            invoker.Trigger(handle);

            eventAwaiter.WaitInvocations(1);
        }

        [Test]
        public void RespectsEventDelay()
        {
            var handle = invoker.Create(OnFoovent, TimeSpan.FromMilliseconds(500));
            Expect(eventAwaiter.Invocations.Count(), EqualTo(0));

            invoker.Trigger(handle);

            eventAwaiter.WaitInvocations(1);

            invoker.Trigger(handle);

            Thread.Sleep(5);
            Expect(eventAwaiter.Invocations.Count(), EqualTo(1));

            eventAwaiter.WaitInvocations(2);
        }

        [Test]
        public void BundlesMultipleSignals()
        {
            var handle = invoker.Create(OnFoovent, TimeSpan.FromMilliseconds(500));
            Expect(eventAwaiter.Invocations.Count(), EqualTo(0));
            invoker.Trigger(handle);

            eventAwaiter.WaitInvocations(1);

            invoker.Trigger(handle);
            invoker.Trigger(handle);
            invoker.Trigger(handle);

            Thread.Sleep(20);
            Expect(eventAwaiter.Invocations.Count(), EqualTo(1));

            eventAwaiter.WaitInvocations(2);

            Thread.Sleep(500);

            Expect(eventAwaiter.Invocations.Count(), EqualTo(2));
        }

        public event EventHandler<EventArgs> Foovent;

        public virtual void OnFoovent()
        {
            var handler = Foovent;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
