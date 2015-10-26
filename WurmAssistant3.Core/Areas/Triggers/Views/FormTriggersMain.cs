using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.TriggersManager;
using AldursLab.WurmAssistant3.Core.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Views
{
    public partial class FormTriggersMain : ExtendedForm
    {
        readonly TriggersFeature parent;
        readonly ISoundEngine soundEngine;

        public FormTriggersMain([NotNull] TriggersFeature parent, [NotNull] ISoundEngine soundEngine)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            this.parent = parent;
            this.soundEngine = soundEngine;
            InitializeComponent();
        }

        private void FormSoundNotifyMain_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(buttonAddNew, "Add new sound managers for more wurm characters");
        }

        public void AddNotifierController(UcPlayerTriggersController controller)
        {
            flowLayoutPanel1.Controls.Add(controller);
        }

        public void RemoveNotifierController(UcPlayerTriggersController controller)
        {
            flowLayoutPanel1.Controls.Remove(controller);
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            //show dialog to choose player
            parent.AddNewNotifier();
        }

        private void FormSoundNotifyMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
