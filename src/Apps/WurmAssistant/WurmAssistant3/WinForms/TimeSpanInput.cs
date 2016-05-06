using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.WinForms
{
    public partial class TimeSpanInput : UserControl
    {
        public TimeSpanInput()
        {
            InitializeComponent();
        }

        TimeSpan _value;
        public TimeSpan Value
        {
            get { return _value; }
            set
            {
                _value = value;
                numericUpDownDay.Value = value.Days;
                numericUpDownHour.Value = value.Hours;
                numericUpDownMinute.Value = value.Minutes;
                numericUpDownSecond.Value = value.Seconds;
            }
        }

        public event EventHandler ValueChanged;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            CalculateValue();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            CalculateValue();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            CalculateValue();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            CalculateValue();
        }

        void CalculateValue()
        {
            Value = new TimeSpan(
                Convert.ToInt32(numericUpDownDay.Value),
                Convert.ToInt32(numericUpDownHour.Value),
                Convert.ToInt32(numericUpDownMinute.Value),
                Convert.ToInt32(numericUpDownSecond.Value));
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
        }
    }
}
