using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.Notifiers
{
    public partial class ChooseNotifierType : ExtendedForm
    {
        public INotifier Result = null;

        public ChooseNotifierType(ITrigger trigger, IEnumerable<INotifier> existingNotifiers, [NotNull] ITrayPopups trayPopups,
            [NotNull] ISoundEngine soundEngine)
        {
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");

            InitializeComponent();
            var enumerable = existingNotifiers as INotifier[] ?? existingNotifiers.ToArray();
            
            CreateButton("Sound Notifier",
                () => Result = new SoundNotifier(trigger, soundEngine), 
                enumerable.Any(x => x is ISoundNotifier));
            
            CreateButton("Popup Notifier",
                () => Result = new PopupNotifier(trigger, trayPopups), 
                enumerable.Any(x => x is IPopupNotifier));
        }

        void CreateButton(string text, Func<INotifier> clickAction, bool disabled)
        {
            var btn = new Button {Width = 150, Height = 30, Text = text};
            if (disabled) btn.Enabled = false;
            else
            {
                btn.Click += (sender, args) =>
                             {
                                 clickAction();
                                 this.DialogResult = DialogResult.OK;
                             };
            }
            flowLayoutPanel1.Controls.Add(btn);
        }
    }
}
