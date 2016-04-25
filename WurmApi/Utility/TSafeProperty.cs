namespace AldursLab.WurmApi.Utility
{
    static class ThreadSafeProperty
    {
        public static ThreadSafeProperty<T> Create<T>(T initialValue)
        {
            return new ThreadSafeProperty<T>() { Value = initialValue };
        }
    }

    class ThreadSafeProperty<T>
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
