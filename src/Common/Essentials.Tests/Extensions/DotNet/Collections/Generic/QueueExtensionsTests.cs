using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Extensions.DotNet.Collections.Generic
{
    public class QueueExtensionsTests
    {
        public class DequeueAll : QueueExtensionsTests
        {
            [Test]
            public void WhenMultipleElements()
            {
                var queue = new Queue<int>();
                queue.Enqueue(2);
                queue.Enqueue(1);

                var result = queue.DequeueAll();
                result.Should().Equal(2, 1);
                queue.Should().BeEmpty();
            }

            [Test]
            public void WhenSingleElement()
            {
                var queue = new Queue<int>();
                queue.Enqueue(2);

                var result = queue.DequeueAll();
                result.Should().Equal(2);
                queue.Should().BeEmpty();
            }

            [Test]
            public void WhenNoElements()
            {
                var queue = new Queue<int>();

                var result = queue.DequeueAll();
                result.Should().BeEmpty();
                queue.Should().BeEmpty();
            }
        }
    }
}
