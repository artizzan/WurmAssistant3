using System;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class DateTimeExt
    {
        /// <summary>
        /// Checks if DateTime represents a moment in time, that is today, based on local time.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == Time.Get.LocalNow.Date;
        }

        public static string FormatForFileNameUniversal(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("DateTime with DateTimeKind.Unspecified is not supported");
            }
            return dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff-K", CultureInfo.InvariantCulture).Replace(":", "-");
        }

        public static string FormatForFileNameUniversal(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff-K", CultureInfo.InvariantCulture).Replace(":", "-");
        }

        public static DateTimeOffset ParseFromFileNameUniversal(FileInfo fileInfo)
        {
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var match = Regex.Match(fileName,
                @"(\d\d\d\d)-(\d\d)-(\d\d)-(\d\d)-(\d\d)-(\d\d)-(\d\d\d)-(.*)",
                RegexOptions.Compiled);

            if (!match.Success)
            {
                throw new ArgumentException(
                    string.Format("Could not parse source into DateTimeOffset, parsed file name: {0} ; source: {1}",
                        fileName,
                        fileInfo.FullName));
            }

            if (!match.Groups[8].Success || string.IsNullOrEmpty(match.Groups[8].Value))
            {
                throw new ArgumentException(
                    string.Format("Could not parse timezone offset, parsed file name: {0} ; source: {1}",
                        fileName,
                        fileInfo.FullName));
            }

            var offsetSign = match.Groups[8].Value[0] == '-' ? -1 : 1;
            var offsetValue = match.Groups[8].Value.Substring(1).Replace("-", ".");
            var offset =
                TimeSpan.FromHours((double) (decimal.Parse(offsetValue, CultureInfo.InvariantCulture)*offsetSign));

            return new DateTimeOffset(
                int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[6].Value, CultureInfo.InvariantCulture),
                int.Parse(match.Groups[7].Value, CultureInfo.InvariantCulture),
                offset
                );
        }

        public static DateTimeOffset AddDaysSnapToMinMax(this DateTimeOffset dateTimeOffset, double days)
        {
            if (Math.Sign(days) >= 0)
            {
                double maxDaysToAdd = (DateTimeOffset.MaxValue - dateTimeOffset).TotalDays;
                if (maxDaysToAdd >= days)
                {
                    return dateTimeOffset.AddDays(days);
                }
                else
                {
                    return DateTimeOffset.MaxValue;
                }
                
            }
            else
            {
                double minDaysToAdd = (DateTimeOffset.MinValue - dateTimeOffset).TotalDays;
                if (minDaysToAdd <= days)
                {
                    return dateTimeOffset.AddDays(days);
                }
                else
                {
                    return DateTimeOffset.MinValue;
                }
            }
        }
    }
}