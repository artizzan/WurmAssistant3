using System;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatStats.Modules
{
    class CombatResultsProcessor
    {
        readonly CombatResults combatResults;

        public CombatResultsProcessor(CombatResults combatResults)
        {
            if (combatResults == null) throw new ArgumentNullException("combatResults");
            this.combatResults = combatResults;
        }

        public void ProcessEntry(LogEntry logEntry)
        {


            // add to combatResults
        }
    }
}
