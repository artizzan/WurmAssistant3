using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant3.Core.Root.Contracts
{
    public interface IChangelogManager
    {
        string GetNewChanges();

        void UpdateLastChangeDate();

        void ShowChanges(string changesText);
    }
}
