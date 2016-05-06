using System;
using AldursLab.WurmAssistant3.Areas.TrayPopups.Contracts;
using AldursLab.WurmAssistant3.Areas.Triggers.Modules.TriggersManager;
using AldursLab.WurmAssistant3.Areas.Triggers.Views.Notifiers;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Modules.Notifiers
{
    public class PopupNotifier : NotifierBase, IPopupNotifier
    {
        readonly ITrigger trigger;
        readonly ITrayPopups trayPopups;

        public override void Notify()
        {
            trayPopups.Schedule(trigger.PopupContent,
                trigger.PopupTitle,
                trigger.StayUntilClicked ? int.MaxValue : trigger.PopupDurationMillis);
        }

        public PopupNotifier([NotNull] ITrigger trigger, [NotNull] ITrayPopups trayPopups)
        {
            if (trigger == null) throw new ArgumentNullException("trigger");
            if (trayPopups == null) throw new ArgumentNullException("trayPopups");
            this.trigger = trigger;
            this.trayPopups = trayPopups;
        }

        public string Content { get { return trigger.PopupContent; } set { trigger.PopupContent = value; } }
        public string Title { get { return trigger.PopupTitle; } set { trigger.PopupTitle = value; } }

        public TimeSpan Duration
        {
            get { return TimeSpan.FromMilliseconds(trigger.PopupDurationMillis); }
            set { trigger.PopupDurationMillis = (int)value.TotalMilliseconds; }
        }

        public bool StayUntilClicked
        {
            get { return trigger.StayUntilClicked; }
            set { trigger.StayUntilClicked = value; }
        }

        public override INotifierConfig GetConfig()
        {
            return new PopupConfig(this);
        }
    }
}
