using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AldursLab.WurmApi.PersistentObjects
{
    class JsonSerializationStrategy : ISerializationStrategy
    {
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
            Serializer = JsonSerializer.Create(customSettings);
        }

        public TEntity Deserialize<TEntity>(string source) where TEntity : Entity, new()
        {
            using (var sr = new StringReader(source))
            {
                using (var jtr = new JsonTextReader(sr))
                {
                    var deserializer = CreateDeserializer();
                    List<DeserializationErrorDetails> errors =
                        new List<DeserializationErrorDetails>();
                    deserializer.Error += (sender, args) =>
                    {
                        errors.Add(new DeserializationErrorDetails()
                        {
                            Exception = args.ErrorContext.Error,
                            Path = args.ErrorContext.Path,
                            DeserializationErrorKind = DeserializationErrorKind.DeserializationOfSomeMembersFailed
                        });
                        args.ErrorContext.Handled = true;
                    };
                    var deserialized = deserializer.Deserialize<TEntity>(jtr);
                    if (deserialized == null)
                    {
                        throw new DeserializationErrorsException<TEntity>(new TEntity(), new [] {new DeserializationErrorDetails()
                        {
                            DeserializationErrorKind = DeserializationErrorKind.DeserializationImpossible,
                        }});
                    }
                    if (errors.Any())
                    {
                        throw new DeserializationErrorsException<TEntity>(deserialized, errors);
                    }
                    return deserialized;
                }
            }
        }

        public string Serialize<TEntity>(TEntity entity) where TEntity : Entity, new()
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

        JsonSerializer CreateDeserializer()
        {
            var customSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            };
            var deserializer = JsonSerializer.Create(customSettings);
            return deserializer;
        }
    }

    public class DeserializationErrorDetails
    {
        /// <summary>
        /// Actual exception that caused this error, or null if no exception.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Path of serialized value, that caused the error, or null if not available.
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Type of the error.
        /// </summary>
        public DeserializationErrorKind DeserializationErrorKind { get; internal set; }

        public override string ToString()
        {
            return $"Kind: {DeserializationErrorKind}, Path: {Path}, Exception: {Exception?.Message ?? string.Empty}";
        }
    }

    public enum DeserializationErrorKind
    {
        /// <summary>
        /// Deserializer returned nothing, which might indicate a corrupted file.
        /// Ignoring this error will cause a fresh instance of Dto to be created, with default values.
        /// </summary>
        DeserializationImpossible,
        /// <summary>
        /// At least one member failed at deserialization (for example: underlying type mismatch).
        /// Check Exception, Path and Member for details.
        /// Ignoring this error will cause fallback to defaults for errored members only.
        /// </summary>
        DeserializationOfSomeMembersFailed,
        /// <summary>
        /// ObjectId of deserialized object is different, than requested ObjectId.
        /// Ignoring this error will overwrite objectId to the expected one.
        /// Note that underlying object will be overwritten on save.
        /// This kind of error should not happen, unless wrong id's are used on the consumer code side 
        /// or files were manually modified/corrupted.
        /// </summary>
        ObjectIdMismatch
    }

    [Serializable]
    class DeserializationErrorsException<TEntity> : Exception where TEntity : Entity, new()
    {
        public DeserializationErrorsException(
            TEntity deserializedFallbackEntity,
            IEnumerable<DeserializationErrorDetails> errors)
            : base("At least one deserialization error occurred and was not handled.")
        {
            if (deserializedFallbackEntity == null) throw new ArgumentNullException(nameof(deserializedFallbackEntity));
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            DeserializedFallbackEntity = deserializedFallbackEntity;
            Errors = errors;
        }

        public TEntity DeserializedFallbackEntity { get; private set; }
        public IEnumerable<DeserializationErrorDetails> Errors { get; private set; }
    }
}