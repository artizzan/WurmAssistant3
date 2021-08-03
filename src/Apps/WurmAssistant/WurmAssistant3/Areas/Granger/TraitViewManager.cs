using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public class TraitViewManager
    {
        readonly ObjectListView olv;
        readonly FormGrangerMain mainForm;
        readonly GrangerContext context;

        readonly List<TraitItem> items = new List<TraitItem>();
        readonly CreatureTrait[] allTraits;

        public TraitViewManager(
            [NotNull] FormGrangerMain mainForm,
            [NotNull] GrangerContext context,
            [NotNull] ObjectListView listview)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (listview == null) throw new ArgumentNullException(nameof(listview));
            this.mainForm = mainForm;
            this.context = context;
            this.olv = listview;

            olv.FormatRow += OLV_FormatRow;

            allTraits = CreatureTrait.GetAllTraitEnums().Select(x => new CreatureTrait(x)).ToArray();
            BuildClearTraitView();

            listview.SetObjects(items);
            Decide();

            this.mainForm.GrangerSelectedSingleCreatureChanged += MainFormGrangerSelectedCreaturesChanged;
            this.mainForm.GrangerValuatorChanged += MainForm_Granger_ValuatorChanged;
            this.mainForm.GrangerTraitViewDisplayModeChanged += MainForm_Granger_TraitViewDisplayModeChanged;
            this.context.OnHerdsModified += Context_OnHerdsModified;
            this.context.OnCreaturesModified += ContextOnCreaturesModified;
            this.context.OnTraitValuesModified += Context_OnTraitValuesModified;
        }

        void OLV_FormatRow(object sender, FormatRowEventArgs e)
        {
            TraitItem model = (TraitItem)e.Model;
            var bkcolor = model.BackColor;
            if (bkcolor != null) e.Item.BackColor = bkcolor.Value;
        }

        void MainForm_Granger_TraitViewDisplayModeChanged(object sender, EventArgs e)
        {
            Decide();
        }

        void Context_OnTraitValuesModified(object sender, EventArgs e)
        {
            Decide();
        }

        void ContextOnCreaturesModified(object sender, EventArgs e)
        {
            Decide();
        }

        void Context_OnHerdsModified(object sender, EventArgs e)
        {
            Decide();
        }

        void MainForm_Granger_ValuatorChanged(object sender, EventArgs e)
        {
            Decide();
        }

        void MainFormGrangerSelectedCreaturesChanged(object sender, EventArgs e)
        {
            Decide();
        }

        void Decide()
        {
            var selected = mainForm.SelectedSingleCreature;
            if (selected != null)
            {
                BuildTraitView(selected);
            }
            else
            {
                BuildClearTraitView();
            }
            olv.BuildList();
        }

        private void BuildTraitView(Creature creature)
        {
            CreatureTrait[] currentCreatureTraits = creature.Traits;
            items.Clear();
            foreach (var trait in allTraits)
            {
                items.Add(new TraitItem()
                {
                    DisplayMode = mainForm.TraitViewDisplayMode,
                    Trait = trait,
                    Exists = currentCreatureTraits.Contains(trait),
                    Value = trait.GetTraitValue(mainForm.CurrentValuator),
                    DisableBackgroundColors = mainForm.Settings.DisableRowColoring
                });
            }
        }

        private void BuildClearTraitView()
        {
            items.Clear();
            foreach (var trait in allTraits)
            {
                items.Add(new TraitItem()
                {
                    DisplayMode = mainForm.TraitViewDisplayMode,
                    Trait = trait,
                    Exists = false,
                    Value = mainForm.CurrentValuator.GetValueForTrait(trait),
                    DisableBackgroundColors = mainForm.Settings.DisableRowColoring
                });
            }
        }
    }
}
