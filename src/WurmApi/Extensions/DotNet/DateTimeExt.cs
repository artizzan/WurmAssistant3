using System;

namespace AldursLab.WurmApi.Extensions.DotNet
{
    static class DateTimeExt
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

        public static DateTime AddConstrain(this DateTime dateTime, TimeSpan timeSpan)
        {
            try
            {
                return dateTime.Add(timeSpan);
            }
            catch (ArgumentOutOfRangeException)
            {
                var tsSign = Math.Sign(timeSpan.Ticks);
                if (tsSign > 0)
                {
                    return DateTime.MaxValue;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        public static DateTime SubtractConstrain(this DateTime dateTime, TimeSpan timeSpan)
        {
            return AddConstrain(dateTime, -timeSpan);
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