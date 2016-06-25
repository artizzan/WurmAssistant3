using System.Collections.Concurrent;
using System.Collections.Generic;
using AldursLab.Essentials.Extensions.DotNet.Collections.Concurrent;
using FluentAssertions;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Extensions.DotNet.Collections.Concurrent
{
    public class ConcurrentQueueExtensionsTests
    {
        public class DequeueAll : ConcurrentQueueExtensionsTests
        {
            [Test]
            public void WhenMultipleElements()
            {
                var queue = new ConcurrentQueue<int>();
                queue.Enqueue(2);
                queue.Enqueue(1);

                var result = queue.DequeueAll();
                result.Should().Equal(2, 1);
                queue.Should().BeEmpty();
            }

            [Test]
            public void WhenSingleElement()
            {
                var queue = new ConcurrentQueue<int>();
                queue.Enqueue(2);

                var result = queue.DequeueAll();
                result.Should().Equal(2);
                queue.Should().BeEmpty();
            }

            [Test]
            public void WhenNoElements()
            {
                var queue = new ConcurrentQueue<int>();

                var result = queue.DequeueAll();
                result.Should().BeEmpty();
                queue.Should().BeEmpty();
            }
        }
    }
}
