using System;
using System.Threading.Tasks;
using AldursLab.Essentials.Eventing;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Eventing.EventBusTests
{
    [TestFixture]
    public class UnitTests : AssertionHelper
    {
        MessageBus messageBus;

        [SetUp]
        public void Setup()
        {
            messageBus = new MessageBus();
        }

        [Test]
        public void GivenTwoSubscribers_ShouldBeHandledInBoth()
        {
            var sub1 = new Subscriber();
            var sub2 = new Subscriber();
            messageBus.Subscribe(sub1);
            messageBus.Subscribe(sub2);
            var message = new EmptyMessage();
            messageBus.Publish(message);
            Expect(sub1.HandledMessage, EqualTo(message));
            Expect(sub2.HandledMessage, EqualTo(message));
        }

        [Test]
        public void GivenExceptionInFirstHandled_SecondHandlerExecutes_ExceptionIsGloballyHandled()
        {
            MessageBus.GlobalExceptionHandler = exception =>
            {
                var isMatch = (exception as EvilException) != null;
                return isMatch;
            };

            var sub1 = new EvilSubscriber();
            var sub2 = new Subscriber();
            messageBus.Subscribe(sub1);
            messageBus.Subscribe(sub2);
            var message = new EmptyMessage();
            messageBus.Publish(message);
            Expect(sub2.HandledMessage, EqualTo(message));
        }

        [Test]
        [Timeout(500)]
        public async Task CanHandleWithAsyncVoidMethod()
        {
            var sub = new AsyncSubscriber();
            messageBus.Subscribe(sub);
            var message = new EmptyMessage();
            EmptyMessage handledMessage = null;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            sub.MessageHandled += (sender, args) =>
            {
                handledMessage = sub.HandledMessage;
                tcs.SetResult(true);
            };
            messageBus.Publish(message);
            await tcs.Task;
            Expect(handledMessage, EqualTo(message));
        }

        class Subscriber : IHandle<EmptyMessage>
        {
            public EmptyMessage HandledMessage { get; private set; }
            public void Handle(EmptyMessage message)
            {
                HandledMessage = message;
            }
        }

        class EvilSubscriber : IHandle<EmptyMessage>
        {
            public void Handle(EmptyMessage message)
            {
                throw new EvilException();
            }
        }

        class AsyncSubscriber : IHandle<EmptyMessage>
        {
            public EmptyMessage HandledMessage { get; private set; }
            public event EventHandler<EventArgs> MessageHandled;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            // intended to be async for testing purposes
            public async void Handle(EmptyMessage message)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                HandledMessage = message;
                OnMessageHandled();
            }

            protected virtual void OnMessageHandled()
            {
                var handler = MessageHandled;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        class EvilException : Exception {}

        class EmptyMessage{}
    }
}
