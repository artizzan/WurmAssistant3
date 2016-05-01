using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Singletons
{
    /// <summary>
    /// Encapsulates instantiation and publication of the singleton instance.
    /// Default InstanceProvider requires parameterless constructor on the type and can be replaced.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class
    {
        public static ISingletonInstanceProvider<T> InstanceProvider { get; set; }

        static T _instance;

        // intended
        // ReSharper disable once StaticMemberInGenericType
        static readonly object Locker = new object();

        static Singleton()
        {
            InstanceProvider = new DefaultSingletonInstanceProvider<T>();
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            _instance = InstanceProvider.CreateInstance();
                        }
                    }
                }
                return _instance;
            }
        }
    }

    public interface ISingletonInstanceProvider<out T> where T : class
    {
        T CreateInstance();
    }

    public class DefaultSingletonInstanceProvider<T> : ISingletonInstanceProvider<T> where T : class
    {
        public T CreateInstance()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
