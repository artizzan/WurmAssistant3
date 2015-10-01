using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.TrayPopups.Modules
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PopupMessage
    {
        [JsonProperty]
        private string title;
        [JsonProperty]
        private string content;
        [JsonProperty]
        private int duration;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
    }
}
