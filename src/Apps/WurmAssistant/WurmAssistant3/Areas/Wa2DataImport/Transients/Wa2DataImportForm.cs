using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;

namespace AldursLab.WurmAssistant3.Areas.Wa2DataImport.Transients
{
    public partial class Wa2DataImportForm : ExtendedForm
    {
        private readonly IWa2DataImporter wa2DataImporter;
        private readonly ILogger logger;

        public Wa2DataImportForm(IWa2DataImporter wa2DataImporter, ILogger logger)
        {
            if (wa2DataImporter == null) throw new ArgumentNullException("wa2DataImporter");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wa2DataImporter = wa2DataImporter;
            this.logger = logger;
            InitializeComponent();
        }

        private async void selectFileBtn_Click(object sender, EventArgs e)
        {
            var oldTxt = selectFileBtn.Text;
            try
            {
                selectFileBtn.Enabled = false;
                selectFileBtn.Text = "Importing, please finish before trying again.";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = openFileDialog.FileName;
                    await wa2DataImporter.ImportFromFileAsync(path);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Data import error");
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                selectFileBtn.Enabled = true;
                selectFileBtn.Text = oldTxt;
            }
        }
    }
}
