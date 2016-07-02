using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class UcGrangerTraitView : UserControl
    {
        FormGrangerMain mainForm;
        GrangerContext context;
        TraitViewManager traitViewManager;
        ILogger logger;

        bool _debugMainFormAssigned = false;
        public UcGrangerTraitView()
        {
            InitializeComponent();
        }

        internal void Init(
            [NotNull] FormGrangerMain formGrangerMain, 
            [NotNull] GrangerContext context, 
            [NotNull] ILogger logger)
        {
            if (formGrangerMain == null) throw new ArgumentNullException(nameof(formGrangerMain));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.mainForm = formGrangerMain;
            this.context = context;
            this.logger = logger;
            _debugMainFormAssigned = true;

            if (mainForm.Settings.AdjustForDarkThemes)
            {
                MakeDarkHighContrastFriendly();
            }

            if (mainForm.Settings.TraitViewState != null)
            {
                objectListView1.RestoreState(mainForm.Settings.TraitViewState);
            }
            traitViewManager = new TraitViewManager(mainForm, this.context, objectListView1);
        }

        private void MakeDarkHighContrastFriendly()
        {
            objectListView1.HeaderUsesThemes = false;
            objectListView1.HeaderFormatStyle = new HeaderFormatStyle()
            {
                Normal = new HeaderStateStyle()
                {
                    ForeColor = Color.Yellow
                },
                Hot = new HeaderStateStyle()
                {
                    ForeColor = Color.Yellow
                },
                Pressed = new HeaderStateStyle()
                {
                    ForeColor = Color.Yellow
                },
            };
        }

        public void SaveStateToSettings()
        {
            if (!_debugMainFormAssigned && mainForm == null) return;

            try
            {
                mainForm.Settings.TraitViewState = objectListView1.SaveState();
            }
            catch (Exception exception)
            {
                logger.Error(exception,
                    $"Something went wrong on saving trait list state, mainform: {mainForm?.ToString() ?? "NULL"}");
            }
        }

        private void fullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.TraitViewDisplayMode = TraitDisplayMode.Full;
        }

        private void compactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.TraitViewDisplayMode = TraitDisplayMode.Compact;
        }

        private void shortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.TraitViewDisplayMode = TraitDisplayMode.Shortcut;
        }

        private void objectListView1_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            SaveStateToSettings();
        }

        private void objectListView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            SaveStateToSettings();
        }
    }
}
