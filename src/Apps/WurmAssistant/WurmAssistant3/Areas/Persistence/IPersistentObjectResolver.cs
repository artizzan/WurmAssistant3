using System;
using System.Runtime.Serialization;
using AldursLab.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Persistence
{
    /// <summary>
    /// General purpose interface to manage all types of persistent objects
    /// </summary>
    public interface IPersistentObjectResolver
    {
        /// <summary>
        /// Retrieves an object of type T with specified id.
        /// If the object does not exist, it is created from Kernel, based on binding strategy.
        /// </summary>
        /// <param name="persistentObjectId"></param>
        /// <returns></returns>
        T Get<T>(string persistentObjectId) where T : class, IPersistentObject;

        /// <summary>
        /// Retrieves an object of objectType with specified id.
        /// If the object does not exist, it is created from Kernel, based on binding strategy.
        /// Object must implement
        /// </summary>
        /// <param name="persistentObjectId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        /// <exception cref="UnsupportedObjectTypeException">Type does not implement IPersistentObject</exception>
        object Get(string persistentObjectId, Type objectType);

        /// <summary>
        /// Retrieves an object of type T with default id (string.Empty).
        /// If the object does not exist, it is created from Kernel, based on binding strategy.
        /// </summary>
        /// <returns></returns>
        T GetDefault<T>() where T : class, IPersistentObject;

        /// <summary>
        /// Retrieves an object of objectType with default id (string.Empty).
        /// If the object does not exist, it is created from Kernel, based on binding strategy.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        /// <exception cref="UnsupportedObjectTypeException">Type does not implement IPersistentObject</exception>
        object GetDefault(Type objectType);

        /// <summary>
        /// Immediatelly stops tracking of the given object and removes it from cache.
        /// Data in the object is not saved. Future requests to save this object will do nothing.
        /// </summary>
        /// <param name="object"></param>
        void Unload<T>(T @object) where T : class, IPersistentObject;

        /// <summary>
        /// Immediatelly stops tracking of the given object and removes it from cache.
        /// Data in the object is not saved. Future requests to save this object will do nothing.
        /// </summary>
        /// <param name="object"></param>
        /// <exception cref="UnsupportedObjectTypeException">Type does not implement IPersistentObject</exception>
        void Unload(object @object);

        /// <summary>
        /// Attempts to enable persistence management for given object and add it to the cache.
        /// If the object of same Category and Id already exists in the cache, exception will be thrown.
        /// Note, that object will not go through usual Kernel pipeline, eg. IInitialize will not be called automatically.
        /// </summary>
        /// <param name="object"></param>
        /// <exception cref="PersistentObjectAlreadyTrackedException">
        /// Object of this type and id is already cached. Use Get method instead.
        /// </exception>
        void StartTracking<T>(T @object) where T : class, IPersistentObject;

        /// <summary>
        /// Attempts to enable persistence management for given object and add it to the cache.
        /// If the object of same Category and Id already exists in the cache, exception will be thrown.
        /// Note, that object will not go through usual Kernel pipeline, eg. IInitialize will not be called automatically.
        /// </summary>
        /// <param name="object"></param>
        /// <exception cref="PersistentObjectAlreadyTrackedException">
        /// Object of this type and id is already cached. Use Get method instead.
        /// </exception>
        /// <exception cref="UnsupportedObjectTypeException">Type does not implement IPersistentObject</exception>
        void StartTracking(object @object);

        void UnloadAndDeleteData<T>([NotNull] T @object) where T : class, IPersistentObject;
        void UnloadAndDeleteData([NotNull] object @object);
    }

    [Serializable]
    public class UnsupportedObjectTypeException : Exception
    {
        public UnsupportedObjectTypeException()
        {
        }

        public UnsupportedObjectTypeException(string message) : base(message)
        {
        }

        public UnsupportedObjectTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnsupportedObjectTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Interface that allows managing specific types of persistent objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPersistentObjectResolver<T> where T : class, IPersistentObject
    {
        /// <summary>
        /// Retrieves an object of type T with specified id.
        /// If the object does not exist, it is created from Kernel, based on binding strategy.
        /// </summary>
        /// <param name="persistentObjectId"></param>
        /// <returns></returns>
        T Get(string persistentObjectId);

        /// <summary>
        /// Retrieves an object of type T with default id (string.Empty).
        /// If the object does not exist, it is created from Kernel, based on binding strategy.
        /// </summary>
        /// <returns></returns>
        T GetDefault();

        /// <summary>
        /// Immediatelly stops tracking of the given object and removes it from cache.
        /// Data in the object is not saved. Future requests to save this object will do nothing.
        /// </summary>
        /// <param name="object"></param>
        void Unload(T @object);

        /// <summary>
        /// Attempts to enable persistence management for given object and add it to the cache.
        /// If the object of same Category and Id already exists in the cache, exception will be thrown.
        /// Note, that object will not go through usual Kernel pipeline, eg. IInitialize will not be called automatically.
        /// </summary>
        /// <param name="object"></param>
        /// <exception cref="PersistentObjectAlreadyTrackedException">
        /// Object of this type and id is already cached. Use Get method instead.
        /// </exception>
        void LoadAndStartTracking(T @object);

        void UnloadAndDeleteData(T @object);
    }
}
