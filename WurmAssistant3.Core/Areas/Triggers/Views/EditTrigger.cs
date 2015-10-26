using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.Notifiers;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views
{
    public partial class EditTrigger : ExtendedForm
    {
        private readonly ITrigger _trigger;
        readonly ISoundEngine soundEngine;
        readonly ITrayPopups trayPopups;

        public EditTrigger(ITrigger trigger, [NotNull] ISoundEngine soundEngine, [NotNull] ITrayPopups trayPopups)
        {
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            _trigger = trigger;
            this.soundEngine = soundEngine;
            this.trayPopups = trayPopups;
            InitializeComponent();
            Text = trigger.TypeAspect.Capitalize() + " Trigger";
            trigger.Configs.ToList()
                .ForEach(x => SettingsLayout.Controls.Add(x.ControlHandle));
            trigger.GetNotifiers().ToList()
                .ForEach(AddConfigurator);
        }

        private void AddNotificationButton_Click(object sender, EventArgs e)
        {
            IEnumerable<INotifier> restrictNotifiers = _trigger.GetNotifiers();
            var ui = new ChooseNotifierType(_trigger, restrictNotifiers, trayPopups, soundEngine);
            if (ui.ShowDialogCenteredOnForm(this) == DialogResult.OK)
            {
                _trigger.AddNotifier(ui.Result);
                AddConfigurator(ui.Result);
            }
        }

        void AddConfigurator(INotifier notifier)
        {
            var configurator = notifier.GetConfig();
            var uc = configurator.ControlHandle;
            NotificationsLayout.Controls.Add(uc);
            configurator.Removed += (o, args) =>
            {
                NotificationsLayout.Controls.Remove(configurator.ControlHandle);
                _trigger.RemoveNotifier(notifier);
            };
        }

        private void SettingsLayout_Layout(object sender, LayoutEventArgs e)
        {
            foreach (UserControl ctrl in SettingsLayout.Controls)
            {
                ctrl.Width = SettingsLayout.Width - 25;
            }
        }
    }
}
