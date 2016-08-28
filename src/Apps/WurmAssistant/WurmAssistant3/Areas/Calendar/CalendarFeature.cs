using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Features;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using AldursLab.WurmAssistant3.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ILogger = AldursLab.WurmAssistant3.Areas.Logging.ILogger;

namespace AldursLab.WurmAssistant3.Areas.Calendar
{
    [KernelBind(BindingHint.Singleton), PersistentObject("CalendarFeature")]
    public sealed class CalendarFeature : PersistentObjectBase, IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly ISoundManager soundManager;
        readonly ITrayPopups trayPopups;
        readonly ITimer updateTimer;

        public class WurmSeasonOutputItem : IComparable<WurmSeasonOutputItem>
        {
            WurmSeasonDefinition seasonDefinition;
            int LengthDays;
            bool inSeason = false;
            bool lastInSeasonState = false;
            public bool notifyUser = false;
            TimeSpan RealTimeToSeason;
            TimeSpan WurmTimeToSeason;
            TimeSpan RealTimeToSeasonEnd;
            TimeSpan WurmTimeToSeasonEnd;
            double CompareValue;
            double CompareOffset;

            public WurmSeasonOutputItem(WurmSeasonDefinition wurmSeasonDefinition, double compareOffset, WurmDateTime currentWDT)
            {
                seasonDefinition = wurmSeasonDefinition;
                LengthDays = seasonDefinition.DayEnd - seasonDefinition.DayBegin + 1;
                CompareOffset = compareOffset;
                Update(currentWDT);
            }

            public void Update(WurmDateTime currentWDT)
            {
                lastInSeasonState = inSeason;
                if (currentWDT.DayInYear >= seasonDefinition.DayBegin
                    && currentWDT.DayInYear <= seasonDefinition.DayEnd)
                {
                    inSeason = true;
                }
                else inSeason = false;

                if (inSeason)
                {
                    WurmTimeToSeasonEnd = GetTimeToSeasonEnd(seasonDefinition.DayEnd + 1, currentWDT);
                    RealTimeToSeasonEnd = new TimeSpan(WurmTimeToSeasonEnd.Ticks / 8);
                    CompareValue = (TimeSpan.FromDays(-WurmCalendar.DaysInYear) + WurmTimeToSeasonEnd).TotalSeconds + CompareOffset;
                }
                else
                {
                    WurmTimeToSeason = GetTimeToSeason(seasonDefinition.DayBegin, currentWDT);
                    RealTimeToSeason = new TimeSpan(WurmTimeToSeason.Ticks / 8);
                    CompareValue = WurmTimeToSeason.TotalSeconds + CompareOffset;
                }

                if (inSeason == true && lastInSeasonState == false)
                    notifyUser = true;
            }

            TimeSpan GetTimeToSeason(int dayBegin, WurmDateTime currentWDT)
            {
                return GetTimeToDay(dayBegin, currentWDT);
            }

            TimeSpan GetTimeToSeasonEnd(int dayEnd, WurmDateTime currentWDT)
            {
                return GetTimeToDay(dayEnd, currentWDT);
            }

            TimeSpan GetTimeToDay(int day, WurmDateTime currentWDT)
            {
                TimeSpan Value = TimeSpan.FromDays(day);
                if (Value < currentWDT.DayAndTimeOfYear)
                {
                    return TimeSpan.FromDays(Value.Days + 336) - currentWDT.DayAndTimeOfYear;
                }
                return Value - currentWDT.DayAndTimeOfYear;
            }

            public string BuildName()
            {
                return seasonDefinition.SeasonName;
            }

            public string BuildTimeData(bool wurmTime)
            {
                string value;
                if (inSeason)
                {
                    value = "IN SEASON!";
                }
                else
                {
                    if (wurmTime) value = ParseTimeSpanToNiceStringDMS(WurmTimeToSeason);
                    else value = ParseTimeSpanToNiceStringDMS(RealTimeToSeason);
                }
                return value;
            }

            public string BuildLengthData(bool wurmTime)
            {
                string value;
                if (inSeason)
                {
                    if (wurmTime) value = ParseTimeSpanToNiceStringDMS(WurmTimeToSeasonEnd) + "more";
                    else value = ParseTimeSpanToNiceStringDMS(RealTimeToSeasonEnd) + "more";
                }
                else
                {
                    if (wurmTime) value = String.Format("{0} days", LengthDays.ToString());
                    else
                    {
                        TimeSpan ts = TimeSpan.FromDays((double)LengthDays / 8D);
                        value = ParseTimeSpanToNiceStringDMS(ts);
                    }
                }
                return value;
            }

