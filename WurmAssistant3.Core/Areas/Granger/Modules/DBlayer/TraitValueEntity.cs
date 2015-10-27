using System;
using System.Data.Linq.Mapping;
using System.Linq;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TraitValueEntity
    {
        //primary key
        [JsonProperty("id")]
        public int ID;
        [JsonProperty("valuemapid")]
        public string ValueMapID;
        [JsonProperty("traitid")]
        string _TraitEnumINTStr;
        public HorseTrait Trait
        {
            get
            {
                return HorseTrait.FromEnumIntStr(_TraitEnumINTStr);
            }
            set
            {
                _TraitEnumINTStr = value.ToInt32().ToString();
            }
        }
        [Column(Name = "traitvalue")]
        public int Value;

        static public int GenerateNewTraitValueID(GrangerContext context)
        {
            try
            {
                return context.TraitValues.Max(x => x.ID) + 1;
            }
            catch (InvalidOperationException)
            {
                return 1;
            }
        }
    }
}