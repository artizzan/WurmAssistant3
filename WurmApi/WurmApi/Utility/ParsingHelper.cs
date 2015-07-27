using System;
using System.Text.RegularExpressions;

namespace AldurSoft.WurmApi.Utility
{
    public class ParsingHelper
    {
        public LogFileDate GetDateFromLogFileName(string fileName)
        {
            LogFileDate logDate = TryGetDateFromLogFileName(fileName);
            if (logDate.LogSavingType == LogSavingType.Unknown)
            {
                throw new WurmApiException("Unable to parse log date from its file name: " + fileName);
            }
            return logDate;
        }

        /// <summary>
        /// Returns new LogFileDate(DateTime.MinValue, LogSavingType.Unknown)
        /// if parsing failed.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public LogFileDate TryGetDateFromLogFileName(string fileName)
        {
            Match matchDate = Regex.Match(fileName, @"(\d\d\d\d)-(\d\d)-(\d\d)");
            if (matchDate.Success)
            {
                return
                    new LogFileDate(
                        new DateTime(
                            Convert.ToInt32(matchDate.Groups[1].Value),
                            Convert.ToInt32(matchDate.Groups[2].Value),
                            Convert.ToInt32(matchDate.Groups[3].Value)),
                        LogSavingType.Daily);
            }
            else
            {
                Match matchDateMonthly = Regex.Match(fileName, @"(\d\d\d\d)-(\d\d)");
                if (matchDateMonthly.Success)
                {
                    return
                        new LogFileDate(
                            new DateTime(
                                Convert.ToInt32(matchDateMonthly.Groups[1].Value),
                                Convert.ToInt32(matchDateMonthly.Groups[2].Value),
                                1),
                            LogSavingType.Monthly);
                }
            }
            return new LogFileDate(DateTime.MinValue, LogSavingType.Unknown);
        }

        public DateTime GetDateFromLogFileLoggingStarted(string line)
        {
            //Logging started 2012-09-01
            Match matchDate = Regex.Match(line, @"(\d\d\d\d)-(\d\d)-(\d\d)");
            if (matchDate.Success)
            {
                return new DateTime(
                    Convert.ToInt32(matchDate.Groups[1].Value),
                    Convert.ToInt32(matchDate.Groups[2].Value),
                    Convert.ToInt32(matchDate.Groups[3].Value));
            }
            throw new WurmApiException("Unable to parse date from logging started message section: " + line);
        }

        public TimeSpan GetTimestampFromLogLine(string line)
        {
            try
            {
                return new TimeSpan(
                    Convert.ToInt32(line.Substring(1, 2)),
                    Convert.ToInt32(line.Substring(4, 2)),
                    Convert.ToInt32(line.Substring(7, 2)));
            }
            catch (Exception exception)
            {
                throw new WurmApiException("Parsing failed for line timestamp, line: " + line, exception);
            }
        }

        /// <summary>
        /// Returns parsed timestamp of a log event, or Timespan.MinValue, if parsing failed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public TimeSpan TryParseTimestampFromLogLine(string line)
        {
            try
            {
                return new TimeSpan(
                    Convert.ToInt32(line.Substring(1, 2)),
                    Convert.ToInt32(line.Substring(4, 2)),
                    Convert.ToInt32(line.Substring(7, 2)));
            }
            catch (Exception)
            {
                return TimeSpan.MinValue;
            }
        }

        public int GetDaysInMonthForLogFile(string fileName)
        {
            var date = GetDateFromLogFileName(fileName);
            if (date.LogSavingType != LogSavingType.Monthly)
            {
                throw new WurmApiException(
                    "Cannot get day count from non-monthly log file, actual file name: " + fileName);
            }
            return DateTime.DaysInMonth(date.DateTime.Year, date.DateTime.Month);
        }

        /// <summary>
        /// Compares two timespans that represent day time in 24-hour format and returns true if 
        /// they are potentially within one hour, with assumption that timespans can indicate 
        /// only time on SAME day.
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public bool AreMoreThanOneHourAppartOnSameDay(TimeSpan dt1, TimeSpan dt2)
        {
            double ts1TotalMinutes = dt1.TotalMinutes;
            double ts2TotalMinutes = dt2.TotalMinutes;

            return Math.Abs(ts1TotalMinutes - ts2TotalMinutes) > 60D;
        }

        /// <summary>
        /// Case sensitive.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsLoggingStartedLine(string line)
        {
            return line.StartsWith("Logging started");
        }

        /// <summary>
        /// String.Empty if nothing parsed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string TryParseSourceFromLogLine(string line)
        {
            var match = Regex.Match(line, @"^\[\d\d\:\d\d\:\d\d\] <(.+)>", RegexOptions.Compiled);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// String.Empty if nothing parsed.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string TryParseContentFromLogLine(string line)
        {
            Match match;

            match = Regex.Match(line, @"^\[\d\d\:\d\d\:\d\d\] <.+> (.+)", RegexOptions.Compiled | RegexOptions.Singleline);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            match = Regex.Match(line, @"^\[\d\d\:\d\d\:\d\d\] (.+)", RegexOptions.Compiled | RegexOptions.Singleline);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }

        public string TryParsePmRecipientFromFileName(string fileName)
        {
            try
            {
                string playername = fileName.Remove(0, 4);
                playername = playername.Remove(playername.IndexOf('.'));
                return playername;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}