using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi.PersistentObjects
{
    class PersistentCollection : IPersistentCollection
    {
        readonly string collectionId;
        readonly IPersistenceStrategy persistenceStrategy;
        readonly ISerializationStrategy serializationStrategy;
        readonly IObjectDeserializationErrorHandlingStrategy deserializationErrorHandlingStrategy;
        readonly Dictionary<string, Persistent> objects = new Dictionary<string, Persistent>();

        internal PersistentCollection(string collectionId, IPersistenceStrategy persistenceStrategy,
            ISerializationStrategy serializationStrategy,
            IObjectDeserializationErrorHandlingStrategy deserializationErrorHandlingStrategy)
        {
            this.collectionId = collectionId;
            this.persistenceStrategy = persistenceStrategy;
            this.serializationStrategy = serializationStrategy;
            this.deserializationErrorHandlingStrategy = deserializationErrorHandlingStrategy;
        }

        internal void SaveChanged()
        {
            Save(false);
        }

        internal void SaveAll()
        {
            Save(true);
        }

        void Save(bool all)
        {
            foreach (var persistent in objects)
            {
                if (all || persistent.Value.HasChanged)
                {
                    var data = persistent.Value.GetSerializedData(serializationStrategy);
                    persistenceStrategy.Save(persistent.Key, collectionId, data);
                    persistent.Value.HasChanged = false;
                }
            }
        }

        public IPersistent<TEntity> GetObject<TEntity>(string objectId)
            where TEntity : Entity, new()
        {
            Persistent persistent;
            if (objects.TryGetValue(objectId, out persistent))
            {
                var castObj = persistent as Persistent<TEntity>;
                if (castObj == null)
                {
                    var type = persistent.GetType();
                    throw new InvalidOperationException(
                        string.Format(
                            "Previously resolved Persistent<T> is of entity type different than requested, requested type: {0}, actual type: {1}",
                            typeof(Persistent<TEntity>).FullName, type.FullName));
                }
                return castObj;
            }

            Persistent<TEntity> obj = null;
            bool retry;
            int currentRetry = 0;
            int retryMax = 100;
            do
            {
                var resolver = new ObjectResolver<TEntity>(objectId, collectionId, persistenceStrategy,
                    serializationStrategy, deserializationErrorHandlingStrategy);
                try
                {
                    obj = resolver.GetObject();
                    retry = false;
                }
                catch (RetryException)
                {
                    if (retryMax == currentRetry)
                    {
                        throw new InvalidOperationException(string.Format("Maximum retry count reached ({0})", retryMax));
                    }
                    retry = true;
                    currentRetry++;
                }
            } while (retry);

            objects.Add(objectId, obj);
            return obj;
        }

        class ObjectResolver<TEntity> where TEntity : Entity, new()
        {
            readonly string objectId;
            readonly string collectionId;
            readonly IPersistenceStrategy persistenceStrategy;
            readonly ISerializationStrategy serializationStrategy;
            readonly IObjectDeserializationErrorHandlingStrategy objectDeserializationErrorHandlingStrategy;

            public ObjectResolver(string objectId, string collectionId, IPersistenceStrategy persistenceStrategy,
                ISerializationStrategy serializationStrategy,
                IObjectDeserializationErrorHandlingStrategy objectDeserializationErrorHandlingStrategy)
            {
                if (objectId == null) throw new ArgumentNullException(nameof(objectId));
                if (collectionId == null) throw new ArgumentNullException(nameof(collectionId));
                if (persistenceStrategy == null) throw new ArgumentNullException(nameof(persistenceStrategy));
                if (serializationStrategy == null) throw new ArgumentNullException(nameof(serializationStrategy));
                if (objectDeserializationErrorHandlingStrategy == null)
                    throw new ArgumentNullException(nameof(objectDeserializationErrorHandlingStrategy));
                this.objectId = objectId;
                this.collectionId = collectionId;
                this.persistenceStrategy = persistenceStrategy;
                this.serializationStrategy = serializationStrategy;
                this.objectDeserializationErrorHandlingStrategy = objectDeserializationErrorHandlingStrategy;
            }

            public Persistent<TEntity> GetObject()
            {
                var o = new Persistent<TEntity>(objectId);
                var data = persistenceStrategy.TryLoad(objectId, collectionId);
                if (data == null)
                {
                    // data does not exist, return new with default
                    return o;
                }

                try
                {
                    o.Entity = serializationStrategy.Deserialize<TEntity>(data);
                }
                catch (DeserializationErrorsException<TEntity> exception)
                {
                    var errorContext = new ErrorContext(exception.Errors, persistenceStrategy, collectionId,
                        objectId);

                    objectDeserializationErrorHandlingStrategy.Handle(errorContext);

                    if (errorContext.Decision == Decision.DoNotIgnoreAndRethrowTheException)
                    {
                        throw;
                    }
                    else if (errorContext.Decision == Decision.IgnoreErrorsAndReturnDefaultsForMissingData)
                    {
                        o.Entity = exception.DeserializedFallbackEntity;
                        return o;
                    }
                    else if (errorContext.Decision == Decision.RetryDeserialization)
                    {
                        throw new RetryException();
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown Decision: " + errorContext.Decision);
                    }
                }

                if (o.Entity.ObjectId != objectId)
                {
                    var errorContext = new ErrorContext(
                        new[]
                        {
                            new DeserializationErrorDetails()
                            {
                                DeserializationErrorKind = DeserializationErrorKind.ObjectIdMismatch
                            }
                        },
                        persistenceStrategy, collectionId, objectId);

                    objectDeserializationErrorHandlingStrategy.Handle(errorContext);

                    if (errorContext.Decision == Decision.DoNotIgnoreAndRethrowTheException)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Deserialized entity objectId {0} is different than requested objectId {1}",
                                o.Entity.ObjectId, objectId));
                    }
                    else if (errorContext.Decision == Decision.IgnoreErrorsAndReturnDefaultsForMissingData)
                    {
                        o.Entity.ObjectId = objectId;
                    }
                    else if (errorContext.Decision == Decision.RetryDeserialization)
                    {
                        throw new RetryException();
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown Decision: " + errorContext.Decision);
                    }
                }

                return o;
            }
        }

        [Serializable]
        public class RetryException : Exception
        {
        }
    }
}