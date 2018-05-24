using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.ImportExport.Legacy;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class FormGrangerImportExport : ExtendedForm
    {
        readonly GrangerContext context;
        readonly ILogger logger;

        public FormGrangerImportExport([NotNull] GrangerContext context, [NotNull] ILogger logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.context = context;
            this.logger = logger;

            InitializeComponent();

            comboBoxExportedHerd.Items.AddRange(context.Herds.Select(x => x.ToString()).Cast<object>().ToArray());
        }

        private void buttonExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                var exporter = new HerdExporter();
                var xml = exporter.CreateXml(context, comboBoxExportedHerd.Text.Trim());
                if (saveXmlFileDialog.ShowDialog() == DialogResult.OK)
                {
                    xml.Save(saveXmlFileDialog.FileName);
                    MessageBox.Show("Export completed");
                }
            }
            catch (GrangerException ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex, "problem at exporting herd");
            }
        }

        private void buttonExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                var exporter = new HerdExporter();
                var fileText = exporter.CreateCsv(context, comboBoxExportedHerd.Text.Trim());
                if (saveCsvFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveCsvFileDialog.FileName, fileText);
                    MessageBox.Show("Export completed");
                }
            }
            catch (GrangerException ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex, "problem at exporting herd");
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var importer = new HerdImporter();
                    importer.ImportHerd(context, textBoxImportedHerd.Text, openFileDialog1.FileName);
                    MessageBox.Show("Import completed");
                }
            }
            catch (GrangerException ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex, "problem at importing herd");
            }
        }

        private void textBoxImportedHerd_TextChanged(object sender, EventArgs e)
        {
            labelImportError.Text = context.Herds.Any(x => x.HerdId == textBoxImportedHerd.Text.Trim())
                ? "This herd already exists"
                : "";
        }
    }
}
