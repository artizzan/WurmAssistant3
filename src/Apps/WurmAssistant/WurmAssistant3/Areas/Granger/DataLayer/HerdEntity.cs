using System;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.DataLayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HerdEntity
    {
        [JsonProperty]
        public Guid GlobalId = Guid.NewGuid();

        //primary key
        [JsonProperty("herdId")]
        public string HerdId;

        [JsonProperty("selected")]
        bool? selected;
        public bool Selected
        {
            get { return selected ?? false; }
            set { selected = value; }
        }

        public override string ToString()
        {
            return HerdId;
        }

        internal HerdEntity CloneMe(string newHerdName)
        {
            var newEntity = (HerdEntity)this.MemberwiseClone();
            newEntity.HerdId = newHerdName;
            return newEntity;
        }

        public string HerdIdAspect => this.HerdId;

        public bool CheckedAspect => this.Selected;
    }
}
