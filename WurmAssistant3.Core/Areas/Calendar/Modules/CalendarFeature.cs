using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.PersistentObjects;
using AldursLab.WurmApi;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Calendar.Views;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Core.Properties;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Ninject;
using ILogger = AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts.ILogger;

namespace AldursLab.WurmAssistant3.Core.Areas.Calendar.Modules
{
    [PersistentObject("CalendarFeature")]
    public sealed class CalendarFeature : PersistentObjectBase, IFeature
    {
        readonly IWurmApi wurmApi;
        readonly ILogger logger;
        readonly IUpdateLoop updateLoop;
        readonly ISoundEngine soundEngine;
        readonly ITrayPopups trayPopups;

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

        [JsonObject(MemberSerialization.OptIn)]
        public class CalendarSettings
        {
            public CalendarFeature CalendarFeature { get; set; }

            public CalendarSettings()
            {
                useWurmTimeForDisplay = false;
                soundWarning = false;
                soundId = Guid.Empty;
                popupWarning = false;
                trackedSeasons = new string[0];
                serverName = "";
                mainWindowSize = new System.Drawing.Size(487, 414);
            }

            [JsonProperty] bool useWurmTimeForDisplay;
            [JsonProperty] bool soundWarning;
            [JsonProperty] Guid soundId;
            [JsonProperty] bool popupWarning;
            [JsonProperty(IsReference = false)] string[] trackedSeasons;
            [JsonProperty] string serverName;
            [JsonProperty] System.Drawing.Size mainWindowSize;

            public bool UseWurmTimeForDisplay
            {
                get { return useWurmTimeForDisplay; }
                set
                {
                    useWurmTimeForDisplay = value;
                    CalendarFeature.FlagAsChanged();
                }
            }

            public bool SoundWarning
            {
                get { return soundWarning; }
                set
                {
                    soundWarning = value;
                    CalendarFeature.FlagAsChanged();
                }
            }

            public Guid SoundId
            {
                get { return soundId; }
                set
                {
                    soundId = value;
                    CalendarFeature.FlagAsChanged();
                }
            }

            public ISoundResource Sound
            {
                get { return CalendarFeature.soundEngine.GetSoundById(soundId); }
            }

            public bool PopupWarning
            {
                get { return popupWarning; }
                set
                {
                    popupWarning = value;
                    CalendarFeature.FlagAsChanged();
                }
            }

            public string[] TrackedSeasons
            {
                get { return trackedSeasons; }
                set
                {
                    trackedSeasons = value;
                    CalendarFeature.FlagAsChanged();
                }
            }

            public string ServerName
            {
                get { return serverName; }
                set
                {
                    serverName = value;
                    CalendarFeature.FlagAsChanged();
                    CalendarFeature.ObtainWdtForCurrentServer();
                }
            }

            public Size MainWindowSize
            {
                get { return mainWindowSize; }
                set
                {
                    mainWindowSize = value;
                    CalendarFeature.FlagAsChanged();
                }
            }
        }

        [JsonProperty]
        readonly CalendarSettings settings;

        readonly WurmSeasonsManager seasonsManager;

        public CalendarFeature([NotNull] IWurmApi wurmApi, [NotNull] ILogger logger, [NotNull] IUpdateLoop updateLoop,
            [NotNull] ISoundEngine soundEngine, [NotNull] ITrayPopups trayPopups,
            [NotNull] WurmSeasonsManager seasonsManager)
        {
            if (wurmApi == null) throw new ArgumentNullException("wurmApi");
            if (logger == null) throw new ArgumentNullException("logger");
            if (updateLoop == null) throw new ArgumentNullException("updateLoop");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            if (seasonsManager == null) throw new ArgumentNullException("seasonsManager");
            this.wurmApi = wurmApi;
            this.logger = logger;
            this.updateLoop = updateLoop;
            this.soundEngine = soundEngine;
            this.trayPopups = trayPopups;
            this.seasonsManager = seasonsManager;
            settings = new CalendarSettings();

            updateLoop.Updated += (sender, args) =>
            {
                ObtainWdtForCurrentServer();
                if (hasWdt) UpdateOutputList();
            };
            seasonsManager.DataChanged += SeasonsManagerOnDataChanged;
        }

