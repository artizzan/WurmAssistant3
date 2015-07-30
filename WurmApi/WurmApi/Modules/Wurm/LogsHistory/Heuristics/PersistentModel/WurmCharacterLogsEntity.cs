using System.Collections.Generic;
using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel
{
    [JsonObject(MemberSerialization.Fields)]
    public class WurmCharacterLogsEntity : Entity
    {
        private readonly Dictionary<string, WurmLogMonthlyFile> wurmLogFiles = new Dictionary<string, WurmLogMonthlyFile>();

        /// <summary>
        /// Key: file name normalized, Value: file information
        /// </summary>
        public Dictionary<string, WurmLogMonthlyFile> WurmLogFiles
        {
            get { return wurmLogFiles; }
        }
    }
}