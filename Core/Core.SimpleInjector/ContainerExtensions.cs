using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SimpleInjector;

namespace AldurSoft.Core.SimpleInjector
{
    /// <summary>
    /// Extensions for SimpleInjector container.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Registers interfaces to their implementations.
        /// Interfaces must be implemented only once.
        /// Only specified namespace is included (subnamespaces are ignored).
        /// Interfaces are registered as transient.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="namespaceFilter">The namespace filter.</param>
        /// <exception cref="ArgumentNullException">namespaceFilter</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AutoRegisterInterfacesToImplementations(this Container container, Assembly assembly,
            string namespaceFilter)
        {
            if (namespaceFilter == null)
                throw new ArgumentNullException("namespaceFilter");

            var registrations =
                from type in assembly.GetExportedTypes()
                where type.Namespace == namespaceFilter
                where !type.IsInterface
                where type.GetInterfaces().Any()
                select new
                {
                    Services = type.GetInterfaces(),
                    Implementation = type
                };

            HashSet<Type> registeredServices = new HashSet<Type>();

            foreach (var reg in registrations)
            {
                foreach (var service in reg.Services)
                {
                    if (registeredServices.Contains(service))
                    {
                        throw new InvalidOperationException(
                            string.Format("Service {0} has already been implemented. Services cannot have multiple implementations when auto-registering", service.FullName));
                    }
                    container.Register(service, reg.Implementation, Lifestyle.Transient);
                    registeredServices.Add(service);
                }
            }
        }
    }
}
