using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;

using NUnit.Framework;

namespace AldurSoft.Core.Tests.Tests
{
    [TestFixture]
    public class TimeTests : FixtureBase
    {
        [Test]
        public void ReturnsCorrectTimes()
        {
            var now = DateTime.Now;
            Expect(Time.Clock.LocalNow, EqualTo(now).Within(TimeSpan.FromSeconds(1)));

            var nowUtc = DateTime.UtcNow;
            Expect(Time.Clock.UtcNow, EqualTo(nowUtc).Within(TimeSpan.FromSeconds(1)));

            var nowOffset = DateTimeOffset.Now;
            Expect(Time.Clock.LocalNowOffset, EqualTo(nowOffset).Within(TimeSpan.FromSeconds(1)));

            var nowOffsetUtc = DateTimeOffset.UtcNow;
            Expect(Time.Clock.UtcNowOffset, EqualTo(nowOffsetUtc).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void MocksTimes()
        {
            var nowMocked = DateTime.Now;
            var nowUtcMocked = DateTime.UtcNow;
            var nowOffsetMocked = DateTimeOffset.Now;
            var nowOffsetUtcMocked = DateTimeOffset.UtcNow;

            using (var scope = MockableClock.CreateScope())
            {
                scope.LocalNow = nowMocked;
                scope.UtcNow = nowUtcMocked;
                scope.LocalNowOffset = nowOffsetMocked;
                scope.UtcNowOffset = nowOffsetUtcMocked;
                Thread.Sleep(1);
                Expect(Time.Clock.LocalNow, EqualTo(nowMocked));
                Expect(Time.Clock.UtcNow, EqualTo(nowUtcMocked));
                Expect(Time.Clock.LocalNowOffset, EqualTo(nowOffsetMocked));
                Expect(Time.Clock.UtcNowOffset, EqualTo(nowOffsetUtcMocked));
            }

            // verify the scope was disposed
            ReturnsCorrectTimes();
        }
    }
}
