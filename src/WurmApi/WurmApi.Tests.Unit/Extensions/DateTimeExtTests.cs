using System;
using AldursLab.WurmApi.Extensions.DotNet;
using FluentAssertions;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Unit.Extensions
{
    public class DateTimeExtTests
    {
        public class AddDaysSnapToMinMaxTests : DateTimeExtTests
        {
            [Test]
            public void WhenLessThanMin_ShouldBeMin()
            {
                DateTimeOffset dt = DateTimeOffset.MinValue.AddDays(1);
                DateTimeOffset dt2 = dt.AddDaysSnapToMinMax(-2);

                dt2.Should().Be(DateTimeOffset.MinValue);
            }

            [Test]
            public void WhenMoreThanMax_ShouldBeMax()
            {
                DateTimeOffset dt = DateTimeOffset.MaxValue.AddDays(-1);
                DateTimeOffset dt2 = dt.AddDaysSnapToMinMax(2);

                dt2.Should().Be(DateTimeOffset.MaxValue);
            }

            [Test]
            public void WhenWithinMinMax_ShouldBeAdded()
            {
                DateTimeOffset dt = DateTimeOffset.MinValue.AddDays(3);
                DateTimeOffset dt2 = dt.AddDaysSnapToMinMax(-2);

                dt2.Should().Be(DateTimeOffset.MinValue.AddDays(1));
            }
        }

        public class AddSafeTests : DateTimeExtTests
        {
            [Test]
            public void AddSafe_WhenNoOverflow_ReturnsResult()
            {
                var dt = new DateTime(DateTime.MaxValue.Ticks / 2);
                var controlDt = dt + TimeSpan.FromDays(1);
                dt = dt.AddConstrain(TimeSpan.FromDays(1));

                dt.Should().Be(controlDt);
            }

            [Test]
            public void AddSafe_WhenOverflowNegativeReturnsMin()
            {
                var dt = new DateTime(DateTime.MaxValue.Ticks / 2);
                dt = dt.AddConstrain(TimeSpan.MaxValue);
                
                dt.Should().Be(DateTime.MaxValue);
            }

            [Test]
            public void AddSafe_WhenOverflowPositive_ReturnsMax()
            {
                var dt = new DateTime(DateTime.MaxValue.Ticks / 2);
                dt = dt.AddConstrain(-TimeSpan.MaxValue);

                dt.Should().Be(DateTime.MinValue);
            }
        }
    }
}
