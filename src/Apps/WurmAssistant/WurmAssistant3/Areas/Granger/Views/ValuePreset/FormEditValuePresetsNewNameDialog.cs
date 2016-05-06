using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Granger.Views.ValuePreset
{
    public partial class FormEditValuePresetsNewNameDialog : ExtendedForm
    {
        private FormEditValuePresets FormEditValuePresets;
        private GrangerContext Context;

        public string Result { get; private set; }

        HashSet<string> TakenValueMapIDs = new HashSet<string>();

        public FormEditValuePresetsNewNameDialog(FormEditValuePresets formEditValuePresets, GrangerContext context)
        {
            this.FormEditValuePresets = formEditValuePresets;
            this.Context = context;

            InitializeComponent();

            var uniqueValMapIDs = Context.TraitValues.AsEnumerable().Select(x => x.ValueMapID).Distinct();
            foreach (var mapID in uniqueValMapIDs)
            {
                TakenValueMapIDs.Add(mapID);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length == 0)
            {
                buttonOK.Enabled = false;
            }
            else if (TakenValueMapIDs.Contains(textBox1.Text.Trim()))
            {
                buttonOK.Enabled = false;
                labelWarn.Visible = true;
            }
            else
            {
                buttonOK.Enabled = true;
                labelWarn.Visible = false;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Result = textBox1.Text.Trim();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (buttonOK.Enabled) buttonOK.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
    }
}
