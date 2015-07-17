using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing.Tools;

using Newtonsoft.Json;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests
{
    [TestFixture]
    public class WurmDateTimeTests
    {
        [Test]
        public void CreatesWithProperValues()
        {
            var date = new WurmDateTime(1000, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 6, 8);
            Assert.AreEqual(1000, date.Year);
            Assert.AreEqual(WurmStarfall.Saw, date.Starfall);
            Assert.AreEqual(1, date.Week);
            Assert.AreEqual(WurmDay.Ant, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(6, date.Minute);
            Assert.AreEqual(8, date.Second);
            Assert.AreEqual(29, date.DayInYear);
            Assert.AreEqual(new TimeSpan(29, 12, 6, 8), date.DayAndTimeOfYear);
        }

        [Test]
        public void MinMaxHasCorrectValues()
        {
            Assert.AreEqual(new WurmDateTime(99999, 12, 4, 7, 23, 59, 59), WurmDateTime.MaxValue);
            Assert.AreEqual(new WurmDateTime(0, 1, 1, 1, 0, 0, 0), WurmDateTime.MinValue);
        }

        [Test]
        public void TimeToCalculatesCorrect()
        {
            var date = new WurmDateTime(1000, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 6, 8);
            Assert.AreEqual(TimeSpan.FromDays(336), date.TimeTo(new WurmDateTime(1001, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 6, 8)));
        }

        [Test]
        public void ToStringCorrectFormat()
        {
            var date = new WurmDateTime(1000, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 6, 8);
            Assert.AreEqual("12:06:08 on day of Ant in week 1 of the Saw starfall in the year of 1000", date.ToString());
        }

        [Test]
        public void SubtractOrAddExceedsLimit_SetsMinMaxValue()
        {
            var dateLow = new WurmDateTime(1, WurmStarfall.Diamond, 1, WurmDay.Ant, 1, 6, 8);
            Assert.AreEqual(WurmDateTime.MinValue, dateLow - TimeSpan.FromDays(500));
            var dateHigh = new WurmDateTime(99999, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 6, 8);
            Assert.AreEqual(WurmDateTime.MaxValue, dateHigh + TimeSpan.FromDays(900));
        }

        [Test]
        public void IsWithinCalculatesCorrect()
        {
            var date = new WurmDateTime(1000, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 6, 8);
            var datemin = new WurmDateTime(1000, WurmStarfall.Diamond, 1, WurmDay.Ant, 12, 6, 8);
            var datemax = new WurmDateTime(1000, WurmStarfall.Leaf, 1, WurmDay.Ant, 12, 6, 8);
            Assert.True(date.IsWithin(datemin, datemax));
            Assert.False(date.IsWithin(datemax, datemax));
            Assert.False(date.IsWithin(datemin, datemin));
        }

        [Test]
        public void NumericConstruction_ThrowsExceptionsOnBadValues()
        {
            Assert.Throws<ArgumentException>(() => new WurmDateTime(100000, 1, 1, 1, 1, 1, 1));
            Assert.Throws<ArgumentException>(() => new WurmDateTime(-1, 1, 1, 1, 1, 1, 1));
        }

        [Test]
        public void TimeSpanAdditionsCalculateCorrectDates()
        {
            var date = new WurmDateTime(1000, WurmStarfall.Diamond, 1, WurmDay.Ant, 12, 0, 0);

            var date2 = date + TimeSpan.FromDays(2);
            Assert.AreEqual(new WurmDateTime(1000, WurmStarfall.Diamond, 1, WurmDay.Wurm, 12, 0, 0), date2);

            var date3 = date + TimeSpan.FromDays(20);
            Assert.AreEqual(new WurmDateTime(1000, WurmStarfall.Diamond, 3, WurmDay.Awakening, 12, 0, 0), date3);

            var date4 = date + TimeSpan.FromDays(28);
            Assert.AreEqual(new WurmDateTime(1000, WurmStarfall.Saw, 1, WurmDay.Ant, 12, 0, 0), date4);

            var date5 = date + TimeSpan.FromDays(336);
            Assert.AreEqual(new WurmDateTime(1001, WurmStarfall.Diamond, 1, WurmDay.Ant, 12, 0, 0), date5);

            var date6 = date - TimeSpan.FromDays(336);
            Assert.AreEqual(new WurmDateTime(999, WurmStarfall.Diamond, 1, WurmDay.Ant, 12, 0, 0), date6);
        }

        [Test]
        public void Equality()
        {
            // disable "Comparison made to same variable"
#pragma warning disable 1718
            var dateLower = new WurmDateTime(1000, WurmStarfall.Diamond, 3, WurmDay.Ant, 12, 0, 0);
            var dateHigher = new WurmDateTime(1003, WurmStarfall.Silent, 4, WurmDay.Awakening, 9, 56, 2);
            Assert.True(dateHigher > dateLower);
            Assert.True(dateLower < dateHigher);
            Assert.True(dateHigher != dateLower);
            // ReSharper disable once EqualExpressionComparison
            Assert.True(dateHigher == dateHigher);
            // ReSharper disable once EqualExpressionComparison
            Assert.True(dateHigher >= dateHigher);
            Assert.True(dateHigher >= dateLower);
            // ReSharper disable once EqualExpressionComparison
            Assert.True(dateHigher <= dateHigher);
            Assert.True(dateLower <= dateHigher);
            var dateLowerClone = new WurmDateTime(1000, WurmStarfall.Diamond, 3, WurmDay.Ant, 12, 0, 0);
            Assert.True(dateLower.Equals(dateLowerClone));
            Assert.True(dateLower.Equals((object)dateLowerClone));
            Assert.True(((object)dateLower).Equals((object)dateLowerClone));
            Assert.False(((object)dateLower).Equals(null));
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(((object)dateLower).Equals(new DateTime()));
            Assert.False((dateLower).Equals(null));
#pragma warning restore 1718
        }

        [Test]
        public void HashCodeMatchesPattern()
        {
            var date = new WurmDateTime(1000, WurmStarfall.Diamond, 3, WurmDay.Ant, 12, 0, 0);
            Assert.AreEqual(date.TotalSeconds.GetHashCode(), date.GetHashCode());
        }

        [Test]
        public void Serialization_Binary()
        {
            WurmDateTime date = new WurmDateTime(1234, WurmStarfall.Fire, 2, WurmDay.Sleep, 2, 5, 8);

            var serializer = new BinaryTestSerializer<WurmDateTime>();

            var deserializedDate = serializer.Reserialize(date);

            Assert.AreEqual(date, deserializedDate);
        }

        [Test]
        public void Serialization_Json()
        {
            var serializer = new JsonTestSerializer<WurmDateTime>();

            WurmDateTime date = new WurmDateTime(1234, WurmStarfall.Fire, 2, WurmDay.Sleep, 2, 5, 8);

            var deserialized = serializer.Reserialize(date);

            Assert.AreEqual(date, deserialized);
        }
    }
}
