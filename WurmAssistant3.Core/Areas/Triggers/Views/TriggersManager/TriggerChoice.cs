using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Data;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager
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
        }

        void CreateButton(string text, Func<ITrigger> clickAction)
        {
            var btn = new Button();
            btn.Width = 150;
            btn.Height = 30;
            btn.Text = text;
            btn.Click += (sender, args) =>
            {
                clickAction();
                this.DialogResult = DialogResult.OK;
            };
            flowLayoutPanel1.Controls.Add(btn);
        }
    }
}
