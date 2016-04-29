using System;
using AldursLab.WurmApi.Extensions.DotNet;
using FluentAssertions;
using Xunit;

namespace AldursLab.WurmApi.Tests.Unit.Extensions
{
    class EventHandlerExtensionsTests
    {
        event EventHandler<EventArgs> Event1;
        event EventHandler<TestEventArgs> Event2;

        [Fact]
        public void SafeInvoke_WhenExplicitArgs()
        {
            bool invoked = false;
            Event1 += (sender, args) => invoked = true;
            Event1.SafeInvoke(this, new EventArgs());
            invoked.Should().BeTrue();
        }

        [Fact]
        public void SafeInvoke_WhenNullArgs()
        {
            bool invoked = false;
            Event1 += (sender, args) => invoked = true;
            Event1.SafeInvoke(null, null);
            invoked.Should().BeTrue();
        }

        [Fact]
        public void SafeInvoke_WithCustomEventArgs_WhenExplicitArgs()
        {
            bool invoked = false;
            Event2 += (sender, args) => invoked = true;
            Event2.SafeInvoke(this, new TestEventArgs());
            invoked.Should().BeTrue();
        }

        class TestEventArgs : EventArgs
        {}
    }
}
