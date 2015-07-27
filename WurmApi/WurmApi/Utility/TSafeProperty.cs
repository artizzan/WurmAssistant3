using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Utility
{
    public static class ThreadSafeProperty
    {
        public static ThreadSafeProperty<T> Create<T>(T initialValue)
        {
            return new ThreadSafeProperty<T>() { Value = initialValue };
        }
    }

    public class ThreadSafeProperty<T>
    {
        readonly object locker = new object();
        T value;

        public T Value
        {
            get
            {
                lock (locker)
                {
                    return value;
                }
            }
            set
            {
                lock (locker)
                {
                    this.value = value;
                }
            }
        }
    }
}
