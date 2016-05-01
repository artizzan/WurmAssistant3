using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AldursLab.WurmApi.Tests.Unit.Testutils
{
    public class JsonTestSerializer<T>
    {
        readonly JsonSerializer serializer;

        public JsonTestSerializer()
        {
            var customSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            };

            serializer = JsonSerializer.Create(customSettings);
        }

        public T Deserialize(string json)
        {
            T deserialized;
            using (var sr = new StringReader(json))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    deserialized = serializer.Deserialize<T>(jtr);
                }
            }
            return deserialized;
        }

        public string Serialize(T obj)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    serializer.Serialize(jtw, obj);
                }
            }
            var serialized = sb.ToString();
            return serialized;
        }

        public T Reserialize(T source)
        {
            return Deserialize(Serialize(source));
        }
    }
}
