namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    public interface IChangelogManager
    {
        string GetNewChanges();

        void UpdateLastChangeDate();

        void ShowChanges(string changesText);
    }
}
