using System;
using AldursLab.WurmAssistant3.Areas.Timers.Services.Timers.Alignment;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Timers.Parts.Timers.Alignment
{
    public partial class AlignmentTimerOptionsForm : ExtendedForm
    {
        private readonly AlignmentTimer alignmentTimer;

        public bool WhiteLighter { get; private set; }
        public AlignmentTimer.WurmReligions Religion { get; private set; }

        public AlignmentTimerOptionsForm(AlignmentTimer alignmentTimer)
        {
            this.alignmentTimer = alignmentTimer;
 
            InitializeComponent();

            if (alignmentTimer.IsWhiteLighter) radioButtonWL.Checked = true;
            else radioButtonBL.Checked = true;

            switch (alignmentTimer.PlayerReligion)
            {
                case AlignmentTimer.WurmReligions.Fo:
                    radioButtonFo.Checked = true; break;
                case AlignmentTimer.WurmReligions.Magranon:
                    radioButtonMag.Checked = true; break;
                case AlignmentTimer.WurmReligions.Vynora:
                    radioButtonVyn.Checked = true; break;
                case AlignmentTimer.WurmReligions.Libila:
                    radioButtonLib.Checked = true; break;
            }

            radioButtonWL.CheckedChanged += UpdateResult;
            radioButtonBL.CheckedChanged += UpdateResult;
            radioButtonFo.CheckedChanged += UpdateResult;
            radioButtonMag.CheckedChanged += UpdateResult;
            radioButtonVyn.CheckedChanged += UpdateResult;
            radioButtonLib.CheckedChanged += UpdateResult;

            UpdateResult(new object(), new EventArgs());
        }

        void UpdateResult(object sender, EventArgs e)
        {
            if (radioButtonWL.Checked) WhiteLighter = true;
            else if (radioButtonBL.Checked) WhiteLighter = false;

            if (radioButtonFo.Checked) Religion = AlignmentTimer.WurmReligions.Fo;
            else if (radioButtonMag.Checked) Religion = AlignmentTimer.WurmReligions.Magranon;
            else if (radioButtonVyn.Checked) Religion = AlignmentTimer.WurmReligions.Vynora;
            else if (radioButtonLib.Checked) Religion = AlignmentTimer.WurmReligions.Libila;
        }

        private void buttonVerifyList_Click(object sender, EventArgs e)
        {
            alignmentTimer.ShowVerifyList();
        }
    }
}