        void SeasonsManagerOnDataChanged(object sender, EventArgs eventArgs)
        {
            ReInitSeasonData();
        }

        protected override void OnPersistentDataLoaded()
        {
            Settings.CalendarFeature = this;

            CalendarUI = new FormCalendar(this, wurmApi, logger, soundEngine);
            CalendarUI.UpdateTrackedSeasonsList(settings.TrackedSeasons);

            ReInitSeasonData();

            ObtainWdtForCurrentServer();
        }

        List<WurmSeasonOutputItem> WurmSeasonOutput = new List<WurmSeasonOutputItem>();
        FormCalendar CalendarUI;

        internal WurmDateTime cachedWDT;

        #region IFeature

        void IFeature.Show()
        {
            CalendarUI.ShowAndBringToFront();
        }

        void IFeature.Hide()
        {
            CalendarUI.Hide();
        }

        string IFeature.Name { get { return "Calendar"; } }

        Image IFeature.Icon { get { return Resources.CalendarIcon; } }

        async Task IFeature.InitAsync()
        {
            // no initialization required
            await Task.FromResult(true);
        }

        #endregion

        public CalendarSettings Settings
        {
            get { return settings; }
        }

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
                    if (!String.IsNullOrEmpty(settings.ServerName))
                    {
                        var server = wurmApi.Servers.GetByName(new ServerName(settings.ServerName));
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
                logger.Error(exception, "Error for server name: " + (settings.ServerName ?? "NULL"));
            }
            finally
            {
                obtainingWdt = false;
            }
        }

        List<KeyValuePair<string, DateTime>> PopupQueue = new List<KeyValuePair<string, DateTime>>();
        bool popupScheduled = false;

        public void UpdateOutputList()
        {
            if (settings.PopupWarning || settings.SoundWarning || CalendarUI.Visible)
            {
                foreach (WurmSeasonOutputItem item in WurmSeasonOutput)
                {
                    item.Update(cachedWDT);
                    if (item.ShouldNotifyUser())
                    {
                        if (item.IsItemTracked(settings.TrackedSeasons))
                        {
                            if (settings.SoundWarning)
                            {
                                TriggerSoundWarning();
                                item.UserNotified();
                            }
                            if (settings.PopupWarning)
                            {
                                PopupQueue.Add(new KeyValuePair<string, DateTime>(item.GetSeasonName(), item.GetSeasonEndDate()));
                                popupScheduled = true;
                                item.UserNotified();
                            }
                        }
                    }
                }
                if (CalendarUI.Visible)
                {
                    WurmSeasonOutput.Sort();
                    CalendarUI.UpdateSeasonOutput(WurmSeasonOutput, settings.UseWurmTimeForDisplay);
                }
                if (popupScheduled)
                {
                    string output = "";
                    foreach (var item in PopupQueue)
                    {
                        //var endsAt = item.Value.ToString("dd-MM-yyyy hh:mm");
                        TimeSpan endsIn = item.Value - DateTime.Now;
                        output += item.Key + " is now in season. Season ends in " + endsIn.ToStringCompact() + "\n";
                    }
                    TriggerPopupWarning(output);
                    popupScheduled = false;
                    PopupQueue.Clear();
                }
            }
        }

        public void ChooseTrackedSeasons()
        {
            var trackedSeasons = settings.TrackedSeasons;

            FormChooseSeasons seasonsDialog = new FormChooseSeasons(
                seasonsManager.AllNotDisabled.Select(x => x.SeasonName).Distinct().ToArray(), trackedSeasons);
            seasonsDialog.ShowDialog();
            var newTrackedSeasons = new List<string>();
            foreach (var item in seasonsDialog.checkedListBox1.CheckedItems)
            {
                newTrackedSeasons.Add(item.ToString());
            }
            settings.TrackedSeasons = newTrackedSeasons.ToArray();

            CalendarUI.UpdateTrackedSeasonsList(settings.TrackedSeasons);
        }

        void TriggerSoundWarning()
        {
            soundEngine.PlayOneShot(settings.SoundId);
        }

        void TriggerPopupWarning(string text)
        {
            trayPopups.Schedule("Wurm Season Notify", text);
        }

        public void ModifySeasons()
        {
            var view = new SeasonsEditView(seasonsManager);
            view.ShowDialog();
        }
    }
}
