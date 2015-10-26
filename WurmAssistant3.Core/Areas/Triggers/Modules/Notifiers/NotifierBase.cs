namespace AldursLab.WurmAssistant3.Core.Areas.Triggers.Modules.Notifiers
{
    //[DataContract]
    public abstract class NotifierBase : NotifierAbstract, INotifier
    {
        public virtual bool HasEmptySound {
            get { return true; }
        }

        public abstract void Notify();

        public abstract INotifierConfig GetConfig();
    }
}
