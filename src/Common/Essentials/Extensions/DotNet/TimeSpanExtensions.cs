using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Extensions.DotNet
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan StripMilliseconds(this TimeSpan timeSpan)
        {
            return new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        /// <summary>
        /// Multiplies by an integer value.
        /// </summary>
        public static TimeSpan Multiply(this TimeSpan multiplicand, int multiplier)
        {
            return TimeSpan.FromTicks(multiplicand.Ticks * multiplier);
        }

        /// <summary>
        /// Multiplies by a double value.
        /// </summary>
        public static TimeSpan Multiply(this TimeSpan multiplicand, double multiplier)
        {
            return TimeSpan.FromTicks((long)(multiplicand.Ticks * multiplier));
        }

        /// <summary>
        /// Converts timespan to string similar to: "9d", "3d 22h", "18h", "3h 22m", "22m"
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static string ToStringCompact(this TimeSpan timespan, CompactResolution resolution = CompactResolution.Seconds)
        {
            //todo unit test
            switch (resolution)
            {
                case CompactResolution.Seconds:
                    return ToStringCompactSecondsResolution(timespan);
                case CompactResolution.Minutes:
                    return ToStringCompactMinutesResolution(timespan);
                default:
                    Debug.Fail("unexpected CompactResolution " + resolution);
                    return ToStringCompactSecondsResolution(timespan);
            }
        }

        static string ToStringCompactSecondsResolution(TimeSpan timespan)
        {

            double totalMinutes = timespan.TotalMinutes;
            if (totalMinutes < 0)
            {
                return "unsupported";
            }
            else if (totalMinutes < 10)
            {
                return timespan.ToString("m'm 's's'");
            }
            else if (totalMinutes < 1 * 60)
            {
                return timespan.ToString("m'm'");
            }
            else if (totalMinutes < 6 * 60)
            {
                return timespan.ToString("h'h 'm'm'");
            }
            else if (totalMinutes < 24 * 60)
            {
                return timespan.ToString("h'h'");
            }
            else if (totalMinutes < 144 * 60)
            {
                return timespan.ToString("d'd 'h'h'");
            }
            else
            {
                return timespan.ToString("d'd'");
            }
        }

        static string ToStringCompactMinutesResolution(TimeSpan timespan)
        {
            double totalMinutes = timespan.TotalMinutes;
            if (totalMinutes < 0)
            {
                return "unsupported";
            }
            else if (totalMinutes < 1 * 60)
            {
                return timespan.ToString("m'm'");
            }
            else if (totalMinutes < 6 * 60)
            {
                return timespan.ToString("h'h 'm'm'");
            }
            else if (totalMinutes < 24 * 60)
            {
                return timespan.ToString("h'h'");
            }
            else if (totalMinutes < 144 * 60)
            {
                return timespan.ToString("d'd 'h'h'");
            }
            else
            {
                return timespan.ToString("d'd'");
            }
        }
    }

    public enum CompactResolution
    {
        Seconds = 0,
        Minutes
    }
}
