using System;
using System.Text.RegularExpressions;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.SoundManager.Contracts;
using AldursLab.WurmAssistant3.Areas.Timers.Contracts;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Timers.Modules.Timers.JunkSale
{
    [PersistentObject("TimersFeature_JunkSaleTimer")]
    public class JunkSaleTimer : WurmTimer
    {
        [JsonProperty]
        DateTime cooldownUntil;
        [JsonProperty]
        int currentTotalAmount;

        public JunkSaleTimer(string persistentObjectId, IWurmApi wurmApi, ILogger logger, ISoundManager soundManager,
            ITrayPopups trayPopups)
            : base(persistentObjectId, trayPopups, logger, wurmApi, soundManager)
        {
        }

        public override void Initialize(PlayerTimersGroup parentGroup, string player, TimerDefinition definition)
        {
            base.Initialize(parentGroup, player, definition);
            View.ShowSkill = true;
            VerifyMoneyAmountAgainstCd();
            UpdateMoneyCounter();
            InitCompleted = true;
        }

        public override void Update()
        {
            base.Update();
            if (View.Visible)
                View.UpdateCooldown(cooldownUntil - DateTime.Now);
            VerifyMoneyAmountAgainstCd();
        }

        void VerifyMoneyAmountAgainstCd()
        {
            if (currentTotalAmount != 0 && DateTime.Now > cooldownUntil)
            {
                currentTotalAmount = 0;
                UpdateMoneyCounter();
            }
        }

        public override void HandleNewEventLogLine(LogEntry line)
        {
            if (line.Content.StartsWith("You receive", StringComparison.Ordinal))
            {
                Match match = Regex.Match(line.Content, @"You receive (\d+) irons\.");
                if (match.Success)
                {
                    if (DateTime.Now > cooldownUntil)
                    {
                        // reset the timer only if its already over
                        cooldownUntil = DateTime.Now + TimeSpan.FromHours(1);
                        currentTotalAmount = 0;
                    }

                    try
                    {
                        currentTotalAmount += int.Parse(match.Groups[1].Value);
                    }
                    catch (FormatException _e)
                    {
                        Logger.Error(_e, "Invalid format while attempting to parse junksale gain amount");
                    }

                    UpdateMoneyCounter();
                }
            }
        }

        private void UpdateMoneyCounter()
        {
            View.SetCustomStringAsSkill(PrepareStrDisplayForMoneyAmount(currentTotalAmount));
        }

        string PrepareStrDisplayForMoneyAmount(int amount)
        {
            int coppers = amount/100;
            int irons = amount%100;
            return string.Format("{0}c{1}i", coppers, irons);
        }
    }
}
