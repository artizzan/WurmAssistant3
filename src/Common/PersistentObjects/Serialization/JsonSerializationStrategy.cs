using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace AldursLab.PersistentObjects.Serialization
{
    public class JsonSerializationStrategy : ISerializationStrategy
    {
        IJsonErrorHandlingStrategy errorStrategy;
        private JsonSerializer Serializer { get; set; }
        private JsonSerializer Deserializer { get; set; }

        public JsonSerializationStrategy()
        {
            var customSettings = CreateDefaultJsonSerializerSettings();

            // set default error handling policy
            ErrorStrategy = new JsonDefaultErrorHandlingStrategy();

            Serializer = CreateSerializer(customSettings);
            Serializer.Error += SerializerOnError;

            Deserializer = CreateSerializer(customSettings);
            Deserializer.Error += DeserializerOnError;
        }

        void SerializerOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            errorStrategy.HandleErrorOnSerialize(sender, errorEventArgs);
        }

        void DeserializerOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            errorStrategy.HandleErrorOnDeserialize(sender, errorEventArgs);
        }

        // allow overriding default policy
        public IJsonErrorHandlingStrategy ErrorStrategy
        {
            get { return errorStrategy; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                errorStrategy = value;
            }
        }

        public string Serialize<T>(T @object) where T : class
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var jtw = new JsonTextWriter(sw))
                {
                    Serializer.Serialize(jtw, @object);
                }
            }
            return sb.ToString();
        }

        public void PopulateFromSerialized<T>(T @object, string data) where T : class
        {
            var previewResult = errorStrategy.PreviewJsonStringOnPopulate(data, @object);

            if (previewResult.BreakPopulating)
            {
                return;
            }

            using (var sr = new StringReader(data))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    Deserializer.Populate(jtr, @object);
                }
            }
        }

        private JsonSerializer CreateSerializer(JsonSerializerSettings settings)
        {
            return JsonSerializer.Create(settings);
        }

        private JsonSerializerSettings CreateDefaultJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ContractResolver = new CustomResolver()
            };
        }
    }

    class CustomResolver : DefaultContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);

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