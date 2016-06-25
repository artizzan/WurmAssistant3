using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AldursLab.Persistence.Simple
{
    public class DefaultJsonSerializer : ISerializer
    {
        readonly JsonSerializer serializer;

        public DefaultJsonSerializer()
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new ExtendedContractResolver()
            };
            serializer = JsonSerializer.Create(settings);
        }

        public string Serialize<T>(T @object)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    serializer.Serialize(jtw, @object);
                }
            }
            return sb.ToString();
        }

        public T Deserialize<T>(string data)
        {
            using (var sr = new StringReader(data))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<T>(jtr);
                }
            }
        }

        class ExtendedContractResolver : DefaultContractResolver
        {
            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                var contract = base.CreateDictionaryContract(objectType);

                // Serialize Enums as numeric.
                var keyType = contract.DictionaryKeyType;
                if (keyType.BaseType == typeof(Enum))
                {
                    contract.DictionaryKeyResolver =
                        propName => ((int)Enum.Parse(keyType, propName)).ToString();
                }

                return contract;
            }
        }
    }
}