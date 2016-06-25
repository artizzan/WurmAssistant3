using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using JetBrains.Annotations;
using Ninject.Infrastructure.Language;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public class WaTypeInfo
    {
        public Type Type { get; private set; }
        public KernelBindAttribute KernelBindAttribute { get; }

        public WaTypeInfo([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Type = type;

            KernelBindAttribute = (KernelBindAttribute) type.GetCustomAttribute(typeof(KernelBindAttribute));
        }

        public bool IsBindable => KernelBindAttribute != null;

        public bool BindableAsTransient
            => IsBindable
               && Type.IsClass
               && !Type.IsAbstract
               && KernelBindAttribute.BindingHint == BindingHint.Transient;

        public bool BindableAsSingleton
            => IsBindable
               && Type.IsClass
               && !Type.IsAbstract
               && KernelBindAttribute.BindingHint == BindingHint.Singleton;

        public bool BindableAsFactoryProxy
            => IsBindable
               && Type.IsInterface
               && KernelBindAttribute.BindingHint == BindingHint.FactoryProxy;

        public bool IsAreaConfigurationType
        {
            get
            {
                var validNamespace = Type.Namespace != null;
                var notTheAbstractClass = Type != typeof(AreaConfig) && !Type.IsAbstract;
                var isAreaConfig = typeof(AreaConfig).IsAssignableFrom(Type);
                return validNamespace && notTheAbstractClass && isAreaConfig;
            }
        }

        public AreaConfig ActivateAsAreaConfiguration()
        {
            try
            {
                return (AreaConfig)Activator.CreateInstance(Type);
            }
            catch (Exception exception)
            {
                throw new InvalidAreaConfigTypeException(this, exception);
            }
        }

        public IEnumerable<Type> GetAllImplementedServices()
        {
            List<Type> services = new List<Type> { Type };
            var interfaces = Type.GetInterfaces();
            if (interfaces.Any())
            {
                services.AddRange(interfaces);
            }
            var baseAbstractTypes = Type.GetAllBaseTypes().Where(type1 => type1.IsAbstract).ToArray();
            if (baseAbstractTypes.Any())
            {
                services.AddRange(baseAbstractTypes);
            }

            return services;
        }
    }
}