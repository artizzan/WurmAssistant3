using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules.Notifiers;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Views.Notifiers
{
    public partial class MessageConfig : UserControl, INotifierConfig
    {
        private readonly IMessageNotifier messageNotifier;
        private readonly bool initComplete;

        public event EventHandler Removed;

        public UserControl ControlHandle { get { return this; }}

        public MessageConfig(IMessageNotifier messageNotifier)
        {
            InitializeComponent();
            this.messageNotifier = messageNotifier;
            ContentTextBox.Text = messageNotifier.Content;
            TitleTextBox.Text = messageNotifier.Title;

            initComplete = true;
        }

        private void Save()
        {
            if (initComplete)
            {
                messageNotifier.Content = ContentTextBox.Text;
                messageNotifier.Title = TitleTextBox.Text;
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

        void OnRemoved()
        {
            var handler = Removed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
