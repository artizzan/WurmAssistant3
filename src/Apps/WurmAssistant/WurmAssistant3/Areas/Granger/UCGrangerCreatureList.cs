using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.CreatureEdit;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using BrightIdeasSoftware;
using Castle.Core.Internal;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public partial class UcGrangerCreatureList : UserControl
    {
        FormGrangerMain mainForm;
        GrangerContext context;
        IWurmApi wurmApi;
        ILogger logger;
        CreatureColorDefinitions creatureColorDefinitons;

        List<Creature> currentCreatures = new List<Creature>();
        List<HerdEntity> activeHerds = new List<HerdEntity>();
        Creature selectedSingleCreature = null;
        Creature[] lastSelectedCreatures = null;
        readonly DateTime treshholdDtValueForBirthDate = new DateTime(1990, 1, 1);
        readonly TimeSpan treshholdTsValueForExactAge = DateTime.Now - new DateTime(1990, 1, 1);
        readonly int treshholdDaysValueForExactAge = (int)(DateTime.Now - new DateTime(1990, 1, 1)).TotalDays;

        bool _debugMainFormAssigned = false;
        bool listViewIsBeingUpdated = false;

        CreatureColorMenuManager creatureColorMenuManager;

        public UcGrangerCreatureList()
        {
            InitializeComponent();
        }

        public void Init(
            [NotNull] FormGrangerMain mainForm,
            [NotNull] GrangerContext context,
            [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi,
            [NotNull] CreatureColorDefinitions creatureColorDefinitons)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (creatureColorDefinitons == null) throw new ArgumentNullException(nameof(creatureColorDefinitons));
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.mainForm = mainForm;
            this.context = context;
            this.creatureColorDefinitons = creatureColorDefinitons;

            _debugMainFormAssigned = true;

            if (this.mainForm.Settings.AdjustForDarkThemes)
            {
                MakeDarkHighContrastFriendly();
            }

            if (mainForm.Settings.CreatureListState != null)
            {
                this.objectListView1.RestoreState(mainForm.Settings.CreatureListState);
            }

            SetupOlvCustomizations();

            this.context.OnHerdsModified += Context_OnHerdsModified;
            this.context.OnCreaturesModified += ContextOnCreaturesModified;
            this.mainForm.GrangerUserViewChanged += MainForm_UserViewChanged;
            this.mainForm.GrangerAdvisorChanged += MainForm_Granger_AdvisorChanged;
            this.mainForm.GrangerValuatorChanged += MainForm_Granger_ValuatorChanged;

            creatureColorMenuManager = new CreatureColorMenuManager(setColorToolStripMenuItem,
                creatureColorDefinitons,
                SetColorMenuItemClickAction);

            UpdateCurrentCreaturesData();
            UpdateDataForView();
            timer1.Enabled = true;
        }

        void SetColorMenuItemClickAction(string creatureColorId)
        {
            UpdateCreaturesColors(creatureColorDefinitons.GetForId(creatureColorId));
        }

        void SetupOlvCustomizations()
        {
            // GroupKeyGetter needs to be IComparable (non generic) to properly support ordering,
            // GroupKeyToTitleConverter converts that key into something more meaningful to display
            // AspectToStringConverter converts aspect into custom display string

            olvColumnPairedWith.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                Creature mate = creature.GetMate();
                if (mate != null)
                {
                    string[] output = new string[2] { creature.ToString(), mate.ToString() };
                    return string.Join(" + ", output.OrderBy(y => y));
                }
                else return "Free";
            });

            olvColumnValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                if (creature.Value >= 0) return "Positive";
                else return "Negative";
            });
            olvColumnValue.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                return x.ToString();
            });

            olvColumnBreedValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                double? val = creature.BreedValueAspect;
                if (val == null) return "Being Compared / Not comparing";
                else return "Candidates";
            });
            olvColumnBreedValue.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is double)
                {
                    double val = (double)x;
                    if (val == double.NegativeInfinity) return "-inf";
                    else return val.ToString("F0");
                }
                else if (x == null) return string.Empty;
                else return x.ToString();
            });

            olvColumnTraits.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                var traits = creature.Traits;
                CreatureTrait[] positives = CreatureTrait.GetGoodTraits(traits, this.mainForm.CurrentValuator);
                CreatureTrait[] negatives = CreatureTrait.GetBadTraits(traits, this.mainForm.CurrentValuator);
                return string.Format("Good: {0}, Neutral: {1}, Bad: {2}",
                    positives.Length,
                    traits.Length - positives.Length - negatives.Length,
                    negatives.Length);
            });

            olvColumnAge.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                CreatureAge age = ((Creature)x).Age;
                return (int)age.CreatureAgeId;
            });
            olvColumnAge.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
            {
                int result = (int)x;
                return (new CreatureAge(result.ToString()).ToString());
            });

            olvColumnNotInMoodFor.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                TimeSpan tms = creature.NotInMoodForAspect;// notInMoodUntil - DateTime.Now;
                if (tms.TotalDays >= 0)
                {
                    return 0; // "Not in mood";
                }
                else
                {
                    return 1; // "In mood";
                }
            });
            olvColumnNotInMoodFor.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)x;
                    if (ts.TotalDays >= 0)
                        return ts.ToStringCompact();
                    else return string.Empty;
                }
                else return string.Empty;
            });
            olvColumnNotInMoodFor.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
            {
                int result = (int)x;
                switch (result)
                {
                    case 0:
                        return "Not in mood";
                    case 1:
                        return "In mood";
                    default:
                        return "";
                }
            });


            olvColumnGroomedAgo.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                TimeSpan tms = creature.GroomedAgoAspect; // DateTime.Now - groomedon;
                if (tms.TotalHours <= 1)
                {
                    return "Less than 1 hour ago";
                }
                else
                {
                    return "More than 1 hour ago";
                }
            });
            olvColumnGroomedAgo.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)x;
                    if (ts < this.mainForm.Settings.ShowGroomingTime)
                        return ts.ToStringCompact();
                    else return string.Empty;
                }
                else return string.Empty;
            });
            olvColumnGroomedAgo.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
            {
                return x.ToString();
            });


            olvColumnPregnantFor.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                DateTime pregnantUntil = creature.PregnantUntil;
                TimeSpan tms = pregnantUntil - DateTime.Now;

                if (tms.Ticks <= 0)
                {
                    if (tms.TotalDays >= -1) return 2; // "Gave birth within last 24h";
                    else if (tms.TotalDays >= -7) return 3; // "Gave birth in last 7 days";
                    else return 4; // "Not pregnant";   
                }
                else if (tms.TotalDays <= 1)
                {
                    return 0; // "Will give birth within 24h";
                }
                else
                {
                    return 1; // "Will give birth in more than 24h";
                }
            });
            olvColumnPregnantFor.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)x;
                    if (ts.Ticks >= 0)
                        return ts.ToStringCompact();
                    else return string.Empty;
                }
                else return string.Empty;
            });
            olvColumnPregnantFor.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
            {
                int group = (int)x;
                switch (group)
                {
                    case 0:
                        return "Will give birth within 24h";
                    case 1:
                        return "Will give birth in more than 24h";
                    case 2:
                        return "Gave birth within last 24h";
                    case 3:
                        return "Gave birth within last 7 days";
                    case 4:
                        return "Not pregnant";
                    default:
                        return "";
                }
            });


            olvColumnBirthDate.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                return creature.BirthDate.Date;
            });
            olvColumnBirthDate.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is DateTime)
                {
                    DateTime dt = (DateTime)x;
                    if (dt < treshholdDtValueForBirthDate)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return dt.ToString(CultureInfo.CurrentCulture);
                    }
                }
                else return string.Empty;
            });
            olvColumnBirthDate.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
            {
                DateTime dt = (DateTime)x;
                if (dt < treshholdDtValueForBirthDate)
                {
                    return "No birth date available";
                }
                else
                {
                    return dt.ToShortDateString();
                }
            });


            olvColumnExactAge.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Creature creature = (Creature)x;
                var val = (int)creature.ExactAgeAspect.TotalDays;
                if (val > treshholdDaysValueForExactAge) return int.MaxValue;
                else return val;
            });
            olvColumnExactAge.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)x;
                    if (ts > treshholdTsValueForExactAge)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return ts.ToStringCompact();
                    }
                }
                else return string.Empty;
            });
            olvColumnExactAge.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
            {
                int gkey = (int)x;
                if (gkey > treshholdDaysValueForExactAge)
                {
                    return "No birth date available";
                }
                else
                {
                    return string.Format("{0} real days", gkey);
                }
            });
        }

        public Creature[] SelectedCreatures
        {
            get
            {
                var array = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
                return array;
            }
        }

        public Creature SelectedSingleCreature
        {
            get { return selectedSingleCreature; }
            set
            {
                selectedSingleCreature = value;
                //checking for null to prevent designer issue, which tries to set it to null initially
                if (mainForm != null)
                {
                    mainForm.TriggerSelectedSingleCreatureChanged();
                    UpdateDataForView();
                }
            }
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

        void MainForm_Granger_ValuatorChanged(object sender, EventArgs e)
        {
            UpdateDataForView();
        }

        void MainForm_Granger_AdvisorChanged(object sender, EventArgs e)
        {
            UpdateDataForView();
        }

        void ContextOnCreaturesModified(object sender, EventArgs e)
        {
            UpdateCurrentCreaturesData();
            UpdateDataForView();
        }

        void MainForm_UserViewChanged(object sender, UserViewChangedEventArgs e)
        {
            if (e.HerdViewVisible) tableLayoutPanel1.RowStyles[0].Height = 0;
            else tableLayoutPanel1.RowStyles[0].Height = 30;
        }

        void Context_OnHerdsModified(object sender, EventArgs e)
        {
            UpdateCurrentCreaturesData();
            UpdateDataForView();
        }

        void UpdateCurrentCreaturesData()
        {
            activeHerds = context.Herds.AsEnumerable().Where(x => x.Selected == true).OrderBy(x => x.HerdId).ToList();

            textBoxHerds.Text = string.Join(", ", activeHerds.Select(x => x.HerdId));

            // take creatures only from selected herds
            currentCreatures = context.Creatures
                .AsEnumerable()
                .Where(x => activeHerds
                    .Select(y => y.HerdId)
                    .Contains(x.Herd))
                .Select(x => new Creature(mainForm, x, context, creatureColorDefinitons))
                .ToList();
        }

        void UpdateDataForView()
        {
            listViewIsBeingUpdated = true;

            try
            {
                // updating breeding advisor feedback
                double maxValue = 0;
                double minValue = 0;

                foreach (var creature in currentCreatures)
                {
                    creature.RebuildCachedBreedValue();
                    if (creature.CachedBreedValue.HasValue)
                    {
                        if (double.IsInfinity(creature.CachedBreedValue.Value)) continue;

                        if (creature.CachedBreedValue > maxValue) maxValue = creature.CachedBreedValue.Value;
                        if (creature.CachedBreedValue < minValue) minValue = creature.CachedBreedValue.Value;
                    }
                }

                foreach (var creature in currentCreatures)
                {
                    creature.ClearColorHints();
                    if (!mainForm.Settings.DisableRowColoring)
                    {
                        creature.RefreshBreedHintColor(minValue, maxValue);
                    }
                }

                // #fix
                // hack fix for an AccessViolationException after clicking object list view group header. 
                // Issue is caused by rebuilding the objectlist during group header click handling, 
                // which itself seems to be asynchronous.
                var anyGroupSelected = objectListView1.OLVGroups?.Any(group => group.Selected) ?? false;
                var listCountChanged = currentCreatures.Count != objectListView1.Objects?.Cast<object>().Count();

                if (!anyGroupSelected || listCountChanged)
                {
                    objectListView1.SetObjects(currentCreatures, true);
                }
                else
                {
                    objectListView1.RefreshObjects(currentCreatures);
                }
                // #endfix
            }
            finally
            {
                listViewIsBeingUpdated = false;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListView1.SelectedObjects.Count > 1)
            {
                MessageBox.Show("Please first select a creature for editing.");
            }
            else
            {
                objectListView1.SelectedObjects
                               .Cast<Creature>()
                               .ToArray()
                               .ForEach(creature =>
                               {
                                   FormCreatureViewEdit ui = new FormCreatureViewEdit(
                                       mainForm,
                                       context,
                                       logger,
                                       wurmApi,
                                       creature,
                                       CreatureViewEditOpType.Edit,
                                       creature.HerdAspect,
                                       creatureColorDefinitons);
                                   ui.ShowDialogCenteredOnForm(mainForm);
                               });
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
            FormChooseHerd ui = new FormChooseHerd(context);
            if (ui.ShowDialogCenteredOnForm(mainForm) == DialogResult.OK)
            {
                string herdId = ui.Result;

                var targetHerd =
                    context.Creatures
                           .Where(x => x.Herd == herdId)
                           .Select(x => new Creature(mainForm, x, context, creatureColorDefinitons))
                           .ToArray();

                List<Creature> nonuniqueCreatures = new List<Creature>();
                foreach (var creature in selected)
                {
                    foreach (var otherCreature in targetHerd)
                    {
                        if (creature != otherCreature)
                        {
                            if (creature.IsNotUniquelyIdentifiableWhenComparedTo(otherCreature))
                            {
                                nonuniqueCreatures.Add(creature);
                            }
                        }
                    }
                }

                if (nonuniqueCreatures.Count > 0)
                {
                    MessageBox.Show(
                        "Unable to change herd for selected creatures. Some creatures have same identity (name and server if known):\r\n"
                        + string.Join(", ", nonuniqueCreatures.Select(x => x.ToString())));
                }
                else
                {
                    foreach (var creature in selected)
                    {
                        creature.Herd = herdId;
                    }
                    context.SubmitChanges();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().OrderBy(x => x.NameAspect).ToArray();
            if (MessageBox.Show("Creatures will be permanently deleted:\r\n" +
                string.Join(",\r\n", (IEnumerable<Creature>)selected) + "\r\nContinue?",
                "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                context.DeleteCreatures(selected.Select(x => x.Entity).ToArray());
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListView1.SelectedObjects.Count > 1)
            {
                MessageBox.Show("Please select a single creature for viewing.");
            }
            else
            {
                objectListView1.SelectedObjects
                               .Cast<Creature>()
                               .ToArray()
                               .ForEach(creature =>
                               {
                                   FormCreatureViewEdit ui = new FormCreatureViewEdit(
                                       mainForm,
                                       context, 
                                       logger,
                                       wurmApi,
                                       creature,
                                       CreatureViewEditOpType.View,
                                       creature.HerdAspect,
                                       creatureColorDefinitons);
                                   ui.ShowDialogCenteredOnForm(mainForm);
                               });
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible) UpdateDataForView();
        }

        private void UCGrangerCreatureList_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) UpdateDataForView();
        }

        private void objectListView1_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
        {
            if (e.Model != null)
            {
                e.MenuStrip = contextMenuStrip1;
            }
        }

        private void tESTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = string.Join(", ", objectListView1.SelectedObjects.Cast<Creature>());
            MessageBox.Show(result);
        }

        void UpdateCreaturesColors(CreatureColor color)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();

            if (selected.Length > 2)
            {
                if (System.Windows.Forms.MessageBox.Show("Color will be changed for creatures:\r\n" +
                    string.Join(", ", (IEnumerable<Creature>)selected) + "\r\nContinue?",
                    "Confirm", System.Windows.Forms.MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
            }

            foreach (var creature in selected)
            {
                creature.Color = color;
            }
            context.SubmitChanges();
        }

        private void diseasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTag("diseased");
        }

        private void deadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTag("dead");
        }

        private void soldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTag("sold");
        }

        private void fatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTag("fat");
        }

        private void starvingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleTag("starving");
        }

        void ToggleTag(string tag)
        {
            objectListView1.SelectedObjects.Cast<Creature>().ToArray().ForEach(
                creature =>
                {
                    bool tagState = creature.CheckTag(tag);
                    creature.SetTag(tag, !tagState);
                });

            context.SubmitChanges();
        }

        private void objectListView1_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            SaveStateToSettings();
        }

        private void objectListView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            SaveStateToSettings();
        }

        public void SaveStateToSettings()
        {
            // On Windows XP ordering events fire before mainform is assigned, 
            // due to mainform being assigned outside constructor.
            // Wurm Assistant is no longer Win XP compatible, but keeping this safeguard, just in case.
            if (!_debugMainFormAssigned && mainForm == null) return;

            var settings = this.objectListView1.SaveState();
            try
            {
                mainForm.Settings.CreatureListState = settings;
            }
            catch (Exception exception)
            {
                logger.Error(exception,
                    $"Something went wrong at saving creaturelist layout, mainform: {mainForm?.ToString() ?? "NULL"}");
            }
        }

        private void setMateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
            if (selected.Length != 2)
            {
                MessageBox.Show("Please select exactly 2 creatures for pairing together.");
                return;
            }

            Creature firstCreatureMate = selected[0].GetMate();
            Creature secondCreatureMate = selected[1].GetMate();
            if (firstCreatureMate != null || secondCreatureMate != null)
            {
                if (MessageBox.Show("At least one of the selected creatures already has a mate. Change their mates anyway?",
                    "Confirm", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    firstCreatureMate?.SetMate(null);
                    secondCreatureMate?.SetMate(null);
                }
            }

            selected[0].SetMate(selected[1]);
            selected[1].SetMate(selected[0]);
            context.SubmitChanges();
        }

        private void clearMateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();

            if (selected.Length > 0)
            {
                if (MessageBox.Show("Mates will be cleared for creatures:\r\n" +
                    string.Join(",\r\n", (IEnumerable<Creature>)selected) + "\r\nContinue?",
                    "Confirm", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }

                foreach (var creature in selected)
                {
                    creature.SetMate(null);
                }

                context.SubmitChanges();
            }
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged();
        }

        void SelectionChanged()
        {
            var newSelectedCreatures = SelectedCreatures;
            bool changed = SelectionChangedCheck(lastSelectedCreatures, newSelectedCreatures);
            if (!listViewIsBeingUpdated && changed)
            {
                var selectedCreatures = newSelectedCreatures;
                if (selectedCreatures.Length == 1)
                {
                    var creature = selectedCreatures[0];
                    if (!creature.Equals(SelectedSingleCreature))
                    {
                        SelectedSingleCreature = selectedCreatures[0];
                    }
                }
                else
                {
                    SelectedSingleCreature = null;
                }
            }
            lastSelectedCreatures = newSelectedCreatures;
        }

        private bool SelectionChangedCheck(Creature[] previouslySelectedCreatures, Creature[] newSelectedCreatures)
        {
            if (previouslySelectedCreatures == null && newSelectedCreatures == null) return false;
            if (previouslySelectedCreatures == null || newSelectedCreatures == null) return true;
            if (previouslySelectedCreatures.Length != newSelectedCreatures.Length) return true;

            foreach (var creature in previouslySelectedCreatures)
            {
                if (!newSelectedCreatures.Contains(creature)) return true;
            }

            return false;
        }

        private void objectListView1_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            Creature creature = (Creature)e.Model;
            System.Drawing.Color? creatureColorId = creature.CreatureBestCandidateColor;
            if (creatureColorId != null)
            {
                e.Item.BackColor = creatureColorId.Value;
            }
            else
            {
                creatureColorId = creature.BreedHintColor;
                if (creatureColorId != null) e.Item.BackColor = creatureColorId.Value;
            }
        }

        private void objectListView1_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (e.Column == olvColumnColor)
            {
                Creature creature = (Creature)e.Model;
                System.Drawing.Color? color = creature.CreatureColorBkColor;
                if (color != null)
                {
                    e.SubItem.BackColor = color.Value;
                    e.SubItem.ForeColor = e.SubItem.BackColor.GetContrastingBlackOrWhiteColor();
                }
            }
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNotInMood(new TimeSpan(0));
        }

        private void set45MinutesFromNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNotInMood(TimeSpan.FromMinutes(45));
        }

        private void setTo1HourFromNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNotInMood(TimeSpan.FromHours(1));
        }

        private void setTo24HoursFromNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNotInMood(TimeSpan.FromHours(24));
        }

        void SetNotInMood(TimeSpan duration)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();

            if (selected.Length > 0)
            {
                foreach (var creature in selected)
                {
                    creature.NotInMoodUntil = DateTime.Now + duration;
                }

                context.SubmitChanges();
            }
        }

        private void objectListView1_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedSingleCreature != null)
            {
                Creature creature = SelectedSingleCreature;
                FormEditComments ui = new FormEditComments(creature.Comments, creature.Name);
                if (ui.ShowDialogCenteredOnForm(mainForm) == DialogResult.OK)
                {
                    creature.Comments = ui.Result;
                    context.SubmitChanges();
                }
            }
        }

        class CreatureColorMenuManager
        {
            readonly ToolStripMenuItem rootToolStripMenuItem;
            readonly CreatureColorDefinitions creatureColorDefinitions;
            readonly Action<string> menuItemClickAction;

            public CreatureColorMenuManager(
                [NotNull] ToolStripMenuItem rootToolStripMenuItem,
                [NotNull] CreatureColorDefinitions creatureColorDefinitions,
                [NotNull] Action<string> menuItemClickAction)
            {
                if (rootToolStripMenuItem == null) throw new ArgumentNullException(nameof(rootToolStripMenuItem));
                if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));
                if (menuItemClickAction == null) throw new ArgumentNullException(nameof(menuItemClickAction));
                this.rootToolStripMenuItem = rootToolStripMenuItem;
                this.creatureColorDefinitions = creatureColorDefinitions;
                this.menuItemClickAction = menuItemClickAction;

                creatureColorDefinitions.DefinitionsChanged += (o, e) => RebuildItems();
                RebuildItems();
            }

            void RebuildItems()
            {
                rootToolStripMenuItem.DropDownItems.Clear();
                rootToolStripMenuItem.DropDownItems.AddRange(
                    creatureColorDefinitions.GetColors()
                                            .Select(color =>
                                            {
                                                var newItem = new ToolStripMenuItem()
                                                {
                                                    Text = color.DisplayName
                                                };
                                                newItem.Click +=
                                                    (sender, args) => menuItemClickAction.Invoke(color.CreatureColorId);
                                                return newItem;
                                            })
                                            .Cast<ToolStripItem>()
                                            .ToArray());
            }
        }
    }
}
