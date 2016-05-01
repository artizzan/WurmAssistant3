using System;
using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Modules.Wurm.LogDefinitions
{
    class WurmLogDefinitions : IWurmLogDefinitions
    {
        private static readonly LogDefinition[] Definitions = new[]
        {
            new LogDefinition(LogType.Combat, "_Combat", "Combat"),
            new LogDefinition(LogType.Event, "_Event", "Event"),
            new LogDefinition(LogType.Friends, "_Friends", "Friends"),
            new LogDefinition(LogType.Local, "_Local", "Local"),
            new LogDefinition(LogType.Skills, "_Skills", "Skills"),
            new LogDefinition(LogType.Alliance, "Alliance", "Alliance"),
            new LogDefinition(LogType.CaHelp, "CA_HELP", "Community Assistance"),
            new LogDefinition(LogType.Freedom, "Freedom", "Freedom Kingdom"),
            new LogDefinition(LogType.GlFreedom, "GL-Freedom", "Freedom Kingdom Global"),
            new LogDefinition(LogType.Mgmt, "MGMT", "MGMT"),
            new LogDefinition(LogType.Pm, "PM", "Private Messages"),
            new LogDefinition(LogType.Team, "Team", "Team"),
            new LogDefinition(LogType.Village, "Village", "Village"),
            new LogDefinition(LogType.Deaths, "_Deaths", "Deaths"),
            new LogDefinition(LogType.MolRehan, "Mol_Rehan", "Mol-Rehan Kingdom"),
            new LogDefinition(
                LogType.GlMolRehan,
                "GL-Mol_Rehan",
                "Mol-Rehan Kingdom Global"),
            new LogDefinition(LogType.JennKellon, "Jenn-Kellon", "Jenn-Kellon Kingdom"),
            new LogDefinition(
                LogType.GlJennKellon,
                "GL-Jenn-Kellon",
                "Jenn-Kellon Kingdom Global"),
            new LogDefinition(LogType.Hots, "HOTS", "Horde of the Summoned Kingdom"),
            new LogDefinition(
                LogType.GlHots,
                "GL-HOTS",
                "Horde of the Summoned Kingdom Global"),  
            new LogDefinition(LogType.Trade, "Trade", "Trade"), 
            new LogDefinition(LogType.Support, "_Support", "Support"), 
            new LogDefinition(LogType.Help, "_Help", "Help"), 
        };

        readonly Dictionary<string, LogType> prefixToEnumMap = new Dictionary<string, LogType>();
        readonly Dictionary<LogType, string> enumToPrefixMap = new Dictionary<LogType, string>();

        public WurmLogDefinitions()
        {
            foreach (var logDefinition in Definitions)
            {
                AddLogTypeMapping(logDefinition.LogPrefix, logDefinition.LogType);
            }

            //AddLogTypeMapping("_Combat", LogType.Combat);
            //AddLogTypeMapping("_Event", LogType.Event);
            //AddLogTypeMapping("_Friends", LogType.Friends);
            //AddLogTypeMapping("_Local", LogType.Local);
            //AddLogTypeMapping("_Skills", LogType.Skills);
            //AddLogTypeMapping("Alliance", LogType.Alliance);
            //AddLogTypeMapping("CA_HELP", LogType.CaHelp);
            //AddLogTypeMapping("Freedom", LogType.Freedom);
            //AddLogTypeMapping("GL-Freedom", LogType.GlFreedom);
            //AddLogTypeMapping("MGMT", LogType.Mgmt);
            //AddLogTypeMapping("PM", LogType.Pm);
            //AddLogTypeMapping("Team", LogType.Team);
            //AddLogTypeMapping("Village", LogType.Village);
            //AddLogTypeMapping("_Deaths", LogType.Deaths);
            //AddLogTypeMapping("Mol_Rehan", LogType.MolRehan);
            //AddLogTypeMapping("GL-Mol_Rehan", LogType.GlMolRehan);
            //AddLogTypeMapping("Jenn-Kellon", LogType.JennKellon);
            //AddLogTypeMapping("GL-Jenn-Kellon", LogType.GlJennKellon);
            //AddLogTypeMapping("HOTS", LogType.Hots);
            //AddLogTypeMapping("GL-HOTS", LogType.GlHots);
            //AddLogTypeMapping("Trade", LogType.Trade);
            //AddLogTypeMapping("_Support", LogType.Support);

            var allTypesCount = AllLogTypes.Count();
            var allDefinitionsCount = AllDefinitions.Count();
            if (allTypesCount != allDefinitionsCount)
            {
                throw new InvalidOperationException("AllDefinitions != AllLogTypes");
            }
        }

        public IEnumerable<LogDefinition> AllDefinitions => Definitions.AsEnumerable();

        public IEnumerable<LogType> AllLogTypes => enumToPrefixMap.Keys.ToArray();

        /// <summary>
        /// Returns Unspecified if failed.
        /// </summary>
        /// <param name="logFileName"></param>
        /// <returns></returns>
        public LogType TryGetTypeFromLogFileName(string logFileName)
        {
            foreach (string name in GetAllNames())
            {
                if (logFileName.StartsWith(name, StringComparison.Ordinal))
                {
                    var type = GetLogTypeForPrefix(name);
                    return type;
                }
            }
            return LogType.Unspecified;
        }

        void AddLogTypeMapping(string logName, LogType logType)
        {
            prefixToEnumMap.Add(logName, logType);
            enumToPrefixMap.Add(logType, logName);
        }

        private LogType GetLogTypeForPrefix(string logFileName)
        {
            return prefixToEnumMap[logFileName];
        }

        private string[] GetAllNames()
        {
            return prefixToEnumMap.Keys.ToArray();
        }
    }
}