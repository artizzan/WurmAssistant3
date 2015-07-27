using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Tests.Builders;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldurSoft.WurmApi.Tests.UnitTests.Events
{
    public class PublicEventInvokerTests : AssertionHelper
    {
        readonly ILogger logger = Mock.Create<ILogger>().RedirectToTraceOut();
        PublicEventInvoker invoker;
        int value = 0;

        [SetUp]
        public void Setup()
        {
            invoker = new PublicEventInvoker(new ThreadPoolMarshaller(logger), logger)
            {
                LoopDelayMillis = 1
            };

            Foovent += OnFoovent;
        }

        void OnFoovent(object sender, EventArgs eventArgs)
        {
            value++;
        }

        [TearDown]
        public void TearDown()
        {
            Foovent -= OnFoovent;
            value = 0;
            invoker.Dispose();
        }

        [Test]
        public void InvokesEvents()
        {
            var handle = invoker.Create(OnFoovent, TimeSpan.Zero);
            Expect(value, EqualTo(0));

            invoker.Trigger(handle);

            Thread.Sleep(10);
            Expect(value, EqualTo(1));
        }

        [Test]
        public void RespectsEventDelay()
        {
            var handle = invoker.Create(OnFoovent, TimeSpan.FromMilliseconds(20));
            Expect(value, EqualTo(0));

            invoker.Trigger(handle);

            Thread.Sleep(10);
            Expect(value, EqualTo(1));

            invoker.Trigger(handle);

            Thread.Sleep(5);
            Expect(value, EqualTo(1));

            Thread.Sleep(20);
            Expect(value, EqualTo(2));
        }

        [Test]
        public void BundlesMultipleSignals()
        {
            var handle = invoker.Create(OnFoovent, TimeSpan.FromMilliseconds(10));
            Expect(value, EqualTo(0));
            invoker.Trigger(handle);
            Thread.Sleep(10);
            Expect(value, EqualTo(1));
            invoker.Trigger(handle);
            invoker.Trigger(handle);
            invoker.Trigger(handle);

            Thread.Sleep(15);
            Expect(value, EqualTo(2));
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
