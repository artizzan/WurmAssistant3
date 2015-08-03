using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Tests.Builders;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace AldurSoft.WurmApi.Tests.UnitTests.Events
{
    public class InternalEventInvokerTests : AssertionHelper
    {
        readonly ILogger logger = Mock.Create<ILogger>().RedirectToTraceOut();
        InternalEventInvoker invoker;
        InternalEventAggregator eventAggregator;
        Subscriber<TestMessage> receiver;
        readonly Func<Message> messageFactory = () => new TestMessage();
            
        [SetUp]
        public void Setup()
        {
            eventAggregator = new InternalEventAggregator();
            receiver = new Subscriber<TestMessage>(eventAggregator);
            //receiver = new MessageReceiver(eventAggregator);
            invoker = new InternalEventInvoker(eventAggregator, logger, new ThreadPoolMarshaller(logger))
            {
                LoopDelayMillis = 1
            };
        }

        [TearDown]
        public void TearDown()
        {
            invoker.Dispose();
        }

        [Test]
        public void InvokesEvents()
        {
            var handle = invoker.Create(messageFactory, TimeSpan.Zero);
            
            Expect(receiver.ReceivedMessages.Count(), EqualTo(0));

            invoker.Trigger(handle);

            receiver.WaitMessages(1);
        }

        [Test]
        public void RespectsEventDelay()
        {
            var handle = invoker.Create(messageFactory, TimeSpan.FromMilliseconds(1000));
            Expect(receiver.ReceivedMessages.Count(), EqualTo(0));

            invoker.Trigger(handle);
            receiver.WaitMessages(1);

            invoker.Trigger(handle);
            Thread.Sleep(10);
            Expect(receiver.ReceivedMessages.Count(), EqualTo(1));

            receiver.WaitMessages(2);
        }

        [Test]
        public void BundlesMultipleSignals()
        {
            var handle = invoker.Create(messageFactory, TimeSpan.FromMilliseconds(1000));
            Expect(receiver.ReceivedMessages.Count(), EqualTo(0));
            invoker.Trigger(handle);
            receiver.WaitMessages(1);

            invoker.Trigger(handle);
            invoker.Trigger(handle);
            invoker.Trigger(handle);

            receiver.WaitMessages(2);
            Thread.Sleep(500);
            Expect(receiver.ReceivedMessages.Count(), EqualTo(2));
        }

        class TestMessage : Message { }
    }
}