using System;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Timers.Modules.Timers
{
    class NotifyHandler
    {
        readonly ILogger logger;
        readonly ISoundEngine soundEngine;
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

        public NotifyHandler([NotNull] ILogger logger, [NotNull] ISoundEngine soundEngine,
            [NotNull] ITrayPopups trayPopups, Guid? soundId = null, string messageTitle = "",
            string messageContent = "", bool messagePersist = false)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            this.logger = logger;
            this.soundEngine = soundEngine;
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
                soundEngine.PlayOneShot(SoundId);
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