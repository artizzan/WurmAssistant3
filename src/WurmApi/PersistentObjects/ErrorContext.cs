using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi.PersistentObjects
{
    class ErrorContext
    {
        readonly IPersistenceStrategy persistenceStrategy;
        readonly string collectionId;
        readonly string objectId;
        string rawSerializedData = null;

        public ErrorContext(IEnumerable<DeserializationErrorDetails> deserializationErrorDetails,
            IPersistenceStrategy persistenceStrategy, string collectionId, string objectId)
        {
            if (deserializationErrorDetails == null) throw new ArgumentNullException(nameof(deserializationErrorDetails));
            if (persistenceStrategy == null) throw new ArgumentNullException(nameof(persistenceStrategy));
            if (collectionId == null) throw new ArgumentNullException(nameof(collectionId));
            if (objectId == null) throw new ArgumentNullException(nameof(objectId));
            this.persistenceStrategy = persistenceStrategy;
            this.collectionId = collectionId;
            this.objectId = objectId;
            DeserializationErrorDetails = deserializationErrorDetails;
            // default decision:
            Decision = Decision.DoNotIgnoreAndRethrowTheException;
        }

        public IEnumerable<DeserializationErrorDetails> DeserializationErrorDetails { get; private set; }

        public Decision Decision { get; set; }

        /// <summary>
        /// Gets the serialized raw data from the backing data store.
        /// </summary>
        public string RawSerializedData => rawSerializedData ?? (rawSerializedData = persistenceStrategy.TryLoad(objectId, collectionId));

        /// <summary>
        /// Overwrites serialized data with the new data.
        /// </summary>
        /// <param name="newData"></param>
        public void OverwriteSerializedData(string newData)
        {
            persistenceStrategy.Save(objectId, collectionId, newData);
            rawSerializedData = null;
        }

        public string GetErrorDetailsAsString()
        {
            return string.Join(", ", DeserializationErrorDetails);
        }
    }

    public enum Decision
    {
        /// <summary>
        /// Exception will be rethrown and object will not be initialized.
        /// </summary>
        DoNotIgnoreAndRethrowTheException = 0,
        /// <summary>
        /// All deserialization errors will be ignored and missing data filled with defaults, as specified for Dto type.
        /// </summary>
        IgnoreErrorsAndReturnDefaultsForMissingData,
        /// <summary>
        /// The entire deserialization attempt will be repeated.
        /// </summary>
        RetryDeserialization
    }
}