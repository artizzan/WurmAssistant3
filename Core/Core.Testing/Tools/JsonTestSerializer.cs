using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AldurSoft.Core.Testing.Tools
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
