using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.IoC;
using JetBrains.Annotations;
using Ninject.Parameters;

namespace AldursLab.WurmAssistant3.Areas.Persistence.Components
{
    public class PersistentObjectResolver : IPersistentObjectResolver
    {
        // assuming that Resolvers<T> are singletons,
        // not required, but resolve of new instance can be costly in Ninject

        // opt: reflection magic may be optimized, do only if necessary

        readonly ISuperFactory kernel;

        public PersistentObjectResolver([NotNull] ISuperFactory kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            this.kernel = kernel;
        }

        public T Get<T>(string persistentObjectId) where T : class, IPersistentObject
        {
            return GetInternal<T>(persistentObjectId);
        }

        public object Get([NotNull] string persistentObjectId, [NotNull] Type objectType)
        {
            if (persistentObjectId == null) throw new ArgumentNullException("persistentObjectId");
            if (objectType == null) throw new ArgumentNullException("objectType");
            ThrowIfNotSupported(objectType);
            MethodInfo genericMethod = GetLocalGenericMethod("GetInternal", objectType);
            return InvokeThis(genericMethod, persistentObjectId);
        }

        public T GetInternal<T>(string persistentObjectId) where T : class, IPersistentObject
        {
            var resolver = GetResolver<T>();
            return resolver.Get(persistentObjectId);
        }


        public T GetDefault<T>() where T : class, IPersistentObject
        {
            return GetDefaultInternal<T>();
        }

        public object GetDefault([NotNull] Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            ThrowIfNotSupported(objectType);
            MethodInfo genericMethod = GetLocalGenericMethod("GetDefaultInternal", objectType);
            return InvokeThis(genericMethod);
        }

        public T GetDefaultInternal<T>() where T : class, IPersistentObject
        {
            var resolver = GetResolver<T>();
            return resolver.GetDefault();
        }

        public void Unload<T>([NotNull] T @object) where T : class, IPersistentObject
        {
            UnloadInternal(@object, false);
        }

        public void UnloadAndDeleteData<T>(T @object) where T : class, IPersistentObject
        {
            UnloadInternal(@object, true);
        }

        public void Unload([NotNull] object @object)
        {
            if (@object == null) throw new ArgumentNullException("object");
            var runtimeType = @object.GetType();
            ThrowIfNotSupported(runtimeType);
            MethodInfo genericMethod = GetLocalGenericMethod("UnloadInternal", runtimeType);
            InvokeThis(genericMethod, @object, false);
        }

        public void UnloadAndDeleteData(object @object)
        {
            if (@object == null)
                throw new ArgumentNullException("object");
            var runtimeType = @object.GetType();
            ThrowIfNotSupported(runtimeType);
            MethodInfo genericMethod = GetLocalGenericMethod("UnloadInternal", runtimeType);
            InvokeThis(genericMethod, @object, true);
        }

        public void UnloadInternal<T>([NotNull] T @object, bool deleteData) where T : class, IPersistentObject
        {
            if (@object == null) throw new ArgumentNullException("object");
            var resolver = GetResolver<T>();
            if (deleteData) resolver.UnloadAndDeleteData(@object);
            else resolver.Unload(@object);
        }

        public void StartTracking<T>([NotNull] T @object) where T : class, IPersistentObject
        {
            StartTrackingInternal(@object);
        }

        public void StartTracking([NotNull] object @object)
        {
            if (@object == null) throw new ArgumentNullException("object");
            var runtimeType = @object.GetType();
            ThrowIfNotSupported(runtimeType);
            MethodInfo genericMethod = GetLocalGenericMethod("StartTrackingInternal", runtimeType);
            InvokeThis(genericMethod, @object);
        }

        public void StartTrackingInternal<T>([NotNull] T @object) where T : class, IPersistentObject
        {
            if (@object == null) throw new ArgumentNullException("object");
            var resolver = GetResolver<T>();
            resolver.LoadAndStartTracking(@object);
        }


        IPersistentObjectResolver<T> GetResolver<T>() where T : class, IPersistentObject
        {
            return kernel.Get<IPersistentObjectResolver<T>>();
        }

        void ThrowIfNotSupported(Type objectType)
        {
            if ((objectType.GetInterface(typeof(IPersistentObject).FullName) == null))
            {
                throw new UnsupportedObjectTypeException(string.Format("Object type {0} must implement {1}",
                    objectType,
                    typeof(IPersistentObject).FullName));
            }
        }

        MethodInfo GetLocalGenericMethod(string name, Type objectType)
        {
            MethodInfo method = typeof(PersistentObjectResolver).GetMethod(name);
            MethodInfo genericMethod = method.MakeGenericMethod(objectType);
            return genericMethod;
        }

        object InvokeThis(MethodInfo method, params object[] parameterValues)
        {
            try
            {
                return method.Invoke(this, parameterValues);
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                return null;
            }
        }
    }

    public class PersistentObjectResolver<T> : IPersistentObjectResolver<T> where T : class, IPersistentObject
    {
        // note: caching removed, because actual object type may be different from generic type, causing cache misses

        readonly ISuperFactory kernel;
        readonly PersistenceManager persistenceManager;

        public PersistentObjectResolver([NotNull] ISuperFactory kernel, [NotNull] PersistenceManager persistenceManager)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (persistenceManager == null) throw new ArgumentNullException("persistenceManager");
            this.kernel = kernel;
            this.persistenceManager = persistenceManager;
        }

        public T Get(string persistentObjectId)
        {
            var persistentObjectIdParam = new ConstructorArgument("persistentObjectId", persistentObjectId);
            var obj = kernel.Get<T>(persistentObjectIdParam);
            return obj;
        }

        public T GetDefault()
        {
            return Get("");
        }

        public void Unload(T @object)
        {
            persistenceManager.StopTracking(@object);
        }

        public void UnloadAndDeleteData(T @object)
        {
            persistenceManager.StopTracking(@object, deleteData:true);
        }

        public void LoadAndStartTracking(T @object)
        {
            persistenceManager.LoadAndStartTracking(@object);
        }
    }
}