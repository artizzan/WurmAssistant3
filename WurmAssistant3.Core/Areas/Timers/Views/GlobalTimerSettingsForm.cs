using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views
{
    public partial class GlobalTimerSettingsForm : ExtendedForm
    {
        private readonly TimersForm timersView;

        public GlobalTimerSettingsForm(TimersForm timersView)
        {
            this.timersView = timersView;
            InitializeComponent();

            checkBoxWidgetView.Checked = timersView.WidgetModeEnabled;
            textBoxWidgetSample.BackColor = timersView.WidgetBgColor;
            textBoxWidgetSample.ForeColor = timersView.WidgetForeColor;
        }

        private void checkBoxWidgetView_CheckedChanged(object sender, EventArgs e)
        {
            timersView.WidgetModeEnabled = checkBoxWidgetView.Checked;
        }

        private void buttonChangeWidgetBgColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = timersView.WidgetBgColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                timersView.WidgetBgColor = colorDialog1.Color;
                textBoxWidgetSample.BackColor = colorDialog1.Color;
            }
        }

        private void textBoxWidgetSample_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonSetWidgetFontColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = timersView.WidgetForeColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                timersView.WidgetForeColor = colorDialog1.Color;
                textBoxWidgetSample.ForeColor = colorDialog1.Color;
            }
        }

        private void buttonResetWidgetDefaultColor_Click(object sender, EventArgs e)
        {
            textBoxWidgetSample.BackColor = timersView.WidgetBgColor = DefaultBackColor;
            textBoxWidgetSample.ForeColor = timersView.WidgetForeColor = DefaultForeColor;
        }
    }
}
