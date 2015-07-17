using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AldurSoft.WurmApi.Impl.WurmConfigsImpl
{
    class ConfigReader
    {
        private readonly WurmConfig config;

        public ConfigReader(WurmConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
        }

        public ReadResult ReadValues()
        {
            var readResult = new ReadResult();
            using (var fs = new FileStream(this.config.FullConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        this.ProcessLine(line, readResult);
                    }
                }
            }
            return readResult;
        }

        void ProcessLine(string line, ReadResult readResult)
        {
            // no, this is not a typo. :)
            if (line.Contains("custim_timer_source"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.CustomTimerSource = LogsLocation.ProfileFolder;
                else if (val == 1)
                    readResult.CustomTimerSource = LogsLocation.PlayerFolder;
            }
            else if (line.Contains("exec_source"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.ExecSource = LogsLocation.ProfileFolder;
                else if (val == 1)
                    readResult.ExecSource = LogsLocation.PlayerFolder;
            }
            else if (line.Contains("key_bindings_source"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.KeyBindSource = LogsLocation.ProfileFolder;
                else if (val == 1)
                    readResult.KeyBindSource = LogsLocation.PlayerFolder;
            }
            else if (line.Contains("auto_run_source"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.AutoRunSource = LogsLocation.ProfileFolder;
                else if (val == 1)
                    readResult.AutoRunSource = LogsLocation.PlayerFolder;
            }

            else if (line.Contains("irc_log_rotation"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.IrcLoggingType = LogSaveMode.Never;
                else if (val == 1)
                    readResult.IrcLoggingType = LogSaveMode.OneFile;
                else if (val == 2)
                    readResult.IrcLoggingType = LogSaveMode.Monthly;
                else if (val == 3)
                    readResult.IrcLoggingType = LogSaveMode.Daily;
            }
            else if (line.Contains("other_log_rotation"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.OtherLoggingType = LogSaveMode.Never;
                else if (val == 1)
                    readResult.OtherLoggingType = LogSaveMode.OneFile;
                else if (val == 2)
                    readResult.OtherLoggingType = LogSaveMode.Monthly;
                else if (val == 3)
                    readResult.OtherLoggingType = LogSaveMode.Daily;
            }
            else if (line.Contains("event_log_rotation"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.EventLoggingType = LogSaveMode.Never;
                else if (val == 1)
                    readResult.EventLoggingType = LogSaveMode.OneFile;
                else if (val == 2)
                    readResult.EventLoggingType = LogSaveMode.Monthly;
                else if (val == 3)
                    readResult.EventLoggingType = LogSaveMode.Daily;
            }
            else if (line.Contains("skillgain_minimum"))
            {
                int? val = this.ExtractSettingNumericValue(line);
                if (val == 0)
                    readResult.SkillGainRate = SkillGainRate.Never;
                else if (val == 1)
                    readResult.SkillGainRate = SkillGainRate.PerInteger;
                else if (val == 2)
                    readResult.SkillGainRate = SkillGainRate.Per0D1;
                else if (val == 3)
                    readResult.SkillGainRate = SkillGainRate.Per0D01;
                else if (val == 4)
                    readResult.SkillGainRate = SkillGainRate.Per0D001;
                else if (val == 5)
                    readResult.SkillGainRate = SkillGainRate.Always;
            }

            else if (line.Contains("skillgain_no_alignment"))
            {
                bool? val = this.ExtractBoolValue(line);
                if (val == true)
                    readResult.NoSkillMessageOnAlignmentChange = true;
                else if (val == false)
                    readResult.NoSkillMessageOnAlignmentChange = false;
            }
            else if (line.Contains("skillgain_no_favor"))
            {
                bool? val = this.ExtractBoolValue(line);
                if (val == true)
                    readResult.NoSkillMessageOnFavorChange = true;
                else if (val == false)
                    readResult.NoSkillMessageOnFavorChange = false;
            }
            else if (line.Contains("save_skills_on_quit"))
            {
                bool? val = this.ExtractBoolValue(line);
                if (val == true)
                    readResult.SaveSkillsOnQuit = true;
                else if (val == false)
                    readResult.SaveSkillsOnQuit = false;
            }
            else if (line.Contains("setting_timestamps"))
            {
                bool? val = this.ExtractBoolValue(line);
                if (val == true)
                    readResult.TimestampMessages = true;
                else if (val == false)
                    readResult.TimestampMessages = false;
            }
        }

        int? ExtractSettingNumericValue(string line)
        {
            string settingString = Regex.Match(line, @"=(\d)").Groups[1].Value;
            return int.Parse(settingString);
        }

        bool? ExtractBoolValue(string line)
        {
            string settingString = Regex.Match(line, @"=(\w+)").Groups[1].Value;
            return bool.Parse(settingString);
        }

        public class ReadResult
        {
            public LogsLocation CustomTimerSource { get; set; }
            public LogsLocation ExecSource { get; set; }
            public LogsLocation KeyBindSource { get; set; }
            public LogsLocation AutoRunSource { get; set; }
            public LogSaveMode IrcLoggingType { get; set; }
            public LogSaveMode OtherLoggingType { get; set; }
            public LogSaveMode EventLoggingType { get; set; }
            public SkillGainRate SkillGainRate { get; set; }
            public bool? NoSkillMessageOnAlignmentChange { get; set; }
            public bool? NoSkillMessageOnFavorChange { get; set; }
            public bool? SaveSkillsOnQuit { get; set; }
            public bool? TimestampMessages { get; set; }
        }
    }
}