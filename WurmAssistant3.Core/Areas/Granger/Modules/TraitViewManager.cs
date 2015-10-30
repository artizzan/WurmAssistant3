using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using BrightIdeasSoftware;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules
{
    public class TraitViewManager
    {
        public enum TraitDisplayMode { Full, Compact, Shortcut }
        
        public class TraitItem
        {
            public TraitDisplayMode DisplayMode = TraitDisplayMode.Full;

            public CreatureTrait Trait;
            public bool Exists;
            public bool Unknown;
            public int Value;

            public string TraitAspect
            {
                get
                {
                    if (DisplayMode == TraitDisplayMode.Compact) return Trait.ToCompactString();
                    else if (DisplayMode == TraitDisplayMode.Shortcut) return Trait.ToShortcutString();
                    else return Trait.ToString();
                }
            }
            public string HasAspect
            {
                get
                {
                    if (Unknown) return "?";
                    else if (Exists) return "YES";
                    else return string.Empty;
                }
            }
            public string ValueAspect { get { return Value.ToString(); } }

            public System.Drawing.Color? BackColor
            {
                get
                {
                    if (DisableBackgroundColors)
                    {
                        return null;
                    }

                    if (Exists) 
                    {
                        if (Value > 0) return System.Drawing.Color.LightGreen;
                        else if (Value < 0) return System.Drawing.Color.OrangeRed;
                    }
                    else if (Unknown)
                    {
                        if (Value > 0) return System.Drawing.Color.LightBlue;
                        else if (Value < 0) return System.Drawing.Color.Yellow;
                    }
                    return null; //no coloring for this item
                }
            }

            public bool DisableBackgroundColors { get; set; }
        }

        ObjectListView OLV;
        FormGrangerMain MainForm;
        GrangerContext Context;

        List<TraitItem> Items = new List<TraitItem>();
        CreatureTrait[] AllTraits;

        public TraitViewManager(FormGrangerMain mainForm, GrangerContext context, ObjectListView listview)
        {
            MainForm = mainForm;
            Context = context;
            OLV = listview;

            OLV.FormatRow += OLV_FormatRow;

            AllTraits = CreatureTrait.GetAllTraitEnums().Select(x => new CreatureTrait(x)).ToArray();
            BuildClearTraitView();

            listview.SetObjects(Items);
            Decide();

            MainForm.Granger_SelectedSingleCreatureChanged += MainFormGrangerSelectedCreaturesChanged;
            MainForm.Granger_ValuatorChanged += MainForm_Granger_ValuatorChanged;
            MainForm.Granger_TraitViewDisplayModeChanged += MainForm_Granger_TraitViewDisplayModeChanged;
            Context.OnHerdsModified += Context_OnHerdsModified;
            Context.OnEntitiesModified += ContextOnEntitiesModified;
            Context.OnTraitValuesModified += Context_OnTraitValuesModified;
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

        void ContextOnEntitiesModified(object sender, EventArgs e)
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
            var selected = MainForm.SelectedSingleCreature;
            if (selected != null)
            {
                BuildTraitView(selected);
            }
            else
            {
                BuildClearTraitView();
            }
            OLV.BuildList();
        }

        private void BuildTraitView(Creature creature)
        {
            CreatureTrait[] currentCreatureTraits = creature.Traits;
            Items.Clear();
            foreach (var trait in AllTraits)
            {
                Items.Add(new TraitItem()
                {
                    DisplayMode = MainForm.TraitViewDisplayMode,
                    Trait = trait,
                    Exists = currentCreatureTraits.Contains(trait),
                    Unknown = trait.IsUnknownForThisCreature(creature),
                    Value = trait.GetTraitValue(MainForm.CurrentValuator),
                    DisableBackgroundColors = MainForm.Settings.DisableRowColoring
                });
            }
        }

        private void BuildClearTraitView()
        {
            Items.Clear();
            foreach (var trait in AllTraits)
            {
                Items.Add(new TraitItem()
                {
                    DisplayMode = MainForm.TraitViewDisplayMode,
                    Trait = trait,
                    Exists = false,
                    Unknown = false,
                    Value = MainForm.CurrentValuator.GetValueForTrait(trait),
                    DisableBackgroundColors = MainForm.Settings.DisableRowColoring
                });
            }
        }
    }
}
