using System;
using System.Collections.Generic;
using System.Threading;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Tests.Builders;
using NUnit.Framework;
using Telerik.JustMock;

namespace AldurSoft.WurmApi.Tests.UnitTests.Events
{
    public class InternalEventInvokerTests : AssertionHelper
    {
        readonly ILogger logger = Mock.Create<ILogger>().RedirectToTraceOut();
        InternalEventInvoker invoker;
        InternalEventAggregator eventAggregator;
        MessageReceiver receiver;
        readonly Func<Message> messageFactory = () => new TestMessage();
            
        [SetUp]
        public void Setup()
        {
            eventAggregator = new InternalEventAggregator();
            receiver = new MessageReceiver(eventAggregator);
            invoker = new InternalEventInvoker(eventAggregator, logger)
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
            Expect(receiver.Messages.Count, EqualTo(0));

            invoker.Trigger(handle);

            Thread.Sleep(20);
            Expect(receiver.Messages.Count, EqualTo(1));
        }

        [Test]
        public void RespectsEventDelay()
        {
            var handle = invoker.Create(messageFactory, TimeSpan.FromMilliseconds(20));
            Expect(receiver.Messages.Count, EqualTo(0));

            invoker.Trigger(handle);

            Thread.Sleep(10);
            Expect(receiver.Messages.Count, EqualTo(1));

            invoker.Trigger(handle);

            Thread.Sleep(5);
            Expect(receiver.Messages.Count, EqualTo(1));

            Thread.Sleep(20);
            Expect(receiver.Messages.Count, EqualTo(2));
        }

        [Test]
        public void BundlesMultipleSignals()
        {
            var handle = invoker.Create(messageFactory, TimeSpan.FromMilliseconds(10));
            Expect(receiver.Messages.Count, EqualTo(0));
            invoker.Trigger(handle);
            Thread.Sleep(10);
            Expect(receiver.Messages.Count, EqualTo(1));
            invoker.Trigger(handle);
            invoker.Trigger(handle);
            invoker.Trigger(handle);

            Thread.Sleep(15);
            Expect(receiver.Messages.Count, EqualTo(2));
        }

        class TestMessage : Message { }

        class MessageReceiver : IHandle<TestMessage>
        {
            public MessageReceiver(IInternalEventAggregator eventAggregator)
            {
                eventAggregator.Subscribe(this);
                Messages = new List<TestMessage>();
            }

            public List<TestMessage> Messages { get; private set; }

            public void Handle(TestMessage message)
            {
                Messages.Add(message);
            }
        }
    }
}