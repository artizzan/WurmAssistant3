using System.Collections.Generic;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.DataLayer;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.Singletons
{
    [PersistentObject("GrangerFeature_GrangerSimpleDb")]
    public class GrangerSimpleDb : PersistentObjectBase
    {
        public GrangerSimpleDb()
        {
            Creatures = new Dictionary<int, CreatureEntity>();
            Herds = new Dictionary<string, HerdEntity>();
            TraitValues = new Dictionary<int, TraitValueEntity>();
        }

        [JsonProperty("creatures")]
        public Dictionary<int, CreatureEntity> Creatures { get; private set; }
        [JsonProperty("herds")]
        public Dictionary<string, HerdEntity> Herds { get; private set; }
        [JsonProperty("traitValues")]
        public Dictionary<int, TraitValueEntity> TraitValues { get; private set; }

        public void Save()
        {
            RequestSave(forceSave:true);
        } 
    }
}
