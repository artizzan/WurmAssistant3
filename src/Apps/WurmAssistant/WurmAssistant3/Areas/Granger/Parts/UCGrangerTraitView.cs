using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts
{
    public partial class UCGrangerTraitView : UserControl
    {
        FormGrangerMain MainForm;
        GrangerContext Context;
        TraitViewManager Manager;
        ILogger logger;

        bool _debug_MainFormAssigned = false;
        public UCGrangerTraitView()
        {
            InitializeComponent();
        }

        internal void Init([NotNull] FormGrangerMain formGrangerMain, [NotNull] GrangerContext context, [NotNull] ILogger logger)
        {
            if (formGrangerMain == null) throw new ArgumentNullException("formGrangerMain");
            if (context == null) throw new ArgumentNullException("context");
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            MainForm = formGrangerMain;
            _debug_MainFormAssigned = true;

            if (MainForm.Settings.AdjustForDarkThemes)
            {
                MakeDarkHighContrastFriendly();
            }

            Context = context;
            if (MainForm.Settings.TraitViewState != null) objectListView1.RestoreState(MainForm.Settings.TraitViewState);
            Manager = new TraitViewManager(MainForm, Context, objectListView1);
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
            if (!_debug_MainFormAssigned && MainForm == null) return;

            try
            {
                MainForm.Settings.TraitViewState = objectListView1.SaveState();
            }
            catch (Exception _e)
            {
                logger.Error(_e, "Something went wrong when trying to save trait list state, mainform null: " + (MainForm == null));
            }
        }

        private void fullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.TraitViewDisplayMode = TraitViewManager.TraitDisplayMode.Full;
        }

        private void compactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.TraitViewDisplayMode = TraitViewManager.TraitDisplayMode.Compact;
        }

        private void shortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.TraitViewDisplayMode = TraitViewManager.TraitDisplayMode.Shortcut;
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
