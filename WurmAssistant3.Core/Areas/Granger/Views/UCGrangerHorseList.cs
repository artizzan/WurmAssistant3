using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.HorseEdit;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.HorseEdit;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy
{
    public partial class UCGrangerHorseList : UserControl
    {
        FormGrangerMain MainForm;
        GrangerContext Context;

        ILogger logger;

        List<Horse> CurrentHorses = new List<Horse>(); //cached

        readonly DateTime _treshholdDtValueForBirthDate = new DateTime(1990, 1, 1);
        readonly TimeSpan _treshholdTsValueForExactAge = DateTime.Now - new DateTime(1990, 1, 1);
        readonly int _treshholdDaysValueForExactAge = (int)(DateTime.Now - new DateTime(1990, 1, 1)).TotalDays;

        public Horse[] SelectedHorses
        {
            get
            {
                var array = objectListView1.SelectedObjects.Cast<Horse>().ToArray();
                return array;
            }
        }

        List<HerdEntity> ActiveHerds = new List<HerdEntity>(); //cached

        Horse _SelectedSingleHorse = null;

        public Horse SelectedSingleHorse
        {
            get { return _SelectedSingleHorse; }
            set
            {
                _SelectedSingleHorse = value;
                if (MainForm != null) //some designer bug, this prop appear in designer which tries to set it to null initially
                {
                    MainForm.TriggerSelectedSingleHorseChanged();
                    UpdateDataForView();
                }
            }
        }

        bool _debug_MainFormAssigned = false;

        public UCGrangerHorseList()
        {
            InitializeComponent();
        }

        public void Init([NotNull] FormGrangerMain mainForm, [NotNull] GrangerContext context, [NotNull] ILogger logger)
        {
            if (mainForm == null) throw new ArgumentNullException("mainForm");
            if (context == null) throw new ArgumentNullException("context");
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            MainForm = mainForm;
            _debug_MainFormAssigned = true;

            if (MainForm.Settings.AdjustForDarkThemes)
            {
                MakeDarkHighContrastFriendly();
            }

            if (mainForm.Settings.HorseListState != null)
            {
                this.objectListView1.RestoreState(mainForm.Settings.HorseListState);
            }
            Context = context;

            olvColumnPairedWith.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    Horse horse = (Horse)x;
                    Horse mate = horse.GetMate();
                    if (mate != null)
                    {
                        string[] output = new string[2] { horse.ToString(), mate.ToString() };
                        return string.Join(" + ", output.OrderBy(y => y));
                    }
                    else return "Free";
                });

            //olvColumnValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            //    {
            //        Horse horse = (Horse)x;
            //        return horse.ValueAspect;
            //    });
            olvColumnValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            {
                Horse horse = (Horse)x;
                if (horse.Value >= 0) return "Positive";
                else return "Negative";
            });
            olvColumnValue.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                return x.ToString();
            });

            //olvColumnBreedValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
            //    {
            //        Horse horse = (Horse)x;
            //        var result = horse.BreedValueAspect;
            //        return result != null ? result.ToString() : "Not comparing";
            //    });
            olvColumnBreedValue.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    Horse horse = (Horse)x;
                    double? val = horse.BreedValueAspect;
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
                    Horse horse = (Horse)x;
                    var traits = horse.Traits;
                    HorseTrait[] positives = HorseTrait.GetGoodTraits(traits, MainForm.CurrentValuator);
                    HorseTrait[] negatives = HorseTrait.GetBadTraits(traits, MainForm.CurrentValuator);
                    return string.Format("Good: {0}, Neutral: {1}, Bad: {2}",
                        positives.Length,
                        traits.Length - positives.Length - negatives.Length,
                        negatives.Length);
                });

            olvColumnTraitsInspectedAt.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    Horse horse = (Horse)x;
                    var traitinfo = horse.TraitsInspectedAtSkillAspect;
                    if (traitinfo.Skill > HorseTrait.GetFullTraitVisibilityCap(traitinfo.EpicCurve))
                        return "Fully known";
                    else return "Known partially";
                });

            olvColumnAge.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    HorseAge age = ((Horse)x).Age;
                    return (int)age.HorseAgeId;
                });
            olvColumnAge.GroupKeyToTitleConverter = new BrightIdeasSoftware.GroupKeyToTitleConverterDelegate(x =>
                {
                    int result = (int)x;
                    return (new HorseAge(result.ToString()).ToString());
                });

            //////////////
            // GroupKeyGetter needs to be sortable to position them correctly,
            // GroupKeyToTitleConverter converts that key into something more meaningful to display
            // AspectToStringConverter converts aspect into display string in special way

            olvColumnNotInMoodFor.GroupKeyGetter = new BrightIdeasSoftware.GroupKeyGetterDelegate(x =>
                {
                    Horse horse = (Horse)x;
                    //DateTime notInMoodUntil = horse.NotInMoodUntil;
                    TimeSpan tms = horse.NotInMoodForAspect;// notInMoodUntil - DateTime.Now;
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
                    Horse horse = (Horse)x;
                    //DateTime groomedon = horse.GroomedOn;
                    TimeSpan tms = horse.GroomedAgoAspect; // DateTime.Now - groomedon;
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
                        if (ts < MainForm.Settings.ShowGroomingTime)
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
                Horse horse = (Horse)x;
                DateTime pregnantUntil = horse.PregnantUntil;
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
                Horse horse = (Horse)x;
                return horse.BirthDate.Date;
            });
            olvColumnBirthDate.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is DateTime)
                {
                    DateTime dt = (DateTime)x;
                    if (dt < _treshholdDtValueForBirthDate)
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
                if (dt < _treshholdDtValueForBirthDate)
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
                Horse horse = (Horse)x;
                var val = (int)horse.ExactAgeAspect.TotalDays;
                if (val > _treshholdDaysValueForExactAge) return int.MaxValue;
                else return val;
            });
            olvColumnExactAge.AspectToStringConverter = new BrightIdeasSoftware.AspectToStringConverterDelegate(x =>
            {
                if (x is TimeSpan)
                {
                    TimeSpan ts = (TimeSpan)x;
                    if (ts > _treshholdTsValueForExactAge)
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
                if (gkey > _treshholdDaysValueForExactAge)
                {
                    return "No birth date available";
                }
                else
                {
                    return string.Format("{0} real days", gkey);
                }
            });


            Context.OnHerdsModified += Context_OnHerdsModified;
            Context.OnHorsesModified += Context_OnHorsesModified;
            MainForm.Granger_UserViewChanged += MainForm_UserViewChanged;
            MainForm.Granger_AdvisorChanged += MainForm_Granger_AdvisorChanged;
            MainForm.Granger_ValuatorChanged += MainForm_Granger_ValuatorChanged;

            UpdateCurrentHorsesData();
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

        void Context_OnHorsesModified(object sender, EventArgs e)
        {
            UpdateCurrentHorsesData();
            UpdateDataForView();
        }

        void MainForm_UserViewChanged(object sender, FormGrangerMain.UserViewChangedEventArgs e)
        {
            if (e.HerdViewVisible) tableLayoutPanel1.RowStyles[0].Height = 0;
            else tableLayoutPanel1.RowStyles[0].Height = 30;
        }

        void Context_OnHerdsModified(object sender, EventArgs e)
        {
            UpdateCurrentHorsesData();
            UpdateDataForView();
        }

        void UpdateCurrentHorsesData() //on init and model updates
        {
            ActiveHerds = Context.Herds.AsEnumerable().Where(x => x.Selected == true).OrderBy(x => x.HerdID).ToList();

            textBoxHerds.Text = string.Join(", ", ActiveHerds.Select(x => x.HerdID));

            CurrentHorses = Context.Horses
                .AsEnumerable()
                .Where(x => ActiveHerds
                    .Select(y => y.HerdID) //create a temporary collection of herdID's
                    .Contains(x.Herd)) //select this horse if herd is in temp collection
                .Select(x => new Horse(MainForm, x, Context))
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

            foreach (var horse in CurrentHorses)
            {
                horse.RebuildCachedBreedValue();
                if (horse.CachedBreedValue.HasValue)
                {
                    if (double.IsInfinity(horse.CachedBreedValue.Value)) continue;

                    if (horse.CachedBreedValue > maxValue) maxValue = horse.CachedBreedValue.Value;
                    if (horse.CachedBreedValue < minValue) minValue = horse.CachedBreedValue.Value;
                }
            }

            foreach (var horse in CurrentHorses)
            {
                horse.ClearColorHints();
                if (!MainForm.Settings.DisableRowColoring)
                {
                    horse.RefreshBreedHintColor(minValue, maxValue);
                }
            }

            objectListView1.SetObjects(CurrentHorses, true);
            
            _updatingListView = false;
        }

        private void UCGrangerHorseList_Load(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListView1.SelectedObjects.Count > 1)
                MessageBox.Show("Select a single creature for editing");
            else
            {
                var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();
                foreach (var horse in selected)
                {
                    FormHorseViewEdit ui = new FormHorseViewEdit(MainForm,
                        horse,
                        Context,
                        HorseViewEditOpType.Edit,
                        horse.HerdAspect,
                        logger);
                    ui.ShowDialogCenteredOnForm(MainForm);
                }
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();
            FormChooseHerd ui = new FormChooseHerd(MainForm, Context);
            if (ui.ShowDialogCenteredOnForm(MainForm) == DialogResult.OK)
            {
                string herdID = ui.Result;

                var targetHerd = Context.Horses.Where(x => x.Herd == herdID).Select(x => new Horse(MainForm, x, Context));

                List<Horse> nonuniqueHorses = new List<Horse>();
                foreach (var horse in selected)
                {
                    foreach (var otherhorse in targetHerd)
                    {
                        if (horse != otherhorse)
                        {
                            if (horse.IsIdenticalIdentity(otherhorse))
                            {
                                nonuniqueHorses.Add(horse);
                            }
                        }
                    }
                }

                if (nonuniqueHorses.Count > 0)
                    MessageBox.Show("could not change herd for selected creatures, because following creatures have same identity (name+gender):\r\n"
                        + string.Join(", ", nonuniqueHorses.Select(x => x.ToString())));
                else
                {
                    foreach (var horse in selected)
                    {
                        horse.Herd = herdID;
                    }
                    Context.SubmitChangesToHorses();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Horse>().OrderBy(x => x.NameAspect).ToArray();
            if (MessageBox.Show("This will permanently delete following creatures:\r\n" +
                string.Join(",\r\n", (IEnumerable<Horse>)selected) + "\r\nContinue?",
                "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                Context.DeleteHorses(selected.Select(x => x.Entity).ToArray());
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (objectListView1.SelectedObjects.Count > 1)
                MessageBox.Show("Select a single creature for viewing");
            else
            {
                var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();
                foreach (var horse in selected)
                {
                    FormHorseViewEdit ui = new FormHorseViewEdit(MainForm,
                        horse,
                        Context,
                        HorseViewEditOpType.View,
                        horse.HerdAspect,
                        logger);
                    ui.ShowDialogCenteredOnForm(MainForm);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible) UpdateDataForView();
        }

        private void UCGrangerHorseList_VisibleChanged(object sender, EventArgs e)
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
            string result = string.Join(", ", objectListView1.SelectedObjects.Cast<Horse>());
            MessageBox.Show(result);
        }

        //set horse color

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHorsesColors(new HorseColor(HorseColorId.Black));
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHorsesColors(new HorseColor(HorseColorId.White));
        }

        private void greyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHorsesColors(new HorseColor(HorseColorId.Grey));
        }

        private void brownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHorsesColors(new HorseColor(HorseColorId.Brown));
        }

        private void goldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHorsesColors(new HorseColor(HorseColorId.Gold));
        }

        private void notAHorseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHorsesColors(new HorseColor(HorseColorId.Unknown));
        }

        void UpdateHorsesColors(HorseColor color)
        {
            var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();

            if (selected.Length > 2)
            {
                if (System.Windows.Forms.MessageBox.Show("This will set color for following creatures:\r\n" +
                    string.Join(", ", (IEnumerable<Horse>)selected) + "\r\nContinue?",
                    "Confirm", System.Windows.Forms.MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
            }

            foreach (var horse in selected)
            {
                horse.Color = color;
            }
            Context.SubmitChangesToHorses();
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
            var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();

            foreach (var horse in selected)
            {
                bool tagState = horse.CheckTag(tag);
                horse.SetTag(tag, !tagState);
            }

            Context.SubmitChangesToHorses();
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
            if (!_debug_MainFormAssigned && MainForm == null) return;

            var settings = this.objectListView1.SaveState();
            try
            {
                MainForm.Settings.HorseListState = settings;
            }
            catch (Exception _e)
            {
                logger.Error(_e,
                    string.Format("Something went wrong when trying to save creaturelist layout, mainform null: {0}",
                        MainForm));
            }
        }

        private void setMateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();
            if (selected.Length != 2)
            {
                MessageBox.Show("Select exactly 2 creatures that should be paired together");
                return;
            }

            Horse firstHorseMate = selected[0].GetMate();
            Horse secondHorseMate = selected[1].GetMate();
            if (firstHorseMate != null || secondHorseMate != null)
            {
                if (MessageBox.Show("At least one of selected creatures already has a mate. Continue to change their mates?",
                    "Confirm", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    if (firstHorseMate != null) firstHorseMate.SetMate(null);
                    if (secondHorseMate != null) secondHorseMate.SetMate(null);
                }
            }

            selected[0].SetMate(selected[1]);
            selected[1].SetMate(selected[0]);
            Context.SubmitChangesToHorses();
        }

        private void clearMateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();

            if (selected.Length > 0)
            {
                if (MessageBox.Show("Mates will be cleared for following creatures:\r\n" +
                    string.Join(",\r\n", (IEnumerable<Horse>)selected) + "\r\nContinue?",
                    "Confirm", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }

                foreach (var horse in selected)
                {
                    horse.SetMate(null);
                }

                Context.SubmitChangesToHorses();
            }
        }

        Horse[] lastSelectedHorses = null;
        private void objectListView1_SelectionChanged(object sender, EventArgs e)
        {
            var newSelectedHorses = SelectedHorses;
            bool changed = SelectionChangedCheck(lastSelectedHorses, newSelectedHorses);
            if (!_updatingListView && changed)
            {
                var selhorses = newSelectedHorses;
                logger.Debug("Selected creatures changed, array count: " + selhorses.Length);
                if (selhorses.Length == 1)
                {
                    logger.Debug("Selected single creature, array count: " + selhorses.Length);
                    var horse = selhorses[0];
                    if (!horse.Equals(SelectedSingleHorse)) //change only if new selected horse is different
                    {
                        logger.Debug("Selected single creature changing");
                        SelectedSingleHorse = selhorses[0];
                    }
                }
                else SelectedSingleHorse = null;
            }
            lastSelectedHorses = newSelectedHorses;
        }

        private bool SelectionChangedCheck(Horse[] lastSelectedHorses, Horse[] newSelectedHorses)
        {
            if (lastSelectedHorses == null && newSelectedHorses == null)
                return false;

            if (lastSelectedHorses == null && newSelectedHorses != null ||
                lastSelectedHorses != null && newSelectedHorses == null)
                return true;

            if (lastSelectedHorses.Length != newSelectedHorses.Length) return true;
            else
            {
                foreach (var horse in lastSelectedHorses)
                {
                    if (!newSelectedHorses.Contains(horse)) return true;
                }
            }

            return false;
        }

        private void objectListView1_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            Horse horse = (Horse)e.Model;
            System.Drawing.Color? horseColorId = horse.HorseBestCandidateColor;
            if (horseColorId != null)
            {
                e.Item.BackColor = horseColorId.Value;
            }
            else
            {
                horseColorId = horse.BreedHintColor;
                if (horseColorId != null) e.Item.BackColor = horseColorId.Value;
            }
        }

        private void objectListView1_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            // moved to row coloring code
            //if (e.Column == olvColumnName)
            //{
            //    Horse horse = (Horse)e.Model;
            //    Color? color = horse.HorseBestCandidateColor;
            //    if (color != null)
            //    {
            //        e.SubItem.BackColor = color.Value;
            //    }
            //}
            if (e.Column == olvColumnColor)
            {
                Horse horse = (Horse)e.Model;
                System.Drawing.Color? color = horse.HorseColorBkColor;
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
            var selected = objectListView1.SelectedObjects.Cast<Horse>().ToArray();

            if (selected.Length > 0)
            {
                foreach (var horse in selected)
                {
                    horse.NotInMoodUntil = DateTime.Now + duration;
                }

                Context.SubmitChangesToHorses();
            }
        }

        private void objectListView1_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            
        }

        private void objectListView1_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedSingleHorse != null)
            {
                Horse tempHorseRef = SelectedSingleHorse;
                FormEditComments ui = new FormEditComments(this.MainForm, tempHorseRef.Comments, tempHorseRef.Name);
                if (ui.ShowDialogCenteredOnForm(MainForm) == DialogResult.OK)
                {
                    tempHorseRef.Comments = ui.Result;
                    Context.SubmitChangesToHorses();
                }
            }
        }
    }
}
