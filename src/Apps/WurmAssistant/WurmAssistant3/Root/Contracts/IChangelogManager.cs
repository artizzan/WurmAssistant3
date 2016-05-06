namespace AldursLab.WurmAssistant3.Root.Contracts
{
    public interface IChangelogManager
    {
        string GetNewChanges();

        void UpdateLastChangeDate();

        void ShowChanges(string changesText);
    }
}
