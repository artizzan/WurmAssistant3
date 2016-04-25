using System;
using AldursLab.WurmApi.Extensions.DotNet;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Extensions
{
    class DateTimeExtensionsTests : AssertionHelper
    {
        [Test]
        public void AddSafe_WhenNoOverflow_ReturnsResult()
        {
            var dt = new DateTime(DateTime.MaxValue.Ticks / 2);
            var controlDt = dt + TimeSpan.FromDays(1);
            dt = dt.AddConstrain(TimeSpan.FromDays(1));
            Expect(dt, EqualTo(controlDt));
        }

        [Test]
        public void AddSafe_WhenOverflowNegativeReturnsMin()
        {
            var dt = new DateTime(DateTime.MaxValue.Ticks / 2);
            dt = dt.AddConstrain(TimeSpan.MaxValue);
            Expect(dt, EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void AddSafe_WhenOverflowPositive_ReturnsMax()
        {
            var dt = new DateTime(DateTime.MaxValue.Ticks / 2);
            dt = dt.AddConstrain(-TimeSpan.MaxValue);
            Expect(dt, EqualTo(DateTime.MinValue));
        }
    }
}
