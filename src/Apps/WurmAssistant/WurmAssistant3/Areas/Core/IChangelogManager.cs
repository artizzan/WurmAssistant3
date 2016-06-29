using System;

namespace AldursLab.WurmAssistant3.Areas.Core
{
    public interface IChangelogManager
    {
        string GetNewChanges();

        void UpdateLastChangeDate();

        void ShowChanges(string changesText);

        string GetChanges(DateTimeOffset sinceDate);
    }
}
