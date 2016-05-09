using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.Modules;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.ImportExport.Legacy;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Utils.WinForms;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Views
{
    public partial class FormGrangerImportExport : ExtendedForm
    {
        private GrangerContext _context;
        readonly ILogger logger;

        public FormGrangerImportExport(GrangerContext context, [NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _context = context;
            this.logger = logger;
            InitializeComponent();
            comboBoxExportedHerd.Items.AddRange(context.Herds.Select(x => x.ToString()).ToArray());
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            try
            {
                var exporter = new HerdExporter();
                var xml = exporter.CreateXML(_context, comboBoxExportedHerd.Text.Trim());
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    xml.Save(saveFileDialog1.FileName);
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
                logger.Error(ex, "problem exporting herd");
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var importer = new HerdImporter();
                    importer.ImportHerd(_context, textBoxImportedHerd.Text, openFileDialog1.FileName);
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
                logger.Error(ex, "problem importing herd");
            }
        }

        private void textBoxImportedHerd_TextChanged(object sender, EventArgs e)
        {
            if (_context.Herds.Any(x => x.HerdID == textBoxImportedHerd.Text.Trim()))
            {
                labelImportError.Text = "This herd already exists";
            }
            else
            {
                labelImportError.Text = "";
            }
        }
    }
}
