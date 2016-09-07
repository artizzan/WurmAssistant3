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

        public bool IsUnknown => Id == Unknown.Id;

        public static CreatureColorEntity Unknown { get; } = new CreatureColorEntity() {Id = "Unknown"};
    }
}