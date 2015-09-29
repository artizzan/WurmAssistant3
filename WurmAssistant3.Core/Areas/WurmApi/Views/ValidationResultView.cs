using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Modules;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.WurmApi.Views
{
    public partial class ValidationResultView : Form
    {
        readonly WurmClientValidator wurmClientValidator;

        public ValidationResultView([NotNull] WurmClientValidator wurmClientValidator)
        {
            if (wurmClientValidator == null) throw new ArgumentNullException("wurmClientValidator");
            this.wurmClientValidator = wurmClientValidator;
            InitializeComponent();
            SkipCheckOnStart.Checked = wurmClientValidator.SkipOnStart;
        }

        public void SetText(string text)
        {
            IssuesTb.Text = text;
        }

        private void SkipCheckOnStart_CheckedChanged(object sender, System.EventArgs e)
        {
            wurmClientValidator.SkipOnStart = SkipCheckOnStart.Enabled;
        }
    }
}
