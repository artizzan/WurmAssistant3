using System;

using SimpleInjector;
using SimpleInjector.Advanced;

namespace AldurSoft.Core.SimpleInjector
{
    public class SingletonLifestyleSelectionBehavior : ILifestyleSelectionBehavior
    {
        public Lifestyle SelectLifestyle(Type serviceType, Type implementationType)
        {
            return Lifestyle.Singleton;
        }
    }
}