            string ParseTimeSpanToNiceStringDMS(TimeSpan ts, bool noMinutes = false)
            {
                string value = "";
                if (ts.Days > 0)
                {
                    if (ts.Days == 1) value += String.Format("{0} day ", ts.Days);
                    else value += String.Format("{0} days ", ts.Days);
                }
                if (ts.Hours > 0 || noMinutes)
                {
                    if (ts.Hours == 1) value += String.Format("{0} hour ", ts.Hours);
                    else value += String.Format("{0} hours ", ts.Hours);
                }
                if (!noMinutes)
                {
                    if (ts.Minutes == 1) value += String.Format("{0} minute ", ts.Minutes);
                    else value += String.Format("{0} minutes ", ts.Minutes);
                }
                return value;
            }

            public int CompareTo(WurmSeasonOutputItem dtlm)
            {
                return this.CompareValue.CompareTo(dtlm.CompareValue);
            }

            public bool ShouldNotifyUser()
            {
                return notifyUser;
            }

            public bool IsItemTracked(string[] trackedSeasons)
            {
                return trackedSeasons.Contains(seasonDefinition.SeasonName, StringComparer.InvariantCultureIgnoreCase);
            }

            public string GetSeasonName()
            {
                return seasonDefinition.SeasonName;
            }

            public DateTime GetSeasonEndDate()
            {
                return DateTime.Now + RealTimeToSeasonEnd;
            }

            public void ResetInSeasonFlag()
            {
                lastInSeasonState = false;
                inSeason = false;
            }

            public void UserNotified()
            {
                notifyUser = false;
            }

            public bool IsItemInSeason()
            {
                return inSeason;
            }
        }

        [JsonProperty]
        bool useWurmTimeForDisplay;
        [JsonProperty]
        bool soundWarning;
        [JsonProperty]
        Guid soundId;
        [JsonProperty]
        bool popupWarning;
        [JsonProperty(IsReference = false)]
        string[] trackedSeasons;
        [JsonProperty]
        string serverName;
        [JsonProperty]
        Size mainWindowSize;

        readonly WurmSeasonsManager seasonsManager;

