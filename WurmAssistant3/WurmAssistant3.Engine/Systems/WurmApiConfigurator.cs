using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AldurSoft.Core.Eventing;
using AldurSoft.WurmApi;
using AldurSoft.WurmAssistant3.Systems;

using JetBrains.Annotations;

using NLog;

namespace AldurSoft.WurmAssistant3.Engine.Systems
{
    public class WurmApiConfigurator : IWurmApiConfigurator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IWurmApi wurmApi;
        private readonly IWurmAssistantSettings settings;

        public WurmApiConfigurator([NotNull] IWurmApi wurmApi, [NotNull] IWurmAssistantSettings settings)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (settings == null) throw new ArgumentNullException("settings");
            this.wurmApi = wurmApi;
            this.settings = settings;
        }

        public void AppendTimeQueryToAutoruns()
        {
            wurmApi.WurmAutoruns.AppendCommandToAllAutoruns("say /uptime");
        }

        public void AppendUptimeQueryToAutoruns()
        {
            wurmApi.WurmAutoruns.AppendCommandToAllAutoruns("say /time");
        }

        public void SynchronizeWurmGameClientSettings()
        {
            foreach (var config in wurmApi.WurmConfigs.All.ToArray())
            {
                config.EventLoggingType = settings.Entity.WurmClientConfig.LogSaveMode;
                config.IrcLoggingType = settings.Entity.WurmClientConfig.LogSaveMode;
                config.OtherLoggingType = settings.Entity.WurmClientConfig.LogSaveMode;

                config.NoSkillMessageOnAlignmentChange = false;

                config.NoSkillMessageOnFavorChange = false;

                config.SkillGainRate = settings.Entity.WurmClientConfig.SkillGainRate;

                config.TimestampMessages = true;
            }
        }

        public WurmGameClientSettingsSyncState VerifyGameClientConfig()
        {
            var result = new WurmGameClientSettingsSyncState();

            foreach (var config in wurmApi.WurmConfigs.All.ToArray())
            {
                // detect serious issues:
                if (config.EventLoggingType != LogSaveMode.Daily && config.EventLoggingType != LogSaveMode.Monthly)
                {
                    result.Issues.Add(string.Format(
                                "Config {0} has unsupported EventLoggingType of {1}",
                                config.Name,
                                config.EventLoggingType));
                }
                if (config.OtherLoggingType != LogSaveMode.Daily && config.OtherLoggingType != LogSaveMode.Monthly)
                {
                    result.Issues.Add(
                        string.Format(
                            "Config {0} has unsupported OtherLoggingType of {1}",
                            config.Name,
                            config.OtherLoggingType));
                }

                if (config.NoSkillMessageOnAlignmentChange != false)
                {
                    result.Issues.Add(
                        string.Format("Config {0} has disabled skill messages on allignment gains", config.Name));
                }
                if (config.NoSkillMessageOnFavorChange != false)
                {
                    result.Issues.Add(
                        string.Format("Config {0} has disabled skill messages on favor gains", config.Name));
                }

                if (!(config.SkillGainRate != SkillGainRate.Per0D01 || config.SkillGainRate != SkillGainRate.Per0D001
                    || config.SkillGainRate != SkillGainRate.Always))
                {
                    if (config.SkillGainRate == SkillGainRate.Never)
                    {
                        result.Issues.Add(
                            string.Format(
                                "Config {0} has low resolution of skill gain messages, "
                                + "which might cause problems at high skill levels",
                                config.Name));
                    }
                    else if (config.SkillGainRate == SkillGainRate.Unknown)
                    {
                        result.Issues.Add(
                            string.Format(
                                "Config {0} has unrecognized value for skill gain messages frequency.",
                                config.Name));
                    }
                    else
                    {
                        result.Issues.Add(
                            string.Format(
                                "Config {0} has too low resolution of skill gain messages, set to: {1}",
                                config.Name,
                                config.SkillGainRate));
                    }
                }

                if (config.TimestampMessages != true)
                {
                    result.Issues.Add(
                        string.Format(
                            "Config {0} causes wurm logs events to not include timestamps, "
                            + "this will break most of Wurm Assistant features",
                            config.Name));
                }
            }

            var missingUptimeInAutoruns = wurmApi.WurmAutoruns.FindIfMissingCommandInAnyAutorun("say /uptime").ToArray();
            if (missingUptimeInAutoruns.Any())
            {
                foreach (var filePath in missingUptimeInAutoruns)
                {
                    result.Issues.Add(string.Format("Autorun file {0} is missing /uptime command.", filePath));
                }
                
            }

            var missingTimeInAutoruns = wurmApi.WurmAutoruns.FindIfMissingCommandInAnyAutorun("say /time").ToArray();
            if (missingTimeInAutoruns.Any())
            {
                foreach (var filePath in missingTimeInAutoruns)
                {
                    result.Issues.Add(string.Format("Autorun file {0} is missing /time command.", filePath));
                }
            }

            return result;
        }
    }
}
