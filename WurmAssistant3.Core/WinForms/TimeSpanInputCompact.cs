using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Core.WinForms
{
    public partial class TimeSpanInputCompact : UserControl
    {
        public TimeSpanInputCompact()
        {
            InitializeComponent();
        }

        public TimeSpan Value
        {
            get { return new TimeSpan(_lastVals[0], _lastVals[1], _lastVals[2], _lastVals[3]); }
            set
            {
                _lastVals[0] = value.Days;
                _lastVals[1] = value.Hours;
                _lastVals[2] = value.Minutes;
                _lastVals[3] = value.Seconds;
                SetCtrlsNovalidate(value.Days, value.Hours, value.Minutes, value.Seconds);
            }
        }

        public bool ReadOnly
        {
            set { DaysTb.ReadOnly = HoursTb.ReadOnly = MinutesTb.ReadOnly = SecondsTb.ReadOnly = value; }
            get { return DaysTb.ReadOnly; }
        }

        public event EventHandler ValueChanged;

        void ValidateAndSetVals()
        {
            if (!_noValidate)
            {
                try
                {
                    var d = GetTbVal(DaysTb, 0, 999);
                    var h = GetTbVal(HoursTb, 0, 23);
                    var m = GetTbVal(MinutesTb, 0, 59);
                    var s = GetTbVal(SecondsTb, 0, 59);
                    SetLastVals(d, h, m, s);
                    SetCtrlsNovalidate(d, h, m, s);
                }
                catch (Exception)
                {
                    SetCtrlsNovalidate(_lastVals);
                }
            }
        }

        private int GetTbVal(TextBox tb, int min, int max)
        {
            return RangeBound(int.Parse(string.IsNullOrEmpty(tb.Text.Trim()) ? "0" : tb.Text), min, max);
        }

        int RangeBound(int input, int min, int max)
        {
            if (input < min) return min;
            if (input > max) return max;
            return input;
        }

        void SetLastVals(int days, int hours, int minutes, int seconds)
        {
            if (days != _lastVals[0] || hours != _lastVals[1] || minutes != _lastVals[2] || seconds != _lastVals[3])
            {
                _lastVals[0] = days;
                _lastVals[1] = hours;
                _lastVals[2] = minutes;
                _lastVals[3] = seconds;
                var eh = ValueChanged;
                if (eh != null) eh(this, new EventArgs());
            }
        }

        private bool _noValidate = false;

        void SetCtrlsNovalidate(int days, int hours, int minutes, int seconds)
        {
            _noValidate = true;
            DaysTb.Text = days.ToString();
            HoursTb.Text = hours.ToString();
            MinutesTb.Text = minutes.ToString();
            SecondsTb.Text = seconds.ToString();
            _noValidate = false;
        }

        void SetCtrlsNovalidate(int[] args)
        {
            SetCtrlsNovalidate(args[0], args[1], args[2], args[3]);
        }

        int[] _lastVals = { 0, 0, 0, 0 };

        private void DaysTb_TextChanged(object sender, EventArgs e)
        {
            ValidateAndSetVals();
        }

        private void HoursTb_TextChanged(object sender, EventArgs e)
        {
            ValidateAndSetVals();
        }

        private void MinutesTb_TextChanged(object sender, EventArgs e)
        {
            ValidateAndSetVals();
        }

        private void SecondsTb_TextChanged(object sender, EventArgs e)
        {
            ValidateAndSetVals();
        }
    }
}
