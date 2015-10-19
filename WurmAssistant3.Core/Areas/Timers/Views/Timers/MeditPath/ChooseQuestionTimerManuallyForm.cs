using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers.MeditPath;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Views.Timers.MeditPath
{
    public partial class ChooseQuestionTimerManuallyForm : Form
    {
        MeditPathTimer ParentTimer;

        public ChooseQuestionTimerManuallyForm(MeditPathTimer parentTimer)
        {
            this.ParentTimer = parentTimer;
            InitializeComponent();
            InitializeChoiceList();
        }

        void InitializeChoiceList()
        {
            foreach (var keyval in MeditPathTimer.MeditPathHelper.LevelToTitlesMap)
            {
                string level = keyval.Key.ToString();
                string titles = "";
                foreach (string str in keyval.Value)
                {
                    titles += str+", ";
                }
                if (titles.Length > 0) titles = titles.Remove(titles.Length - 2, 2);

                string output = "Level " + level + ": " + titles;
                listBox1.Items.Add(output);
            }
            listBox1.Refresh();
        }

        private void FormChooseQTimerManually_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dateTimePicker1.Visible = false;
                dateTimePicker1.Value = DateTime.Now;
            }
            else dateTimePicker1.Visible = true;
        }

        public DateTime GetResultOriginDate()
        {
            return dateTimePicker1.Value;
        }
        public int GetResultMeditLevel()
        {
            int result = listBox1.SelectedIndex + 1;
            return result > 15 ? 15 : result;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Choose your meditation level");
                DialogResult = System.Windows.Forms.DialogResult.None;
            }
        }
    }
}
