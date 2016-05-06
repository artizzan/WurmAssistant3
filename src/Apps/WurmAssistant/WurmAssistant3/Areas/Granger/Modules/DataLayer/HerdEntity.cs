using System;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.Modules.DataLayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HerdEntity
    {
        [JsonProperty]
        public Guid GlobalId = Guid.NewGuid();

        //primary key
        [JsonProperty("herdId")]
        public string HerdID;

        [JsonProperty("selected")]
        bool? _Selected;
        public bool Selected
        {
            get { return _Selected == null ? false : _Selected.Value; }
            set { _Selected = value; }
        }

        public override string ToString()
        {
            return HerdID;
        }

        internal HerdEntity CloneMe(string newHerdName)
        {
            var newEntity = (HerdEntity)this.MemberwiseClone();
            newEntity.HerdID = newHerdName;
            return newEntity;
        }

        public string HerdIDAspect
        {
            get { return this.HerdID; }
        }

        public bool CheckedAspect
        {
            get { return this.Selected; }
        }
    }
}
