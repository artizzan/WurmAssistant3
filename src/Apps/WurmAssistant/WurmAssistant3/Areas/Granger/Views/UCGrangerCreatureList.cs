using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Granger.Modules;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.CreatureEdit;
using AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Areas.Granger.Views.HorseEdit;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Views
{
    public partial class UCGrangerCreatureList : UserControl
    {
        FormGrangerMain mainForm;
        GrangerContext context;
        IWurmApi wurmApi;

        ILogger logger;

        List<Creature> CurrentCreatures = new List<Creature>(); //cached

        readonly DateTime treshholdDtValueForBirthDate = new DateTime(1990, 1, 1);
        readonly TimeSpan treshholdTsValueForExactAge = DateTime.Now - new DateTime(1990, 1, 1);
        readonly int treshholdDaysValueForExactAge = (int)(DateTime.Now - new DateTime(1990, 1, 1)).TotalDays;

        public Creature[] SelectedCreatures
        {
            get
            {
                var array = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
                return array;
            }
        }

        List<HerdEntity> activeHerds = new List<HerdEntity>(); //cached

        Creature selectedSingleCreature = null;

        public Creature SelectedSingleCreature
        {
            get { return selectedSingleCreature; }
            set
            {
                selectedSingleCreature = value;
                if (mainForm != null) //some designer bug, this prop appear in designer which tries to set it to null initially
                {
                    mainForm.TriggerSelectedSingleCreatureChanged();
                    UpdateDataForView();
                }
            }
        }

        bool _debug_MainFormAssigned = false;

        public UCGrangerCreatureList()
        {
            InitializeComponent();
        }

        public void Init([NotNull] FormGrangerMain mainForm, [NotNull] GrangerContext context, [NotNull] ILogger logger,
            [NotNull] IWurmApi wurmApi)
        {
            if (mainForm == null) throw new ArgumentNullException("mainForm");
            if (context == null) throw new ArgumentNullException("context");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            this.logger = logger;
            this.wurmApi = wurmApi;
            this.mainForm = mainForm;
            _debug_MainFormAssigned = true;

            if (this.mainForm.Settings.AdjustForDarkThemes)
            {
                MakeDarkHighContrastFriendly();
            }

            if (mainForm.Settings.CreatureListState != null)
            {
                this.objectListView1.RestoreState(mainForm.Settings.CreatureListState);
            }
            this.context = context;

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

            //olvColumnValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            //    {
            //        Creature creature = (Creature)x;
            //        return creature.ValueAspect;
            //    });
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

            //olvColumnBreedValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            //    {
            //        Creature creature = (Creature)x;
            //        var result = creature.BreedValueAspect;
            //        return result != null ? result.ToString() : "Not comparing";
            //    });
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

            olvColumnTraitsInspectedAt.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    Creature creature = (Creature)x;
                    var traitinfo = creature.TraitsInspectedAtSkillAspect;
                    if (traitinfo.Skill > CreatureTrait.GetFullTraitVisibilityCap(traitinfo.EpicCurve))
                        return "Fully known";
                    else return "Known partially";
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

            //////////////
            // GroupKeyGetter needs to be sortable to position them correctly,
            // GroupKeyToTitleConverter converts that key into something more meaningful to display
            // AspectToStringConverter converts aspect into display string in special way

            olvColumnNotInMoodFor.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    Creature creature = (Creature)x;
                    //DateTime notInMoodUntil = creature.NotInMoodUntil;
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
                    //DateTime groomedon = creature.GroomedOn;
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
                DateTime dt = (DateTime) x;
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


            this.context.OnHerdsModified += Context_OnHerdsModified;
            this.context.OnEntitiesModified += ContextOnEntitiesModified;
            this.mainForm.Granger_UserViewChanged += MainForm_UserViewChanged;
            this.mainForm.Granger_AdvisorChanged += MainForm_Granger_AdvisorChanged;
            this.mainForm.Granger_ValuatorChanged += MainForm_Granger_ValuatorChanged;

            UpdateCurrentCreaturesData();
            UpdateDataForView();
            timer1.Enabled = true;
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

        void ContextOnEntitiesModified(object sender, EventArgs e)
        {
            UpdateCurrentCreaturesData();
            UpdateDataForView();
        }

        void MainForm_UserViewChanged(object sender, FormGrangerMain.UserViewChangedEventArgs e)
        {
            if (e.HerdViewVisible) tableLayoutPanel1.RowStyles[0].Height = 0;
            else tableLayoutPanel1.RowStyles[0].Height = 30;
        }

        void Context_OnHerdsModified(object sender, EventArgs e)
        {
            UpdateCurrentCreaturesData();
            UpdateDataForView();
        }

        void UpdateCurrentCreaturesData() //on init and model updates
        {
            activeHerds = context.Herds.AsEnumerable().Where(x => x.Selected == true).OrderBy(x => x.HerdID).ToList();

            textBoxHerds.Text = string.Join(", ", activeHerds.Select(x => x.HerdID));

            CurrentCreatures = context.Creatures
                .AsEnumerable()
                .Where(x => activeHerds
                    .Select(y => y.HerdID) //create a temporary collection of herdID's
                    .Contains(x.Herd)) //select this creature if herd is in temp collection
                .Select(x => new Creature(mainForm, x, context))
                .ToList();
        }

        //is this really useful at all?
        bool _updatingListView = false;
        void UpdateDataForView()
        {
            _updatingListView = true;

            // here goes breeding update!
            //cache breeding eval
            double maxValue = 0;
            double minValue = 0;

            foreach (var creature in CurrentCreatures)
            {
                creature.RebuildCachedBreedValue();
                if (creature.CachedBreedValue.HasValue)
                {
                    if (double.IsInfinity(creature.CachedBreedValue.Value)) continue;

                    if (creature.CachedBreedValue > maxValue) maxValue = creature.CachedBreedValue.Value;
                    if (creature.CachedBreedValue < minValue) minValue = creature.CachedBreedValue.Value;
                }
            }

            foreach (var creature in CurrentCreatures)
            {
                creature.ClearColorHints();
                if (!mainForm.Settings.DisableRowColoring)
                {
                    creature.RefreshBreedHintColor(minValue, maxValue);
                }
            }

            objectListView1.SetObjects(CurrentCreatures, true);
            
            _updatingListView = false;
        }

        private void UCGrangerCreatureList_Load(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListView1.SelectedObjects.Count > 1)
                MessageBox.Show("Select a single creature for editing");
            else
            {
                var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
                foreach (var creature in selected)
                {
                    FormCreatureViewEdit ui = new FormCreatureViewEdit(mainForm,
                        creature,
                        context,
                        CreatureViewEditOpType.Edit,
                        creature.HerdAspect,
                        logger,
                        wurmApi);
                    ui.ShowDialogCenteredOnForm(mainForm);
                }
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
            FormChooseHerd ui = new FormChooseHerd(mainForm, context);
            if (ui.ShowDialogCenteredOnForm(mainForm) == DialogResult.OK)
            {
                string herdId = ui.Result;

                var targetHerd = context.Creatures.Where(x => x.Herd == herdId).Select(x => new Creature(mainForm, x, context)).ToArray();

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
                    MessageBox.Show("could not change herd for selected creatures, because following creatures have same identity (name and server if known):\r\n"
                        + string.Join(", ", nonuniqueCreatures.Select(x => x.ToString())));
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
            if (MessageBox.Show("This will permanently delete following creatures:\r\n" +
                string.Join(",\r\n", (IEnumerable<Creature>)selected) + "\r\nContinue?",
                "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                context.DeleteCreatures(selected.Select(x => x.Entity).ToArray());
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListView1.SelectedObjects.Count > 1)
                MessageBox.Show("Select a single creature for viewing");
            else
            {
                var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
                foreach (var creature in selected)
                {
                    FormCreatureViewEdit ui = new FormCreatureViewEdit(mainForm,
                        creature,
                        context,
                        CreatureViewEditOpType.View,
                        creature.HerdAspect,
                        logger,
                        wurmApi);
                    ui.ShowDialogCenteredOnForm(mainForm);
                }
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

        //set creature color

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.Black));
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.White));
        }

        private void greyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.Grey));
        }

        private void brownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.Brown));
        }

        private void goldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.Gold));
        }

        private void notACreatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.Unknown));
        }

        void UpdateCreaturesColors(CreatureColor color)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();

            if (selected.Length > 2)
            {
                if (System.Windows.Forms.MessageBox.Show("This will set color for following creatures:\r\n" +
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
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();

            foreach (var creature in selected)
            {
                bool tagState = creature.CheckTag(tag);
                creature.SetTag(tag, !tagState);
            }

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
            //this causes crash on Win XP, ordering events happen before mainform assigned,
            //because mainform is assigned after constructor
            if (!_debug_MainFormAssigned && mainForm == null) return;

            var settings = this.objectListView1.SaveState();
            try
            {
                mainForm.Settings.CreatureListState = settings;
            }
            catch (Exception _e)
            {
                logger.Error(_e,
                    string.Format("Something went wrong when trying to save creaturelist layout, mainform null: {0}",
                        mainForm));
            }
        }

        private void setMateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Creature>().ToArray();
            if (selected.Length != 2)
            {
                MessageBox.Show("Select exactly 2 creatures that should be paired together");
                return;
            }

            Creature firstCreatureMate = selected[0].GetMate();
            Creature secondCreatureMate = selected[1].GetMate();
            if (firstCreatureMate != null || secondCreatureMate != null)
            {
                if (MessageBox.Show("At least one of selected creatures already has a mate. Continue to change their mates?",
                    "Confirm", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    if (firstCreatureMate != null) firstCreatureMate.SetMate(null);
                    if (secondCreatureMate != null) secondCreatureMate.SetMate(null);
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
                if (MessageBox.Show("Mates will be cleared for following creatures:\r\n" +
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

        Creature[] lastSelectedCreatures = null;
        private void objectListView1_SelectionChanged(object sender, EventArgs e)
        {
            // disabled: not fired properly under WPF application, replaced by SelectedIndexChanged
            //SelectionChanged();
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged();
        }

        void SelectionChanged()
        {
            var newSelectedCreatures = SelectedCreatures;
            bool changed = SelectionChangedCheck(lastSelectedCreatures, newSelectedCreatures);
            if (!_updatingListView && changed)
            {
                var selectedCreatures = newSelectedCreatures;
                logger.Debug("Selected creatures changed, array count: " + selectedCreatures.Length);
                if (selectedCreatures.Length == 1)
                {
                    logger.Debug("Selected single creature, array count: " + selectedCreatures.Length);
                    var creature = selectedCreatures[0];
                    if (!creature.Equals(SelectedSingleCreature)) //change only if new selected creature is different
                    {
                        logger.Debug("Selected single creature changing");
                        SelectedSingleCreature = selectedCreatures[0];
                    }
                }
                else SelectedSingleCreature = null;
            }
            lastSelectedCreatures = newSelectedCreatures;
        }

        private bool SelectionChangedCheck(Creature[] lastSelectedCreatures, Creature[] newSelectedCreatures)
        {
            if (lastSelectedCreatures == null && newSelectedCreatures == null)
                return false;

            if (lastSelectedCreatures == null && newSelectedCreatures != null ||
                lastSelectedCreatures != null && newSelectedCreatures == null)
                return true;

            if (lastSelectedCreatures.Length != newSelectedCreatures.Length) return true;
            else
            {
                foreach (var creature in lastSelectedCreatures)
                {
                    if (!newSelectedCreatures.Contains(creature)) return true;
                }
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
            // moved to row coloring code
            //if (e.Column == olvColumnName)
            //{
            //    creature creature = (Creature)e.Model;
            //    Color? color = creature.CreatureBestCandidateColor;
            //    if (color != null)
            //    {
            //        e.SubItem.BackColor = color.Value;
            //    }
            //}
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

        //not in mood
        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetNotInMood(new TimeSpan(0));
        }

        //not in mood
        private void set45MinutesFromNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNotInMood(TimeSpan.FromMinutes(45));
        }

        //not in mood
        private void setTo1HourFromNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetNotInMood(TimeSpan.FromHours(1));
        }

        //not in mood
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

        private void objectListView1_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            
        }

        private void objectListView1_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedSingleCreature != null)
            {
                Creature tempCreatureRef = SelectedSingleCreature;
                FormEditComments ui = new FormEditComments(this.mainForm, tempCreatureRef.Comments, tempCreatureRef.Name);
                if (ui.ShowDialogCenteredOnForm(mainForm) == DialogResult.OK)
                {
                    tempCreatureRef.Comments = ui.Result;
                    context.SubmitChanges();
                }
            }
        }

        private void bloodBayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.BloodBay));
        }

        private void ebonyBlackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.EbonyBlack));
        }

        private void piebaldPintoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCreaturesColors(new CreatureColor(CreatureColorId.PiebaldPinto));
        }
    }
}
