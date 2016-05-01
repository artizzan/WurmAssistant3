using System;

namespace WurmAssistantDataTransfer.Dtos
{
    public class Timer
    {
        public Guid? Id { get; set; }
        public Guid? DefinitionId { get; set; }

        /// <summary>
        /// For legacy (WA2) custom timer support only.
        /// If this is not null, DefinitionId is null.
        /// WA3 timers are always identified by GUIDs.
        /// </summary>
        public string LegacyDefinitionName { get; set; }

        public string RuntimeTypeIdEnum { get; set; }

        public string Name { get; set; }
        public bool SoundNotify { get; set; }
        public bool PopupNotify { get; set; }
        public bool PersistentPopup { get; set; }
        public bool PopupOnWaLaunch { get; set; }
        public int PopupDurationMillis { get; set; }

        public string CharacterName { get; set; }

        public Sound Sound { get; set; }

        public string ServerGroupId { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, CharacterName: {2}, ServerGroupId: {3}",
                Id,
                Name,
                CharacterName,
                ServerGroupId);
        }
    }

    public class MeditationTimer : Timer
    {
        public bool SleepBonusReminder { get; set; }
        public int SleepBonusPopupDurationMillis { get; set; }
        public bool ShowMeditSkill { get; set; }
        public bool ShowMeditCount { get; set; }
    }

    public class AlignmentTimer : Timer
    {
        public bool IsWhiteLighter { get; set; }
        public string WurmReligion { get; set; }
    }

    public class PrayerTimer : Timer
    {
        public bool FavorNotifySoundEnabled { get; set; }
        public bool FavorNotifyPopupEnabled { get; set; }
        public bool FavorNotifyWhenMax { get; set; }
        public float FavorNotifyOnLevel { get; set; }
        public bool FavorNotifyPopupPersist { get; set; }

        public Sound FavorNotifySound { get; set; }
    }

    public class CustomTimer : Timer { }

    public class JunkSaleTimer : Timer { }

    public class MeditPathTimer : Timer { }

    public class SermonTimer : Timer { }
}