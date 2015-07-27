using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AldurSoft.WurmApi.Modules.Wurm.Configs
{
    class ConfigWriter
    {
        private readonly WurmConfig config;

        public ConfigWriter(WurmConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
        }

        public void SetEventLoggingMode(LogSaveMode logSaveMode)
        {
            this.SetLoggingMode(logSaveMode, "event_log_rotation=");
        }

        public void SetOtherLoggingMode(LogSaveMode logSaveMode)
        {
            this.SetLoggingMode(logSaveMode, "other_log_rotation=");
        }

        public void SetIrcLoggingMode(LogSaveMode logSaveMode)
        {
            this.SetLoggingMode(logSaveMode, "irc_log_rotation=");
        }

        void SetLoggingMode(LogSaveMode logSaveMode, string replaceString)
        {
            if (logSaveMode == LogSaveMode.Unknown)
            {
                throw new InvalidOperationException("Unknown is not valid Wurm config value");
            }
            string replaceValue = replaceString;
            if (logSaveMode == LogSaveMode.Never)
            {
                replaceValue += "0";
            }
            if (logSaveMode == LogSaveMode.OneFile)
            {
                replaceValue += "1";
            }
            if (logSaveMode == LogSaveMode.Monthly)
            {
                replaceValue += "2";
            }
            if (logSaveMode == LogSaveMode.Daily)
            {
                replaceValue += "3";
            }
            this.RewriteFile(replaceString + @"\d", replaceValue);
        }

        public void SetSkillGainRate(SkillGainRate skillGainRate)
        {
            if (skillGainRate == SkillGainRate.Unknown)
            {
                throw new InvalidOperationException("Unknown is not valid Wurm config value");
            }
            string replacement = "skillgain_minimum=";
            if (skillGainRate == SkillGainRate.Never)
                replacement += "0";
            if (skillGainRate == SkillGainRate.PerInteger)
                replacement += "1";
            if (skillGainRate == SkillGainRate.Per0D1)
                replacement += "2";
            if (skillGainRate == SkillGainRate.Per0D01)
                replacement += "3";
            if (skillGainRate == SkillGainRate.Per0D001)
                replacement += "4";
            if (skillGainRate == SkillGainRate.Always)
                replacement += "5";
            this.RewriteFile(@"skillgain_minimum=\d", replacement);
        }

        public void SetNoSkillMessageOnAlignmentChange(bool? newValue)
        {
            if (newValue == null)
            {
                throw new InvalidOperationException("Null is not valid Wurm config value");
            }
            this.SetBooleanValue(newValue.Value, "skillgain_no_alignment=");
        }

        public void SetNoSkillMessageOnFavorChange(bool? newValue)
        {
            if (newValue == null)
            {
                throw new InvalidOperationException("Null is not valid Wurm config value");
            }
            this.SetBooleanValue(newValue.Value, "skillgain_no_favor=");
        }

        public void SetSaveSkillsOnQuit(bool? newValue)
        {
            if (newValue == null)
            {
                throw new InvalidOperationException("Null is not valid Wurm config value");
            }
            this.SetBooleanValue(newValue.Value, "save_skills_on_quit=");
        }

        public void SetTimestampMessages(bool? newValue)
        {
            if (newValue == null)
            {
                throw new InvalidOperationException("Null is not valid Wurm config value");
            }
            this.SetBooleanValue(newValue.Value, "setting_timestamps=");
        }

        void SetBooleanValue(bool newValue, string replacementString)
        {
            string replacement = replacementString;
            replacement += newValue ? "true" : "false";
            this.RewriteFile(replacementString + @"\w+", replacement);
        }

        void RewriteFile(string currentSettingRegex, string replacementSettingString)
        {
            string configFilePath = this.config.FullConfigFilePath;
            string configText;
            Encoding fileEncoding;
            //save encoding to ensure correct output
            using (FileStream fs = new FileStream(configFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fileEncoding = sr.CurrentEncoding;
                    configText = sr.ReadToEnd();
                }
            }

            //need to count replacements
            int replaceCount = 0;
            configText = Regex.Replace(
                configText,
                currentSettingRegex, m =>
                    {
                        replaceCount++;
                        return replacementSettingString;
                    }
                , RegexOptions.CultureInvariant);

            //if there were replacements, rewrite the file.
            //else use file appending and add option at the end of config
            bool append = replaceCount <= 0;
            using (FileStream fs = new FileStream(configFilePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs, fileEncoding))
                {
                    if (!append)
                        sw.Write(configText);
                    else
                    {
                        //trim end whitespace
                        configText = configText.TrimEnd(new char[] { ' ' });

                        //verify that the file ends with correct newline
                        //add newline if not
                        string lastTwoChars = configText.Trim().Substring(configText.Length - 2, 2);
                        if (lastTwoChars != "\r\n")
                            sw.Write("\r\n");
                        //write setting
                        sw.Write(replacementSettingString + "\r\n");
                    }
                }
            }
        }
    }
}