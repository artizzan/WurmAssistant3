using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormEditCreatureColorDialog : Form
    {
        public FormEditCreatureColorDialog()
        {
            InitializeComponent();
        }

        public string Id { get => this.textBoxId.Text; set => this.textBoxId.Text = value; }
        public Color Color { get => this.textBoxColor.BackColor; set => this.textBoxColor.BackColor = value; }
        public string WurmLogText { get => this.textBoxWurmLogText.Text; set => this.textBoxWurmLogText.Text = value; }
        public string DisplayName { get => this.textBoxDisplayName.Text; set => this.textBoxDisplayName.Text = value; }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonChangeColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.Color = colorDialog.Color;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
