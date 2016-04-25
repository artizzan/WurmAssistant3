using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi.Extensions.DotNet;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Extensions
{
    public class DateTimeExtTests : AssertionHelper
    {
        public class AddDaysSnapToMinMaxTests : DateTimeExtTests
        {
            [Test]
            public void WhenLessThanMin_ShouldBeMin()
            {
                DateTimeOffset dt = DateTimeOffset.MinValue.AddDays(1);
                DateTimeOffset dt2 = dt.AddDaysSnapToMinMax(-2);
                Expect(dt2, EqualTo(DateTimeOffset.MinValue));
            }

            [Test]
            public void WhenMoreThanMax_ShouldBeMax()
            {
                DateTimeOffset dt = DateTimeOffset.MaxValue.AddDays(-1);
                DateTimeOffset dt2 = dt.AddDaysSnapToMinMax(2);
                Expect(dt2, EqualTo(DateTimeOffset.MaxValue));
            }

            [Test]
            public void WhenWithinMinMax_ShouldBeAdded()
            {
                DateTimeOffset dt = DateTimeOffset.MinValue.AddDays(3);
                DateTimeOffset dt2 = dt.AddDaysSnapToMinMax(-2);
                Expect(dt2, EqualTo(DateTimeOffset.MinValue.AddDays(1)));
            }
        }
    }
}
