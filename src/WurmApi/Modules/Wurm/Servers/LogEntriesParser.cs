using System;
using System.Text.RegularExpressions;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class LogEntriesParser
    {
        /// <summary>
        /// Null if entry is not applicable.
        /// </summary>
        public ServerDateStamped TryParseWurmDateTime(LogEntry wurmLogEntry)
        {
            //[16:24:19] It is 09:00:48 on day of the Wurm in week 4 of the Bear's starfall in the year of 1035.
            if (wurmLogEntry.Content.Contains("It is", StringComparison.InvariantCulture))
            {
                if (Regex.IsMatch(
                    wurmLogEntry.Content,
                    @"It is \d\d:\d\d:\d\d on .+ in week .+ in the year of \d+\.",
                    RegexOptions.Compiled))
                {

                    var wurmDateTime = TryCreateWurmDateTimeFromLogLine(wurmLogEntry.Content);
                    if (wurmDateTime == null)
                    {
                        return null;
                    }
                    var result = new ServerDateStamped()
                    {
                        WurmDateTime = wurmDateTime.Value,
                        Stamp = wurmLogEntry.Timestamp
                    };
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Null if entry is not applicable.
        /// </summary>
        public ServerUptimeStamped TryParseUptime(LogEntry wurmLogEntry)
        {
            if (wurmLogEntry.Content.Contains("The server has been up"))
            {
                var uptime = TryGetTimeSpanServerUpSince(wurmLogEntry.Content);
                if (uptime == null)
                {
                    return null;
                }
                var result = new ServerUptimeStamped()
                {
                    Uptime = uptime.Value,
                    Stamp = wurmLogEntry.Timestamp
                };
                return result;
            }
            return null;
        }

        static TimeSpan? TryGetTimeSpanServerUpSince(string logevent)
        {
            //EX:   The server has been up 1 days, 14 hours and 43 minutes.
            Match matchdays = Regex.Match(logevent, @"(\d\d*) days", RegexOptions.Compiled);
            int days = ParseMatchToInt32(matchdays);
            Match matchhours = Regex.Match(logevent, @"(\d\d*) hours", RegexOptions.Compiled);
            int hours = ParseMatchToInt32(matchhours);
            Match matchminutes = Regex.Match(logevent, @"(\d\d*) minutes", RegexOptions.Compiled);
            int minutes = ParseMatchToInt32(matchminutes);
            Match matchseconds = Regex.Match(logevent, @"(\d\d*) seconds", RegexOptions.Compiled);
            int seconds = ParseMatchToInt32(matchseconds);

            if (!matchdays.Success && !matchhours.Success && !matchminutes.Success && !matchseconds.Success)
            {
                return null;
            }

            return new TimeSpan(days, hours, minutes, seconds);
        }

        static int ParseMatchToInt32(Match match)
        {
            if (match.Success)
            {
                return Int32.Parse(match.Groups[1].Value);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Attempt to create WurmDateTime from wurm log line, throws exception on error
        /// </summary>
        /// <exception cref="WurmApiException">Parsing failed</exception>
        /// <param name="logLine"></param>
        private WurmDateTime? TryCreateWurmDateTimeFromLogLine(string logLine)
        {
            //[16:24:19] It is 09:00:48 on day of the Wurm in week 4 of the Bear's starfall in the year of 1035.

            //time
            Match wurmTime = Regex.Match(logLine, @" \d\d:\d\d:\d\d ", RegexOptions.Compiled);
            if (!wurmTime.Success)
            {
                return null;
            }
            int hour = Convert.ToInt32(wurmTime.Value.Substring(1, 2));
            int minute = Convert.ToInt32(wurmTime.Value.Substring(4, 2));
            int second = Convert.ToInt32(wurmTime.Value.Substring(7, 2));
            //day
            WurmDay? day = null;
            foreach (string name in WurmDay.AllNormalizedNames)
            {
                if (Regex.IsMatch(logLine, name, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    day = new WurmDay(name);
                    break;
                }
            }
            //week
            Match wurmWeek = Regex.Match(logLine, @"week (\d)", RegexOptions.Compiled);
            if (!wurmWeek.Success)
            {
                return null;
            }
            int week = Convert.ToInt32(wurmWeek.Groups[1].Value);
            //month(starfall)
            WurmStarfall? starfall = null;
            foreach (string name in WurmStarfall.AllNormalizedNames)
            {
                if (Regex.IsMatch(logLine, name, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    starfall = new WurmStarfall(name);
                    break;
                }
            }
            //year
            Match wurmYear = Regex.Match(logLine, @"in the year of (\d+)", RegexOptions.Compiled);
            if (!wurmYear.Success)
            {
                return null;
            }
            int year = Convert.ToInt32(wurmYear.Groups[1].Value);

            if (day == null || starfall == null)
            {
                return null;
            }

            return new WurmDateTime(year, starfall.Value, week, day.Value, hour, minute, second);
        }
    }
}