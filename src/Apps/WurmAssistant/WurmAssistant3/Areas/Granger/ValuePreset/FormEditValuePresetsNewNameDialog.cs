using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Granger.ValuePreset
{
    public partial class FormEditValuePresetsNewNameDialog : ExtendedForm
    {
        FormEditValuePresets formEditValuePresets;

        public string Result { get; private set; }

        readonly HashSet<string> takenValueMapIDs = new HashSet<string>();

        public FormEditValuePresetsNewNameDialog(FormEditValuePresets formEditValuePresets, GrangerContext context)
        {
            this.formEditValuePresets = formEditValuePresets;

            InitializeComponent();

            var uniqueValMapIDs = context.TraitValues.AsEnumerable().Select(x => x.ValueMapId).Distinct();
            foreach (var mapId in uniqueValMapIDs)
            {
                takenValueMapIDs.Add(mapId);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length == 0)
            {
                buttonOK.Enabled = false;
            }
            else if (takenValueMapIDs.Contains(textBox1.Text.Trim()))
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
