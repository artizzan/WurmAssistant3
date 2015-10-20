using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers
{
    public partial class TimerDisplayView : UserControl
    {
        private readonly WurmTimer wurmTimer;

        TimeSpan cooldownLength = TimeSpan.Zero;

        string timerName = string.Empty;
        string skillLevel = "0";
        private string meditCount = "0";

        /// <summary>
        /// show skill in () after timer name
        /// </summary>
        public bool ShowSkill = false;

        public TimerDisplayView()
        {
            InitializeComponent();
        }

        public TimerDisplayView(WurmTimer wurmTimer) : this()
        {
            this.wurmTimer = wurmTimer;
            SetName(wurmTimer.ShortName);
        }

        /// <summary>
        /// set to display extra info appended to remaining time,
        /// set null or empty to disable
        /// </summary>
        public string ExtraInfo { get; set; }

        public bool ShowMeditCount { get; set; }

        public WidgetModeManager WidgetManager { get; set; }

        public void SetName(string text)
        {
            timerName = text;
            labelName.Text = timerName;
        }

        /// <summary>
        /// Sets the duration of a cooldown, as displayed on progress bar. If actual cooldown is longer, it will show as empty progress bar.
        /// </summary>
        /// <param name="length"></param>
        public void SetCooldown(TimeSpan length)
        {
            this.cooldownLength = length;
        }

        public void UpdateCooldown(DateTime cooldownTo)
        {
            UpdateCooldown(cooldownTo - DateTime.Now);
        }

        /// <summary>
        /// update skill with most recent skill value
        /// </summary>
        /// <param name="skillValue"></param>
        public void UpdateSkill(float skillValue)
        {
            skillLevel = skillValue.ToString("F2");
        }

        /// <summary>
        /// sets the skill display to any text
        /// </summary>
        /// <param name="txt"></param>
        public void SetCustomStringAsSkill(string txt)
        {
            skillLevel = txt;
        }

        public void SetMeditCount(int count)
        {
            meditCount = count.ToString();
        }

        public void UpdateCooldown(TimeSpan cd_remaining)
        {
            string presentation = timerName;
            if (ShowSkill) presentation += " ("+skillLevel+")";
            if (ShowMeditCount) presentation += " " + meditCount;
            labelName.Text = presentation;

            if (cd_remaining.Ticks < 0)
            {
                labelTimeTo.Text = "ready!";
                progressBar1.Value = progressBar1.Maximum;
            }
            else
            {
                int value = (int)((cd_remaining.TotalSeconds / cooldownLength.TotalSeconds) * progressBar1.Maximum);
                if (value > progressBar1.Maximum) value = progressBar1.Maximum;
                else if (value < 0) value = 0;

                value = progressBar1.Maximum - value;

                progressBar1.Value = value;

                labelTimeTo.Text = string.Empty;
                if (cd_remaining.Days > 1) labelTimeTo.Text += String.Format("{0} days ", cd_remaining.Days);
                else if (cd_remaining.Days > 0) labelTimeTo.Text += String.Format("{0} day ", cd_remaining.Days);
                labelTimeTo.Text += cd_remaining.ToString(@"hh\:mm\:ss");
                if (!string.IsNullOrEmpty(ExtraInfo)) labelTimeTo.Text += ExtraInfo;
            }
        }

        private void UControlTimerDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }

        private void labelTimeTo_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }

        private void labelName_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }

        private void progressBar1_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }

        void HandleMouseClick(MouseEventArgs e)
        {
            if (WidgetManager != null && WidgetManager.WidgetMode)
            {
                return;
            }
            if (e.Button == MouseButtons.Right) wurmTimer.OpenTimerConfig();
        }

        private void tableLayoutPanel2_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }

        private void tableLayoutPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(e);
        }
    }
}
