using System.Collections.Generic;
using Newtonsoft.Json;

namespace AldurSoft.WurmApi.Modules.DataContext.DataModel.LogsHistoryModel
{
    [JsonObject(MemberSerialization.Fields)]
    public class WurmCharacterLogsEntity
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