using System;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    public class CooldownHandler
    {
        readonly NotifyHandler handler;
        DateTime cooldownTo = DateTime.MinValue;
        public DateTime CooldownTo
        {
            get { return cooldownTo; }
            set
            {
                if (value > DateTime.Now)
                {
                    shown = played = false;
                    cooldownTo = value;
                }
            }
        }

        public bool SoundEnabled { get; set; }
        public bool PopupEnabled { get; set; }

        bool shown = true;
        bool played = true;

        public CooldownHandler([NotNull] ILogger logger, [NotNull] ISoundManager soundManager,
            [NotNull] ITrayPopups trayPopups,
            Guid? soundId = null, string messageTitle = null, string messageContent = null, bool messagePersist = false)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            handler = new NotifyHandler(logger, soundManager, trayPopups)
            {
                SoundId = (soundId ?? Guid.Empty),
                Title = (messageTitle ?? string.Empty),
                Message = (messageContent ?? string.Empty)
            };
            if (messagePersist) handler.PopupPersistent = true;
        }

        public void ResetShownAndPlayed()
        {
            shown = played = false;
        }

        public Guid SoundId
        {
            get { return handler.SoundId; }
            set { handler.SoundId = value; }
        }

        public string Title
        {
            get { return handler.Title; }
            set { handler.Title = (value ?? ""); }
        }

        public string Message
        {
            get { return handler.Message; }
            set { handler.Message = (value ?? ""); }
        }

        public bool PersistentPopup
        {
            get { return handler.PopupPersistent; }
            set { handler.PopupPersistent = value; }
        }

        public int DurationMillis
        {
            get { return handler.Duration; }
            set { handler.Duration = value; }
        }

        public void Update()
        {
            if (DateTime.Now > CooldownTo)
            {
                if (SoundEnabled)
                {
                    if (!played)
                    {
                        handler.Play();
                        played = true;
                    }
                }
                if (PopupEnabled)
                {
                    if (!shown)
                    {
                        handler.Show();
                        shown = true;
                    }
                }
            }
            handler.Update();
        }
    }
}
