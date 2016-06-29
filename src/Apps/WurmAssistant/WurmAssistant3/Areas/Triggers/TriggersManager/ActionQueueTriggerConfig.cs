using System;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Triggers.ActionQueueParsing;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager
{
    public partial class ActionQueueTriggerConfig : UserControl, ITriggerConfig
    {
        private readonly ActionQueueTrigger actionQueueTrigger;
        readonly IActionQueueConditions conditions;

        public ActionQueueTriggerConfig(ActionQueueTrigger actionQueueTrigger,
            [NotNull] IActionQueueConditions conditions)
        {
            if (conditions == null) throw new ArgumentNullException("conditions");
            this.actionQueueTrigger = actionQueueTrigger;
            this.conditions = conditions;
            InitializeComponent();

            NotificationDelayInput.Value = ((decimal)this.actionQueueTrigger.NotificationDelay).ConstrainToRange(0, 1000);
        }

        public UserControl ControlHandle { get { return this; } }

        private void NotificationDelayInput_ValueChanged(object sender, EventArgs e)
        {
            actionQueueTrigger.NotificationDelay = (double)NotificationDelayInput.Value;
        }

        private void ModifyConditionsBtn_Click(object sender, EventArgs e)
        {
            conditions.ShowConditionsEditGui();
        }
    }
}
