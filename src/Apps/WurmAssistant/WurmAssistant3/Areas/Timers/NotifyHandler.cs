using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.SoundManager;
using AldursLab.WurmAssistant3.Areas.TrayPopups;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Timers
{
    class NotifyHandler
    {
        readonly ILogger logger;
        readonly ISoundManager soundManager;
        readonly ITrayPopups trayPopups;
        public Guid SoundId { get; set; }
        bool play;

        public string Title { get; set; }
        public string Message { get; set; }
        public bool PopupPersistent { get; set; }
        private int duration = 4000;
        /// <summary>
        /// Duration of the popup in milliseconds
        /// </summary>
        public int Duration { get { return duration; } set { duration = value.ConstrainToRange(1000, int.MaxValue); } }
        bool show;

        public NotifyHandler([NotNull] ILogger logger, [NotNull] ISoundManager soundManager,
            [NotNull] ITrayPopups trayPopups, Guid? soundId = null, string messageTitle = "",
            string messageContent = "", bool messagePersist = false)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundManager == null) throw new ArgumentNullException("soundManager");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            this.logger = logger;
            this.soundManager = soundManager;
            this.trayPopups = trayPopups;
            this.SoundId = soundId ?? Guid.Empty;
            this.Title = messageTitle;
            this.Message = messageContent;
            this.PopupPersistent = messagePersist;
        }

        public void Update()
        {
            if (play)
            {
                //todo reimpl: store as guid, not string
                soundManager.PlayOneShot(SoundId);
                logger.Debug("played notify sound");
                play = false;
            }
            if (show)
            {
                if (PopupPersistent)
                    trayPopups.Schedule(Message, Title, int.MaxValue);
                else
                    trayPopups.Schedule(Message, Title, Duration);
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