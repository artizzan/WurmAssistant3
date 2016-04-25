using System;
using AldursLab.WurmApi.Extensions.DotNet;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Extensions
{
    class EventHandlerExtensionsTests : AssertionHelper
    {
        event EventHandler<EventArgs> Event1;
        event EventHandler<TestEventArgs> Event2;

        [Test]
        public void SafeInvoke_WhenExplicitArgs()
        {
            bool invoked = false;
            Event1 += (sender, args) => invoked = true;
            Event1.SafeInvoke(this, new EventArgs());
            Expect(invoked);
        }

        [Test]
        public void SafeInvoke_WhenNullArgs()
        {
            bool invoked = false;
            Event1 += (sender, args) => invoked = true;
            Event1.SafeInvoke(null, null);
            Expect(invoked);
        }

        [Test]
        public void SafeInvoke_WithCustomEventArgs_WhenExplicitArgs()
        {
            bool invoked = false;
            Event2 += (sender, args) => invoked = true;
            Event2.SafeInvoke(this, new TestEventArgs());
            Expect(invoked);
        }

        class TestEventArgs : EventArgs
        {}
    }
}
