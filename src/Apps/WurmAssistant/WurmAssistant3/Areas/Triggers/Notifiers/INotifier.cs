using System;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Areas.Triggers.Notifiers
{
    public interface INotifier
    {
        void Notify();
        INotifierConfig GetConfig();
        bool HasEmptySound { get; }
    }

    public interface ISoundNotifier
    {
        Guid SoundId { get; set; }
    }

    public interface IMessageNotifier
    {
        string Title { get; set; }
        string Content { get; set; }
    }

    public interface IPopupNotifier
    {
        string Title { get; set; }
        string Content { get; set; }
        TimeSpan Duration { get; set; }
        bool StayUntilClicked { get; set; }
    }

    public interface INotifierConfig
    {
        UserControl ControlHandle { get; }
        event EventHandler Removed;
    }
}
