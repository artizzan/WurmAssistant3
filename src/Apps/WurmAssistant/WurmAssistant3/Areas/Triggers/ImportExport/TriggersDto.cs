using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Triggers.Data.Model;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    [JsonObject]
    public class TriggersDto
    {
        [JsonProperty]
        public List<TriggerEntity> TriggerEntities { get; set; }
    }
}
