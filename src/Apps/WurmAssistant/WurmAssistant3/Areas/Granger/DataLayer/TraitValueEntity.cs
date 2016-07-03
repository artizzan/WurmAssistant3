using System;
using System.Linq;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.DataLayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TraitValueEntity
    {
        //primary key
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("valuemapid")]
        public string ValueMapId;

        [JsonProperty("traitid")]
        string traitEnumIntStr;
        public CreatureTrait Trait
        {
            get
            {
                return CreatureTrait.FromEnumIntStr(traitEnumIntStr);
            }
            set
            {
                traitEnumIntStr = value.ToInt32().ToString();
            }
        }
        [JsonProperty("traitvalue")]
        public int Value;

        public static int GenerateNewTraitValueId(GrangerContext context)
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