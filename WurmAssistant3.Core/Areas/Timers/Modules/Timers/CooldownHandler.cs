using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers
{
    public class CooldownHandler
    {
        NotifyHandler Handler;
        DateTime _cooldownTo = DateTime.MinValue;
        public DateTime CooldownTo
        {
            get { return _cooldownTo; }
            set
            {
                if (value > DateTime.Now)
                {
                    shown = played = false;
                    _cooldownTo = value;
                }
            }
        }

        public bool SoundEnabled { get; set; }
        public bool PopupEnabled { get; set; }

        bool shown = true;
        bool played = true;

        public CooldownHandler([NotNull] ILogger logger, [NotNull] ISoundEngine soundEngine,
            [NotNull] ITrayPopups trayPopups,
            string soundName = null, string messageTitle = null, string messageContent = null, bool messagePersist = false)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            Handler = new NotifyHandler(logger, soundEngine, trayPopups);
            Handler.SoundName = (soundName ?? string.Empty);
            Handler.Title = (messageTitle ?? string.Empty);
            Handler.Message = (messageContent ?? string.Empty);
            if (messagePersist) Handler.PopupPersistent = true;
        }

        public void ResetShownAndPlayed()
        {
            shown = played = false;
        }

        public string SoundName
        {
            get { return Handler.SoundName; }
            set { Handler.SoundName = (value ?? ""); }
        }

        public string Title
        {
            get { return Handler.Title; }
            set { Handler.Title = (value ?? ""); }
        }

        public string Message
        {
            get { return Handler.Message; }
            set { Handler.Message = (value ?? ""); }
        }

        public bool PersistentPopup
        {
            get { return Handler.PopupPersistent; }
            set { Handler.PopupPersistent = value; }
        }

        public int DurationMillis
        {
            get { return Handler.Duration; }
            set { Handler.Duration = value; }
        }

        public void Update()
        {
            this.Update(false);
        }

        public void Update(bool engineSleeping)
        {
            if (DateTime.Now > CooldownTo)
            {
                if (SoundEnabled)
                {
                    if (!played)
                    {
                        Handler.Play();
                        played = true;
                    }
                }
                if (PopupEnabled)
                {
                    if (!shown)
                    {
                        Handler.Show();
                        shown = true;
                    }
                }
            }
            Handler.Update();
        }
    }

    class NotifyHandler
    {
        readonly ILogger logger;
        readonly ISoundEngine soundEngine;
        readonly ITrayPopups trayPopups;
        public string SoundName { get; set; }
        bool play;

        public string Title { get; set; }
        public string Message { get; set; }
        public bool PopupPersistent { get; set; }
        private int _Duration = 4000;
        /// <summary>
        /// Duration of the popup in milliseconds
        /// </summary>
        public int Duration { get { return _Duration; } set { _Duration = value.ConstrainToRange(1000, int.MaxValue); } }
        bool show;

        public NotifyHandler([NotNull] ILogger logger, [NotNull] ISoundEngine soundEngine,
            [NotNull] ITrayPopups trayPopups, string soundname = "", string messageTitle = "",
            string messageContent = "", bool messagePersist = false)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            this.logger = logger;
            this.soundEngine = soundEngine;
            this.trayPopups = trayPopups;
            this.SoundName = soundname;
            this.Title = messageTitle;
            this.Message = messageContent;
            this.PopupPersistent = messagePersist;
        }

        public void Update()
        {
            if (play)
            {
                //todo reimpl: store as guid, not string
                soundEngine.PlayOneShot(new Guid(SoundName));
                logger.Debug("played notify sound");
                play = false;
            }
            if (show)
            {
                if (PopupPersistent)
                    trayPopups.Schedule(Title, Message, int.MaxValue);
                else
                    trayPopups.Schedule(Title, Message, Duration);
                show = false;
            }
        }

        public void Play()
        {
            play = true;
        }

        public void Show()
        {
            show = true;
        }
    }
}