        public CalendarFeature(
            [NotNull] IWurmApi wurmApi, 
            [NotNull] ILogger logger,
            [NotNull] ITimerFactory timerFactory,
            [NotNull] ISoundManager soundManager, 
            [NotNull] ITrayPopups trayPopups,
            [NotNull] WurmSeasonsManager seasonsManager)
        {
            if (wurmApi == null) throw new ArgumentNullException(nameof(wurmApi));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));
            if (soundManager == null) throw new ArgumentNullException(nameof(soundManager));
            if (trayPopups == null) throw new ArgumentNullException(nameof(trayPopups));
            if (seasonsManager == null) throw new ArgumentNullException(nameof(seasonsManager));
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            this.seasonsManager = seasonsManager;

            useWurmTimeForDisplay = false;
            soundWarning = false;
            soundId = Guid.Empty;
            popupWarning = false;
            trackedSeasons = new string[0];
            serverName = "";
            mainWindowSize = new System.Drawing.Size(487, 414);

            updateTimer = timerFactory.CreateUiThreadTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            updateTimer.Tick += (sender, args) =>
            {
                ObtainWdtForCurrentServer();
                if (hasWdt) UpdateOutputList();
            };
            seasonsManager.DataChanged += SeasonsManagerOnDataChanged;
        }

        public bool UseWurmTimeForDisplay
        {
            get { return useWurmTimeForDisplay; }
            set
            {
                useWurmTimeForDisplay = value;
                FlagAsChanged();
            }
        }

        public bool SoundWarning
        {
            get { return soundWarning; }
            set
            {
                soundWarning = value;
                FlagAsChanged();
            }
        }

        public Guid SoundId
        {
            get { return soundId; }
            set
            {
                soundId = value;
                FlagAsChanged();
            }
        }

        public ISoundResource Sound
        {
            get { return soundManager.GetSoundById(soundId); }
        }

        public bool PopupWarning
        {
            get { return popupWarning; }
            set
            {
                popupWarning = value;
                FlagAsChanged();
            }
        }

        public string[] TrackedSeasons
        {
            get { return trackedSeasons; }
            set
            {
                trackedSeasons = value;
                FlagAsChanged();
            }
        }

        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                FlagAsChanged();
                ObtainWdtForCurrentServer();
            }
        }

        public Size MainWindowSize
        {
            get { return mainWindowSize; }
            set
            {
                mainWindowSize = value;
                FlagAsChanged();
            }
        }

        void SeasonsManagerOnDataChanged(object sender, EventArgs eventArgs)
        {
            ReInitSeasonData();
        }

        protected override void OnPersistentDataLoaded()
        {
            ui = new CalendarForm(this, wurmApi, logger, soundManager);
            ui.UpdateTrackedSeasonsList(TrackedSeasons);

            ReInitSeasonData();

            ObtainWdtForCurrentServer();

            updateTimer.Start();
        }

        List<WurmSeasonOutputItem> WurmSeasonOutput = new List<WurmSeasonOutputItem>();
        CalendarForm ui;

        internal WurmDateTime cachedWDT;

        #region IFeature

        void IFeature.Show()
        {
            ui.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
            ui.Hide();
        }

        string IFeature.Name { get { return "Calendar"; } }

        Image IFeature.Icon { get { return Resources.CalendarIcon; } }

        async Task IFeature.InitAsync()
        {
            // no initialization required
            await Task.FromResult(true);
        }

        public int DataImportOrder { get { return 0; } }

        #endregion

        internal void ReInitSeasonData()
        {
            WurmSeasonOutput.Clear();

            double compareOffset = 0D;
            foreach (WurmSeasonDefinition seasondata in seasonsManager.AllNotDisabled)
            {
                WurmSeasonOutput.Add(new WurmSeasonOutputItem(seasondata, compareOffset, cachedWDT));
                compareOffset += 0.1D;
            }
        }

        bool obtainingWdt = false;
        bool hasWdt = false;
        internal async void ObtainWdtForCurrentServer()
        {
            if (obtainingWdt) return;
            obtainingWdt = true;
            // try to get server datetime, if server configured
            try
            {
                while (true)
                {
                    if (!String.IsNullOrEmpty(ServerName))
                    {
                        var server = wurmApi.Servers.GetByName(new ServerName(ServerName));
                        var result = await server.TryGetCurrentTimeAsync();
                        if (result != null)
                        {
                            cachedWDT = result.Value;
                            hasWdt = true;
                            break;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Error for server name: " + (ServerName ?? "NULL"));
            }
            finally
            {
                obtainingWdt = false;
            }
        }

        readonly List<KeyValuePair<string, DateTime>> popupQueue = new List<KeyValuePair<string, DateTime>>();
        bool popupScheduled = false;

        public void UpdateOutputList()
        {
            if (PopupWarning || SoundWarning || ui.Visible)
            {
                foreach (WurmSeasonOutputItem item in WurmSeasonOutput)
                {
                    item.Update(cachedWDT);
                    if (item.ShouldNotifyUser())
                    {
                        if (item.IsItemTracked(TrackedSeasons))
                        {
                            if (SoundWarning)
                            {
                                TriggerSoundWarning();
                                item.UserNotified();
                            }
                            if (PopupWarning)
                            {
                                popupQueue.Add(new KeyValuePair<string, DateTime>(item.GetSeasonName(), item.GetSeasonEndDate()));
                                popupScheduled = true;
                                item.UserNotified();
                            }
                        }
                    }
                }
                if (ui.Visible)
                {
                    WurmSeasonOutput.Sort();
                    ui.UpdateSeasonOutput(WurmSeasonOutput, UseWurmTimeForDisplay);
                }
                if (popupScheduled)
                {
                    string output = "";
                    foreach (var item in popupQueue)
                    {
                        //var endsAt = item.Value.ToString("dd-MM-yyyy hh:mm");
                        TimeSpan endsIn = item.Value - DateTime.Now;
                        output += item.Key + " is now in season. Season ends in " + endsIn.ToStringCompact() + "\n";
                    }
                    TriggerPopupWarning(output);
                    popupScheduled = false;
                    popupQueue.Clear();
                }
            }
        }

        public void ChooseTrackedSeasons()
        {
            var seasons = TrackedSeasons;

            ChooseSeasonsForm seasonsDialog = new ChooseSeasonsForm(
                seasonsManager.AllNotDisabled.Select(x => x.SeasonName).Distinct().ToArray(), seasons);
            seasonsDialog.ShowDialog();
            var newTrackedSeasons = new List<string>();
            foreach (var item in seasonsDialog.checkedListBox1.CheckedItems)
            {
                newTrackedSeasons.Add(item.ToString());
            }
            TrackedSeasons = newTrackedSeasons.ToArray();

            ui.UpdateTrackedSeasonsList(TrackedSeasons);
        }

        void TriggerSoundWarning()
        {
            soundManager.PlayOneShot(SoundId);
        }

        void TriggerPopupWarning(string text)
        {
            trayPopups.Schedule(text, "Wurm Season Notify");
        }

        public void ModifySeasons()
        {
            var view = new SeasonsEditForm(seasonsManager);
            view.ShowDialog();
        }
    }
}
