using System;
using System.Data.Linq.Mapping;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TraitValueEntity
    {
        //primary key
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("valuemapid")]
        public string ValueMapID;

        [JsonProperty("traitid")]
        string _TraitEnumINTStr;
        public CreatureTrait Trait
        {
            get
            {
                return CreatureTrait.FromEnumIntStr(_TraitEnumINTStr);
            }
            set
            {
                _TraitEnumINTStr = value.ToInt32().ToString();
            }
        }
        [JsonProperty("traitvalue")]
        public int Value;

        public static int GenerateNewTraitValueID(GrangerContext context)
        {
            try
            {
                return context.TraitValues.Max(x => x.Id) + 1;
            }
            catch (InvalidOperationException)
            {
                return 1;
            }
        }
    }
}