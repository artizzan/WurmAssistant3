using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace AldurSoft.SimplePersist.Serializers.JsonNet
{
    public class JsonSerializationStrategy : ISerializationStrategy
    {
        private JsonSerializer Deserializer { get; set; }
        private JsonSerializer Serializer { get; set; }

        public JsonSerializationStrategy()
        {
            var customSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            };
            Deserializer = JsonSerializer.Create(customSettings);
            Serializer = JsonSerializer.Create(customSettings);
        }

        public SerializationResult<TEntity> Deserialize<TEntity>(string source) where TEntity : class, new()
        {
            using (var sr = new StringReader(source))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    List<Exception> deserializationErrors = new List<Exception>();
                    EventHandler<ErrorEventArgs> deserializerOnError = (sender, args) =>
                        {
                            args.ErrorContext.Handled = true;
                            deserializationErrors.Add(args.ErrorContext.Error);
                        };
                    try
                    {
                        Deserializer.Error += deserializerOnError;
                        var deserialized = Deserializer.Deserialize<TEntity>(jtr);
                        if (deserialized == null)
                        {
                            deserialized = new TEntity();
                        }
                        if (deserializationErrors.Any())
                        {
                            return new SerializationResult<TEntity>(
                                new TEntity(),
                                new SerializationErrors(deserializationErrors));
                        }
                        else
                        {
                            return new SerializationResult<TEntity>(deserialized);
                        }
                    }
                    finally
                    {
                        Deserializer.Error -= deserializerOnError;
                    }
                }
            }
        }

        public string Serialize<TEntity>(TEntity entity) where TEntity : class, new()
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    Serializer.Serialize(jtw, entity);
                }
            }
            return sb.ToString();
        }
    }
}
