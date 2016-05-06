using System;
using System.Diagnostics;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules.TriggersManager;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Views.TriggersManager
{
    public partial class TriggerBaseConfig : UserControl, ITriggerConfig
    {
        private readonly TriggerBase trigger;
        readonly IWurmApi wurmApi;
        readonly ILogger logger;

        private readonly bool initComplete = false;

        public TriggerBaseConfig(TriggerBase trigger, [NotNull] IWurmApi wurmApi, [NotNull] ILogger logger)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            this.trigger = trigger;
            this.wurmApi = wurmApi;
            this.logger = logger;
            InitializeComponent();
            toolTip1.SetToolTip(ResetOnCndHitChkbox, "Every time condition is hit, cooldown will be set to full duration (including when trigger is already on cooldown). Useful for new chat message triggers.".WrapLines());

            TriggerNameTbox.Text = trigger.Name;
            ApplicableLogsDisplayTxtbox.Text = trigger.LogTypesAspect;
            foreach (var logType in wurmApi.LogDefinitions.AllLogTypes)
            {
                LogTypesChklist.Items.Add(logType, trigger.CheckLogType(logType));
            }
            if (trigger.LogTypesLocked) LogTypesChklist.Enabled = false;
            CooldownEnabledChkbox.Checked
                = ResetOnCndHitChkbox.Enabled
                = CooldownInput.Enabled 
                = trigger.CooldownEnabled;
            CooldownInput.Value = trigger.Cooldown;
            ResetOnCndHitChkbox.Checked = this.trigger.ResetOnConditonHit;
            if (this.trigger.DefaultDelayFunctionalityDisabled)
            {
                DelayChkbox.Enabled = DelayInput.Enabled = false;
            }
            else
            {
                DelayChkbox.Checked = DelayInput.Enabled = this.trigger.DelayEnabled;
                DelayInput.Value = this.trigger.Delay;
            }

            initComplete = true;
        }

        public UserControl ControlHandle { get { return this; } }

        private void TriggerNameTbox_TextChanged(object sender, EventArgs e)
        {
            if (initComplete) trigger.Name = TriggerNameTbox.Text;
        }

        private void LogTypesChklist_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (initComplete)
            {
                trigger.SetLogType((LogType)LogTypesChklist.Items[e.Index], e.NewValue);
                ApplicableLogsDisplayTxtbox.Text = trigger.LogTypesAspect;
            }
        }

        private void CooldownEnabledChkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (initComplete)
            {
                CooldownInput.Enabled 
                    = ResetOnCndHitChkbox.Enabled
                    = trigger.CooldownEnabled 
                    = CooldownEnabledChkbox.Checked;
            }
        }

        private void CooldownInput_ValueChanged(object sender, EventArgs e)
        {
            if (initComplete) trigger.Cooldown = CooldownInput.Value;
        }

        private void ResetOnCndHitChkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (initComplete) trigger.ResetOnConditonHit = ResetOnCndHitChkbox.Checked;
        }

        private void DelayChkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (initComplete)
            {
                DelayInput.Enabled
                    = trigger.DelayEnabled
                    = DelayChkbox.Checked;
            }
        }

        private void DelayInput_ValueChanged(object sender, EventArgs e)
        {
            if (initComplete) trigger.Delay = DelayInput.Value;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(
                    @"http://forum.wurmonline.com/index.php?/topic/68031-wurm-assistant-2x-bundle-of-useful-tools/#entry948073");
            }
            catch (Exception exception)
            {
                logger.Error(exception, "");
            }

        }
    }
}
