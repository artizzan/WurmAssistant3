using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Views.TriggersManager
{
    public partial class TriggerChoice : ExtendedForm
    {
        public ITrigger Result = null;

        public TriggerChoice(TriggerManager triggerManager)
        {
            InitializeComponent();
            CreateButton("Simple", () => Result = triggerManager.CreateTrigger(TriggerKind.Simple));
            CreateButton("Regex", () => Result = triggerManager.CreateTrigger(TriggerKind.Regex));
            CreateButton("Action Queue", () => Result = triggerManager.CreateTrigger(TriggerKind.ActionQueue));
            CreateButton("Skill Level", () => Result = triggerManager.CreateTrigger(TriggerKind.SkillLevel));
        }

        void CreateButton(string text, Func<ITrigger> clickAction)
        {
            var btn = new Button();
            btn.Width = 150;
            btn.Height = 30;
            btn.Text = text;
            btn.Click += (sender, args) =>
            {
                try
                {
                    clickAction();
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            flowLayoutPanel1.Controls.Add(btn);
        }
    }
}
