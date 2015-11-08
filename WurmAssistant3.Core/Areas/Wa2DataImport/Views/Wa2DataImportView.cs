using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Contracts;
using AldursLab.WurmAssistant3.Core.WinForms;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Views
{
    public partial class Wa2DataImportView : ExtendedForm
    {
        private readonly IWa2DataImporter wa2DataImporter;
        private readonly ILogger logger;

        public Wa2DataImportView(IWa2DataImporter wa2DataImporter, ILogger logger)
        {
            if (wa2DataImporter == null) throw new ArgumentNullException("wa2DataImporter");
            if (logger == null) throw new ArgumentNullException("logger");
            this.wa2DataImporter = wa2DataImporter;
            this.logger = logger;
            InitializeComponent();
        }

        private void selectFileBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var path = openFileDialog.FileName;
                    wa2DataImporter.ImportFromFile(path);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Data import error");
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
