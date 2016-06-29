using System;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    /// <summary>
    /// Skill entry parsing helper.
    /// </summary>
    public class SkillEntryParser
    {
        readonly IWurmApiLogger logger;

        public SkillEntryParser([NotNull] IWurmApi wurmApi) : this(wurmApi.Logger)
        {
        }

        public SkillEntryParser([NotNull] IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }

        /// <summary>
        /// Attempts to parse log entry into a skill gain information.
        /// Returns null if entry was not recognized as related to skill gains.
        /// </summary>
        /// <param name="wurmLogEntry"></param>
        /// <returns></returns>
        public SkillInfo TryParseSkillInfoFromLogLine(LogEntry wurmLogEntry)
        {
            if (wurmLogEntry.Content.Contains("increased") | wurmLogEntry.Content.Contains("decreased"))
            {
                if (wurmLogEntry.Content.EndsWith("affinity", StringComparison.InvariantCulture))
                {
                    logger.Log(LogLevel.Info,
                        "Skill message appears to inform about affinity, not supported. Raw entry: " + wurmLogEntry,
                        this,
                        null);
                    return null;
                }

                var match = Regex.Match(wurmLogEntry.Content,
                    @"^(.+) (?:increased|decreased) (.*) to (\d+(?:\,|\.)\d+|\d+).*$",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                string skillName;
                float? parsedLevel;
                float? parsedGain = null;
                if (match.Success)
                {
                    skillName = match.Groups[1].Value;

                    if (string.IsNullOrWhiteSpace(skillName))
                    {
                        logger.Log(LogLevel.Error,
                            "Skill name was parsed to empty string, raw entry: " + wurmLogEntry,
                            this,
                            null);
                        return null;
                    }

                    var rawSkillGain = match.Groups[2].Value;

                    // check if 'by xx.xx' can be present in this entry
                    rawSkillGain = rawSkillGain.Trim();
                    if (rawSkillGain.Length > 3 && rawSkillGain.StartsWith("by "))
                    {
                        //try parse the skill gain
                        parsedGain = TryParseFloatInvariant(rawSkillGain.Remove(0, 3).Trim());

                        if (parsedGain == null)
                        {
                            logger.Log(LogLevel.Error,
                                "Skill gain appears to be in log entry content, but could not be parsed, raw string: " + rawSkillGain
                                + " raw entry: " + wurmLogEntry,
                                this,
                                null);
                        }
                    }

                    var rawSkillLevel = match.Groups[3].Value;

                    parsedLevel = TryParseFloatInvariant(rawSkillLevel);

                    if (parsedLevel == null)
                    {
                        logger.Log(LogLevel.Error,
                            "Skill level could not be parsed, raw value: " + rawSkillLevel
                            + " raw entry: " + wurmLogEntry,
                            this,
                            null);
                        return null;
                    }
                }
                else
                {
                    logger.Log(LogLevel.Error,
                        "Skill gain/loss message could not be parsed, raw entry: " + wurmLogEntry,
                        this,
                        null);
                    return null;
                }

                return new SkillInfo(skillName, parsedLevel.Value, wurmLogEntry.Timestamp, parsedGain);
            }
            else
            {
                return null;
            }
        }

        public float? TryParseFloatInvariant(string text)
        {
            float? parsedLevel = null;
            float level;
            if (float.TryParse(
                text,
                System.Globalization.NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out level))
            {
                parsedLevel = level;
            }
            else if (float.TryParse(
                text.Replace(",", "."),
                System.Globalization.NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out level))
            {
                parsedLevel = level;
            }

            return parsedLevel;
        }
    }
}