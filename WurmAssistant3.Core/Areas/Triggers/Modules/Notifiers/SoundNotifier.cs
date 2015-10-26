using System;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Core.Areas.Triggers.Views.Notifiers;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers
{
    public class SoundNotifier : NotifierBase, ISoundNotifier
    {
        readonly ITrigger trigger;
        readonly ISoundEngine soundEngine;

        public Guid SoundId
        {
            get { return trigger.SoundId; }
            set
            {
                trigger.SoundId = value;
                SoundResource = soundEngine.GetSoundById(value);
            }
        }

        ISoundResource SoundResource { get; set; }

        public override bool HasEmptySound
        {
            get
            {
                if (SoundResource == null) return true;
                return SoundResource.Id == Guid.Empty;
            }
        }

        public SoundNotifier([NotNull] ITrigger trigger, [NotNull] ISoundEngine soundEngine)
        {
            if (trigger == null) throw new ArgumentNullException("trigger");
            if (soundEngine == null) throw new ArgumentNullException("soundEngine");
            this.trigger = trigger;
            this.soundEngine = soundEngine;
            // restore previous sound, if still in sounds library
            this.SoundResource = soundEngine.GetSoundById(trigger.SoundId);
        }

        public override INotifierConfig GetConfig()
        {
            return new SoundConfig(this, soundEngine);
        }

        public override void Notify()
        {
            var handle = soundEngine.PlayOneShot(SoundResource);
            if (handle.IsNullSound) this.SoundId = Guid.Empty;
        }
    }
}
