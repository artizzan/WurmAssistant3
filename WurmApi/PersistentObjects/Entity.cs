using Newtonsoft.Json;

namespace AldursLab.WurmApi.PersistentObjects
{
    abstract class Entity
    {
        [JsonProperty]
        public string ObjectId { get; internal set; }
        [JsonProperty]
        public int Version { get; internal set; }
    }
}