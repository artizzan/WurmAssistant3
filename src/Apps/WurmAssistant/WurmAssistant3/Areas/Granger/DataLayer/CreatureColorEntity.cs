using System.Drawing;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.DataLayer
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureColorEntity
    {
        [JsonProperty]
        string id = string.Empty;

        [JsonProperty]
        bool isReadOnly = false;

        [JsonProperty]
        Color color = Color.Empty;

        [JsonProperty]
        string wurmLogText = string.Empty;

        [JsonProperty]
        string displayName = string.Empty;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public string WurmLogText
        {
            get { return wurmLogText; }
            set { wurmLogText = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public bool IsUnknown => Id == Unknown.Id;

        public static CreatureColorEntity Unknown { get; } = new CreatureColorEntity() {Id = DefaultCreatureColorIds.Unknown };
    }
}