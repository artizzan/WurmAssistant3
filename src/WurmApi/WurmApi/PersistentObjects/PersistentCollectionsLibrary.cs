using System;
using System.Collections.Generic;
using AldursLab.WurmApi.Extensions.DotNet.Collections.Generic;

namespace AldursLab.WurmApi.PersistentObjects
{
    class PersistentCollectionsLibrary : IPersistentCollectionsLibrary
    {
        readonly IPersistenceStrategy persistenceStrategy;
        readonly IObjectDeserializationErrorHandlingStrategy deserializationErrorHandlingStrategy;
        readonly ISerializationStrategy serializationStrategy;
        readonly IDictionary<string, PersistentCollection> collections = new Dictionary<string, PersistentCollection>();

        internal const string DefaultCollectionId = "_default";

        public PersistentCollectionsLibrary(IPersistenceStrategy persistenceStrategy, 
            IObjectDeserializationErrorHandlingStrategy deserializationErrorHandlingStrategy = null)
        {
            if (persistenceStrategy == null) throw new ArgumentNullException(nameof(persistenceStrategy));
            this.persistenceStrategy = persistenceStrategy;

            if (deserializationErrorHandlingStrategy == null)
            {
                this.deserializationErrorHandlingStrategy = new DefaultObjectDeserializationErrorHandlingStrategy();
            }
            else
            {
                this.deserializationErrorHandlingStrategy = deserializationErrorHandlingStrategy;
            }

            serializationStrategy = new JsonSerializationStrategy();
        }

        public IPersistentCollection DefaultCollection
        {
            get
            {
                return collections.GetOrAdd(DefaultCollectionId,
                    () =>
                        new PersistentCollection(DefaultCollectionId, persistenceStrategy, serializationStrategy,
                            deserializationErrorHandlingStrategy));
            }
        }

        public IPersistentCollection GetCollection(string collectionId)
        {
            PersistentObjectValidator.ValidateCollectionId(collectionId);
            return collections.GetOrAdd(collectionId,
                () =>
                    new PersistentCollection(collectionId, persistenceStrategy, serializationStrategy,
                        deserializationErrorHandlingStrategy));
        }

        public void SaveChanged()
        {
            foreach (var persistentObjectCollection in collections.Values)
            {
                persistentObjectCollection.SaveChanged();
            }
        }

        public void SaveAll()
        {
            foreach (var persistentObjectCollection in collections.Values)
            {
                persistentObjectCollection.SaveAll();
            }
        }

        internal IPersistenceStrategy PersistenceStrategy => persistenceStrategy;
    }

    /// <summary>
    /// This strategy should be implemented to handle any deserialization errors.
    /// </summary>
    interface IObjectDeserializationErrorHandlingStrategy
    {
        /// <summary>
        /// This method is called, when deserialization-specific error occours during persistent object instantiation.
        /// The default handling strategy is to throw exception at persistent object construction.
        /// Strategy can take action and decide what to do. The appropriate setting can be set on ErrorContext.
        /// </summary>
        /// <param name="errorContext"></param>
        void Handle(ErrorContext errorContext);
    }

    internal class DefaultObjectDeserializationErrorHandlingStrategy : IObjectDeserializationErrorHandlingStrategy
    {
        public void Handle(ErrorContext errorContext)
        {
            // do nothing, return default decision
        }
    }
}