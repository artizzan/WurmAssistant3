using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Csv;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Contracts;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Data.Combat;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.Modules;
using AldursLab.WurmAssistant3.Areas.CombatAssistant.ViewModels;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Properties;
using AldursLab.WurmAssistant3.Utils.WinForms;
using AldursLab.WurmAssistant3.Utils.WinForms.Reusables;
using BrightIdeasSoftware;

namespace AldursLab.WurmAssistant3.Areas.CombatAssistant.Views
{
    public partial class CombatResultsView : ExtendedForm
    {
        readonly ICombatDataSource combatDataSource;
        readonly FeatureSettings featureSettings;
        readonly IProcessStarter processStarter;
        readonly ILogger logger;

        bool refreshRequired = false;

        readonly Dictionary<Tuple<CombatActor, CombatActor>, CombatActorViewModel> viewModelsMap =
            new Dictionary<Tuple<CombatActor, CombatActor>, CombatActorViewModel>();

        readonly TextMatchFilter filter;
        readonly IModelFilter defaultFilter;

        readonly byte[] initialOlvState;

        public CombatResultsView(ICombatDataSource combatDataSource, FeatureSettings featureSettings,
            IProcessStarter processStarter, ILogger logger)
        {
            if (combatDataSource == null) throw new ArgumentNullException("combatDataSource");
            if (featureSettings == null) throw new ArgumentNullException("featureSettings");
            if (processStarter == null) throw new ArgumentNullException("processStarter");
            if (logger == null) throw new ArgumentNullException("logger");
            this.combatDataSource = combatDataSource;
            this.featureSettings = featureSettings;
            this.processStarter = processStarter;
            this.logger = logger;
            InitializeComponent();

            combatDataSource.DataChanged += CombatResultsOnDataChanged;

            filter = new TextMatchFilter(objectListView1,
                "",
                StringComparison.InvariantCultureIgnoreCase);
            filter.Columns = new[] {olvColumn0};

            defaultFilter = objectListView1.ModelFilter;
            rowHeightNup.Value = objectListView1.RowHeight.ConstrainToRange(1, 10000);

            if (featureSettings.CombatResultViewState.Any())
            {
                objectListView1.RestoreState(featureSettings.CombatResultViewState);
            }

            initialOlvState = objectListView1.SaveState();

            RefreshData();
        }

        void CombatResultsOnDataChanged(object sender, EventArgs eventArgs)
        {
            refreshRequired = true;
        }

        private void CombatResultsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            combatDataSource.DataChanged -= CombatResultsOnDataChanged;
            PersistOlvState();
        }

        void PersistOlvState()
        {
            // persist the state only if changed
            // this will still cause conficts when multiple states are modified in multiple windows,
            // but it's not an easy problem to fix
            var olvState = objectListView1.SaveState();
            if (!olvState.SequenceEqual(initialOlvState))
            {
                featureSettings.CombatResultViewState = objectListView1.SaveState();
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (refreshRequired)
            {
                RefreshData();

                refreshRequired = false;
            }
        }

        void RefreshData()
        {
            foreach (var pairStats in combatDataSource.CombatStatus.AllStats)
            {
                {
                    var key = new Tuple<CombatActor, CombatActor>(pairStats.ActorOne, pairStats.ActorTwo);
                    if (!viewModelsMap.ContainsKey(key))
                    {
                        viewModelsMap[key] = new CombatActorViewModel(pairStats, key.Item1.Name);
                    }
                }

                {
                    var key = new Tuple<CombatActor, CombatActor>(pairStats.ActorTwo, pairStats.ActorOne);
                    if (!viewModelsMap.ContainsKey(key))
                    {
                        viewModelsMap[key] = new CombatActorViewModel(pairStats, key.Item1.Name);
                    }
                }
            }

            objectListView1.SetObjects(viewModelsMap.Values.ToArray(), preserveState: true);
        }

        private void nameFilterTbox_TextChanged(object sender, EventArgs e)
        {
            var text = nameFilterTbox.Text ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                filter.ContainsStrings = new List<string>() { text };
                objectListView1.ModelFilter = filter;
            }
            else
            {
                objectListView1.ModelFilter = defaultFilter;
            }

            objectListView1.BuildList(true);
        }

        private void rowHeightNup_ValueChanged(object sender, EventArgs e)
        {
            objectListView1.RowHeight = (int)rowHeightNup.Value;
        }

        private void legendBtn_Click(object sender, EventArgs e)
        {
            UniversalTextDisplayView view = new UniversalTextDisplayView();
            view.Text = "Combat statistics legend";
            view.ContentText = Resources.CombatStatisticsLegend;
            view.ShowCenteredOnForm(this);
        }

        private void CombatResultsView_Load(object sender, EventArgs e)
        {

        }

        private void toCsvBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToCsv();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(exception, "CombatResultsView: Csv export failed");
            }
            
        }

        void ExportToCsv()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var builder = new EnumerableToCsvBuilder<CombatActorViewModel>(viewModelsMap.Values.ToArray())
                .AddMapping("Pair", model => model.CombatPair)
                .AddMapping("Attacker", model => model.AttackerName)
                .AddMapping("Defender", model => model.DefenderName)
                .AddMapping("Target choices", model => model.TargetPrefs)
                .AddMapping("Damage caused", model => model.DamageCaused)
                .AddMapping("Spell triggers", model => model.WeaponSpellAttacks)
                .AddMapping("Misses", model => model.Misses)
                .AddMapping("Glancing blows", model => model.GlancingBlows)
                .AddMapping("Parried", model => model.Parries)
                .AddMapping("Evaded", model => model.Evasions)
                .AddMapping("Shield blocked", model => model.ShieldBlocks)
                .AddMapping("Total hits", model => model.TotalHits.ToString())
                .AddMapping("Total attacks", model => model.TotalMainActorAttacks.ToString())
                .AddMapping("Hit ratio", model => model.HitRatio)
                .AddMapping("Miss ratio", model => model.MissRatio)
                .AddMapping("Glance ratio", model => model.GlanceRatio)
                .AddMapping("Blocked ratio", model => model.BlockRatio)
                .AddMapping("Parried ratio", model => model.ParryRatio)
                .AddMapping("Evaded ratio", model => model.EvadeRatio);
                var csv = builder.BuildCsv();
                var filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, csv);
                processStarter.StartSafe(Path.GetDirectoryName(filePath));
            }
        }
    }
}
