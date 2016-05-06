using System;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules.Notifiers;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Views.Notifiers
{
    public partial class PopupConfig : UserControl, INotifierConfig
    {
        private readonly IPopupNotifier popupNotifier;
        private readonly bool initComplete;

        public event EventHandler Removed;

        public UserControl ControlHandle { get { return this; } }

        public PopupConfig(IPopupNotifier popupNotifier)
        {
            InitializeComponent();
            this.popupNotifier = popupNotifier;
            ContentTextBox.Text = popupNotifier.Content;
            TitleTextBox.Text = popupNotifier.Title;
            DurationNumeric.Value = ((decimal)popupNotifier.Duration.TotalSeconds).ConstrainToRange(0, 10000);
            StayUntilClickedCheckBox.Checked = popupNotifier.StayUntilClicked;
            DurationNumeric.Enabled = !popupNotifier.StayUntilClicked;

            initComplete = true;
        }

        private void Save()
        {
            if (initComplete)
            {
                popupNotifier.Content = ContentTextBox.Text;
                popupNotifier.Title = TitleTextBox.Text;
                popupNotifier.Duration = TimeSpan.FromSeconds((double) DurationNumeric.Value);
                popupNotifier.StayUntilClicked = StayUntilClickedCheckBox.Checked;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            OnRemoved();
        }

        private void TitleTextBox_TextChanged(object sender, EventArgs e)
        {
            Save();
        }

        private void ContentTextBox_TextChanged(object sender, EventArgs e)
        {
            Save();
        }

        private void DurationNumeric_ValueChanged(object sender, EventArgs e)
        {
            Save();
        }

        private void StayUntilClickedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Save();
            DurationNumeric.Enabled = !StayUntilClickedCheckBox.Checked;
        }

        protected void OnRemoved()
        {
            var handler = Removed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
