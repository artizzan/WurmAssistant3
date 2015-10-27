using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    [PersistentObject("GrangerFeature_GrangerSimpleDb")]
    public class GrangerSimpleDb : PersistentObjectBase
    {
        public GrangerSimpleDb()
        {
            Horses = new Dictionary<int, HorseEntity>();
            Herds = new Dictionary<string, HerdEntity>();
            TraitValues = new Dictionary<int, TraitValueEntity>();
        }

        [JsonProperty("horses")]
        public Dictionary<int, HorseEntity> Horses { get; private set; }
        [JsonProperty("gerds")]
        public Dictionary<string, HerdEntity> Herds { get; private set; }
        [JsonProperty("traitValues")]
        public Dictionary<int, TraitValueEntity> TraitValues { get; private set; }

        public void Save()
        {
            RequestSave(forceSave:true);
        } 
    }
}
