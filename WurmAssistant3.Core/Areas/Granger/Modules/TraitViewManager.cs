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

            public HorseTrait Trait;
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
        HorseTrait[] AllTraits;

        public TraitViewManager(FormGrangerMain mainForm, GrangerContext context, ObjectListView listview)
        {
            MainForm = mainForm;
            Context = context;
            OLV = listview;

            OLV.FormatRow += OLV_FormatRow;

            AllTraits = HorseTrait.GetAllTraitEnums().Select(x => new HorseTrait(x)).ToArray();
            BuildClearTraitView();

            listview.SetObjects(Items);
            Decide();

            MainForm.Granger_SelectedSingleHorseChanged += MainForm_Granger_SelectedHorsesChanged;
            MainForm.Granger_ValuatorChanged += MainForm_Granger_ValuatorChanged;
            MainForm.Granger_TraitViewDisplayModeChanged += MainForm_Granger_TraitViewDisplayModeChanged;
            Context.OnHerdsModified += Context_OnHerdsModified;
            Context.OnHorsesModified += Context_OnHorsesModified;
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

        void Context_OnHorsesModified(object sender, EventArgs e)
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

        void MainForm_Granger_SelectedHorsesChanged(object sender, EventArgs e)
        {
            Decide();
        }

        void Decide()
        {
            var selected = MainForm.SelectedSingleHorse;
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

        private void BuildTraitView(Horse horse)
        {
            HorseTrait[] currentHorseTraits = horse.Traits;
            Items.Clear();
            foreach (var trait in AllTraits)
            {
                Items.Add(new TraitItem()
                {
                    DisplayMode = MainForm.TraitViewDisplayMode,
                    Trait = trait,
                    Exists = currentHorseTraits.Contains(trait),
                    Unknown = trait.IsUnknownForThisHorse(horse),
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
