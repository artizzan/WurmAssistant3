using System;
using System.IO;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Extensions.DotNet
{
    public class DateTimeExtensionsTests : AssertionHelper
    {
        [Test]
        public void FormatForFileNameUniversal_ParseFromFileNameUniversal_LocalTime_DateTimeOvrl()
        {
            using (var tempDir = TempDirectoriesFactory.CreateEmpty())
            {
                var date = new DateTime(2015, 09, 16, 22, 45, 15, 456, DateTimeKind.Local);
                var dateAsOffset = new DateTimeOffset(date);

                var str = date.FormatForFileNameUniversal();
                var file = new FileInfo(Path.Combine(tempDir.FullName, str + ".txt"));
                File.WriteAllText(file.FullName, "test");
                var parsedDate = DateTimeExtensions.ParseFromFileNameUniversal(file);

                Expect(parsedDate, EqualTo(dateAsOffset));
            }
        }

        [Test]
        public void FormatForFileNameUniversal_ParseFromFileNameUniversal_LocalTime_DateTimeOffsetOvrl()
        {
            using (var tempDir = TempDirectoriesFactory.CreateEmpty())
            {
                var date = new DateTimeOffset(2015, 09, 16, 22, 45, 15, 456, TimeSpan.FromHours(4));

                var str = date.FormatForFileNameUniversal();
                var file = new FileInfo(Path.Combine(tempDir.FullName, str + ".txt"));
                File.WriteAllText(file.FullName, "test");
                var parsedDate = DateTimeExtensions.ParseFromFileNameUniversal(file);

                Expect(parsedDate, EqualTo(date));
            }
        }

        [Test]
        public void FormatForFileNameUniversal_ParseFromFileNameUniversal_LocalTime_ExtensionlessFile()
        {
            using (var tempDir = TempDirectoriesFactory.CreateEmpty())
            {
                var date = new DateTimeOffset(2015, 09, 16, 22, 45, 15, 456, TimeSpan.FromHours(-4));

                var str = date.FormatForFileNameUniversal();
                var file = new FileInfo(Path.Combine(tempDir.FullName, str));
                File.WriteAllText(file.FullName, "test");
                var parsedDate = DateTimeExtensions.ParseFromFileNameUniversal(file);

                Expect(parsedDate, EqualTo(date));
            }
        }

        [Test]
        public void FormatForFileNameUniversal_UnspecifiedTime_ShouldThrow()
        {
            var date = new DateTime(2015, 09, 16, 22, 45, 15, 456, DateTimeKind.Unspecified);
            Assert.Throws<ArgumentException>(() => date.FormatForFileNameUniversal());
        }

        public class AddDaysSnapToMinMaxTests : DateTimeExtensionsTests
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
