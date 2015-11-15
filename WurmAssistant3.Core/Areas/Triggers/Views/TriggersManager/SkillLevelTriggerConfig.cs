using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager
{
    public partial class SkillLevelTriggerConfig : UserControl, ITriggerConfig
    {
        private readonly SkillLevelTrigger trigger;

        public SkillLevelTriggerConfig(SkillLevelTrigger trigger)
        {
            if (trigger == null) throw new ArgumentNullException("trigger");
            this.trigger = trigger;
            InitializeComponent();

            skillFeedbackLbl.Text = trigger.SkillFeedback;
            SkillNameTbox.Text = trigger.SkillName;
            SkillLevelInput.Value = ((decimal)trigger.TriggerSkillLevel).ConstrainToRange(0, 1000);

            trigger.SubscribeForRefresh(this);
        }

        private void NotificationDelayInput_ValueChanged(object sender, EventArgs e)
        {
            trigger.TriggerSkillLevel = (double)SkillLevelInput.Value;
        }

        private void SkillNameTbox_TextChanged(object sender, EventArgs e)
        {
            trigger.SkillName = SkillNameTbox.Text;
        }

        public UserControl ControlHandle { get { return this; } }

        public void RefreshSkillFeedback()
        {
            skillFeedbackLbl.Text = trigger.SkillFeedback;
        }
    }
}
