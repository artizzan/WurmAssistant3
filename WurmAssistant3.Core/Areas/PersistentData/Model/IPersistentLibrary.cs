namespace AldursLab.WurmAssistant3.Core.Areas.PersistentData.Model
{
    public interface IPersistentLibrary
    {
        void SavePending();

        void SaveAll();
    }
}