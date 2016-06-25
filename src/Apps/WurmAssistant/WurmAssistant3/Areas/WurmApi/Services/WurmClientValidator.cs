using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.WurmApi.Contracts;
using AldursLab.WurmAssistant3.Areas.WurmApi.Parts;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.WurmApi.Services
{
    [KernelBind(BindingHint.Singleton), PersistentObject("WurmClientValidator")]
    public class WurmClientValidator : PersistentObjectBase, IWurmClientValidator
    {
        readonly IWurmApi wurmApi;

        [JsonProperty]
        bool skipOnStart = false;

        public WurmClientValidator([NotNull] IWurmApi wurmApi)
        {
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            this.wurmApi = wurmApi;
        }

        public bool SkipOnStart
        {
            get { return skipOnStart; }
            set
            {
                if (value == skipOnStart) return;
                skipOnStart = value;
                FlagAsChanged();
            }
        }

        public IReadOnlyList<WurmClientIssue> Validate()
        {
            var issues = new List<WurmClientIssue>();
            issues.AddRange(CheckAllConfigs());
            issues.AddRange(AppendAutoruns());
            return issues.ToArray();
        }

        public void ShowSummaryWindow(IReadOnlyList<WurmClientIssue> issues)
        {
            if (issues == null || !issues.Any()) issues = new WurmClientIssue[] {new WurmClientIssue("No issues")};

            var view = new ValidationResultForm(this);
            view.SetText(String.Join(Environment.NewLine, issues));
            view.Show();
        }

        IEnumerable<WurmClientIssue> AppendAutoruns()
        {
            wurmApi.Autoruns.MergeCommandToAllAutoruns("say /uptime");
            wurmApi.Autoruns.MergeCommandToAllAutoruns("say /time");
            return new WurmClientIssue[0];
        }

        IEnumerable<WurmClientIssue> CheckAllConfigs()
        {
            var issues = new List<WurmClientIssue>();
            var configs = wurmApi.Configs.All;
            foreach (var wurmConfig in configs)
            {
                var validator = new ConfigValidator(wurmConfig);
                validator.Validate();
                issues.AddRange(validator.GetIssues());
            }
            return issues;
        }

        class ConfigValidator
        {
            readonly IWurmConfig config;
            readonly List<WurmClientIssue> issues = new List<WurmClientIssue>();
            bool headerWritten = false;

            public ConfigValidator([NotNull] IWurmConfig config)
            {
                if (config == null) throw new ArgumentNullException("config");
                this.config = config;
            }

            public void Validate()
            {
                if (!config.HasBeenRead)
                {
                    AddIssue("Could not read config values. See application log for details...");
                    return;
                }

                if (!config.TimestampMessages.HasValue)
                {
                    AddIssue("Unknown state of \"Timestamp messages\" option.");
                }
                else if (config.TimestampMessages == false)
                {
                    AddIssue(
                        "IMPORTANT: \"Timestamp messages\" option must be enabled. WurmAssistant log reading relies on timestamps.");
                }

                if (!config.IrcLoggingType.In(LogSaveMode.Daily, LogSaveMode.Monthly))
                {
                    AddIssue("\"Irc message logging\" option should be set to \"Daily files\" or \"Monthly files\".");
                }
                if (!config.EventLoggingType.In(LogSaveMode.Daily, LogSaveMode.Monthly))
                {
                    AddIssue("\"Event message logging\" option should be set to \"Daily files\" or \"Monthly files\".");
                }
                if (!config.OtherLoggingType.In(LogSaveMode.Daily, LogSaveMode.Monthly))
                {
                    AddIssue("\"Other message logging\" option should be set to \"Daily files\" or \"Monthly files\".");
                }

                if (!config.NoSkillMessageOnAlignmentChange.HasValue)
                {
                    AddIssue("Unknown state of \"Hide alignment updates\" option.");
                }
                else if (config.NoSkillMessageOnAlignmentChange == true)
                {
                    AddIssue("\"Hide alignment updates\" option should be unchecked.");
                }

                if (!config.NoSkillMessageOnFavorChange.HasValue)
                {
                    AddIssue("Unknown state of \"Hide favor updates\" option.");
                }
                else if (config.NoSkillMessageOnFavorChange == true)
                {
                    AddIssue("\"Hide favor updates\" option should be unchecked.");
                }

                if (!config.SkillGainRate.In(SkillGainRate.Always, SkillGainRate.Per0D001))
                {
                    AddIssue(
                        "\"Skillgain tab updates\" option should be set to as frequent as possible. Recommended setting is \"Always\" or \"Per 0.001 increase\". This is especially important at very high skill levels.");
                }

                if (config.SaveSkillsOnQuit != true)
                {
                    AddIssue("\"Save skills on quit\" option should be enabled. It greatly helps in finding all skill levels.");
                }
            }

            void AddIssue(string issue)
            {
                if (!headerWritten)
                {
                    issues.Add(new WurmClientIssue(">> Issues in config: " + config.Name));
                    headerWritten = true;
                }
                issues.Add(new WurmClientIssue(issue));
            }

            public IEnumerable<WurmClientIssue> GetIssues()
            {
                if (issues.Any())
                {
                    issues.Add(new WurmClientIssue("-------------"));
                }
                return issues;
            }
        }
    }

    public class WurmClientIssue
    {
        public WurmClientIssue(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text ?? string.Empty;
        }
    }
}
