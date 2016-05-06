using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Areas.Wa2DataImport.Modules;

namespace AldursLab.WurmAssistant3.Areas.Wa2DataImport.Views
{
    public partial class ImportMergeAssistantView : Form
    {
        private readonly ILogger logger;
        private TaskCompletionSource<bool> completedCompletionSource = new TaskCompletionSource<bool>(); 

        public ImportMergeAssistantView(IEnumerable<ImportItem> items, ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            InitializeComponent();

            var objects = items.ToArray();
            foreach (var importItem in objects)
            {
                importItem.Resolved += (sender, args) =>
                {
                    objectListView1.RemoveObject(sender);
                    objectListView1.BuildList(true);
                };
            }

            objectListView1.SetObjects(objects);
        }

        public Task Completed { get { return completedCompletionSource.Task; }}

        private void objectListView1_ButtonClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            ResolveItem(e.Model, e.Column == ImportAsNewColumn ? MergeResult.AddAsNew : MergeResult.DoNothing);
        }

        private void ImportAllBtn_Click(object sender, EventArgs e)
        {
            bool resolveError = false;
            foreach (var o in objectListView1.Objects.Cast<object>().ToArray())
            {
                var item = o as ImportItem;
                if (item != null && (item.HasDestination || item.Blocked))
                {
                    // matched or blocked items should be skipped
                    continue;
                }

                if (!ResolveItem(o, MergeResult.AddAsNew, showError:false))
                {
                    resolveError = true;
                }
            }
            if (resolveError)
            {
                MessageBox.Show("At least one item didn't resolve correctly.",
                    "Resolve error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SkipAllExistingBtn_Click(object sender, EventArgs e)
        {
            foreach (var o in objectListView1.Objects.Cast<object>().ToArray())
            {
                var item = o as ImportItem;
                if (item != null && item.HasDestination)
                {
                    ResolveItem(o, MergeResult.DoNothing, showError: false);
                }
            }
        }

        private bool ResolveItem(object model, MergeResult result, bool showError = true)
        {
            var item = model as ImportItem;

            if (item != null)
            {
                try
                {
                    item.MergeResult = result;
                    item.Resolve();
                    if (!objectListView1.Objects.Cast<object>().Any())
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    try
                    {
                        if (showError)
                        {
                            MessageBox.Show(exception.ToString(),
                                "Resolve error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        logger.Error(exception,
                            "ImportMergeAssistantView: resolution error for item: " + item.ToString());
                        return false;
                    }
                    catch (Exception exception2)
                    {
                        logger.Error(exception2, "ImportMergeAssistantView: resolution error, item.ToString failed");
                        return false;
                    }
                }
            }
            else
            {
                if (showError)
                {
                    MessageBox.Show("item was null", "Resolve error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                logger.Error("ImportMergeAssistantView: item was null");
                return false;
            }
        }

        private void ImportMergeAssistantView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK && e.CloseReason == CloseReason.UserClosing)
            {
                if (!ConfirmClose())
                {
                    e.Cancel = true;
                }
            }
            completedCompletionSource.TrySetResult(true);
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if (ConfirmClose())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ConfirmClose()
        {
            bool anyRemaining = objectListView1.Objects.Cast<object>().Any();

            if (anyRemaining)
            {
                if (
                    MessageBox.Show(
                        "Are you sure? Some items are still unresolved, if you continue, they will be ignored. " +
                        "They might be needed for next batch of imported assets (eg. sounds for timers and triggers).",
                        "Confirm",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
