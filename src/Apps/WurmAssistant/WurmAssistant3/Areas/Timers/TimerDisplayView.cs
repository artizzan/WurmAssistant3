using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers
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

        // Update cooldown overloads group to support different input types
        // Only custom timers can run as elapsed time, and pass the cooldown from
        public void UpdateCooldown(DateTime cooldownTo, DateTime coolDownFrom)
        {
            UpdateCooldown(cooldownTo - DateTime.Now, coolDownFrom);
        }
        public void UpdateCooldown(DateTime cooldownTo)
        {
            UpdateCooldown(cooldownTo - DateTime.Now, DateTime.Now);
        }
        public void UpdateCooldown(TimeSpan cooldownTo)
        {
            UpdateCooldown(cooldownTo, DateTime.Now);
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

        public void UpdateCooldown(TimeSpan cd_remaining, DateTime cooldownFrom)
        {
            // todo: before making more adjustments here, consider refactoring naming and simplifying logic, as it becomes difficult to understand what happens here

            TimeSpan timeSpan = cd_remaining;
            string presentation = timerName;
            int value;
            if (ShowSkill) presentation += " ("+skillLevel+")";
            if (ShowMeditCount) presentation += " " + meditCount;
            labelName.Text = presentation;
            labelTimeTo.Text = string.Empty;

            if (wurmTimer.TimerDefinition.IsCustomTimerShowingElapsed())
            {
                // Elapsed timer
                progressBar1.Value = progressBar1.Maximum;
                timeSpan = DateTime.Now - cooldownFrom;
                SetBarColor(BarState.Elapsed);
            }
            else
            {
                // Running
                value = (int)((cd_remaining.TotalSeconds / cooldownLength.TotalSeconds) * progressBar1.Maximum);
                if (value > progressBar1.Maximum) value = progressBar1.Maximum;
                else if (value < 0) value = 0;
                value = progressBar1.Maximum - value;
                progressBar1.Value = value;
                SetBarColor(BarState.Running);
            }

            if (timeSpan.Ticks < 0)
            {
                // Completed timer
                labelTimeTo.Text = "ready!";
                progressBar1.Value = progressBar1.Maximum;
                SetBarColor(BarState.Ready);
            }
            else
            {
                if (wurmTimer.TimersFeature.ShowEndDateInsteadOfTimeRemaining)
                {
                    labelTimeTo.Text += (DateTime.Now + timeSpan).ToString("MM-dd HH:mm:ss");
                }
                else if (wurmTimer.TimersFeature.ShowEndDate)
                {
                    if (timeSpan.Days > 1)
                    {
                        labelTimeTo.Text += String.Format("{0} days ", timeSpan.Days);
                        labelTimeTo.Text += timeSpan.ToString(@"hh\:mm\:ss");
                    }
                    else if (timeSpan.Days > 0)
                    {
                        labelTimeTo.Text += String.Format("{0} day ", timeSpan.Days);
                        labelTimeTo.Text += timeSpan.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        labelTimeTo.Text += timeSpan.ToString(@"hh\:mm\:ss");
                        labelTimeTo.Text += string.Format(" ({0})", (DateTime.Now + timeSpan).ToString("HH:mm"));
                    }
                }
                else
                {
                    if (timeSpan.Days > 1)
                    {
                        labelTimeTo.Text += String.Format("{0} days ", timeSpan.Days);
                        labelTimeTo.Text += timeSpan.ToString(@"hh\:mm\:ss");
                    }
                    else if (timeSpan.Days > 0)
                    {
                        labelTimeTo.Text += String.Format("{0} day ", timeSpan.Days);
                        labelTimeTo.Text += timeSpan.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        labelTimeTo.Text += timeSpan.ToString(@"hh\:mm\:ss");
                    }
                }

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

        private void SetBarColor(BarState barState)
        {
            // SetState colors: 1 = Green, 2 = Red, 3 = Yellow
            if (this.wurmTimer.TimersFeature.BarColorMode == 0)
            {
                progressBar1.SetState(1);
            } else
            {
                switch(barState)
                {
                    case BarState.Ready: 
                        progressBar1.SetState(this.wurmTimer.TimersFeature.BarColorMode == 1 ? 1 : 2);
                        break;
                    case BarState.Running: 
                        progressBar1.SetState(this.wurmTimer.TimersFeature.BarColorMode == 1 ? 2 : 1);
                        break;
                    case BarState.Elapsed:
                        progressBar1.SetState(3);
                        break;
                }
            }
        }
    }

    public enum BarState
    {
        Elapsed,
        Running,
        Ready
    }

    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
   
}
