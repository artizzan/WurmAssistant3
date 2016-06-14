using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Areas.Persistence;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using JetBrains.Annotations;
using Ninject;
using Ninject.Extensions.Factory;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public class ConventionBindingManager
    {
        readonly IKernel kernel;
        readonly IReadOnlyList<Assembly> assemblies;

        public ConventionBindingManager([NotNull] IKernel kernel,
            [NotNull] IReadOnlyList<Assembly> assemblies)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            this.kernel = kernel;
            this.assemblies = assemblies;
        }

        public void BindAssembliesByConvention()
        {
            List<WaTypeInfo> allTypes = new List<WaTypeInfo>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                                        .Where(type => type.Namespace != null)
                                        .Select(type => new WaTypeInfo(type))
                                        .ToArray();
                    allTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException exception)
                {
                    throw new KernelBindException($"Unable to load types from assembly {assembly.FullName}. " +
                                                  $"If this is a plugin, consider updating or removing it." +
                                                  $"Loader errors: " +
                                                  string.Join(", ", exception.LoaderExceptions.AsEnumerable()), exception);
                }
            }

            var bindableTypes = allTypes.Where(info => info.IsBindable).ToArray();
            foreach (var typeInfo in bindableTypes)
            {
                if (typeInfo.IsBindable)
                {
                    var contracts = typeInfo.GetAllImplementedServices();
                    if (typeInfo.BindableAsTransient)
                    {
                        kernel.Bind(contracts.ToArray())
                                .To(typeInfo.Type)
                                .InTransientScope()
                                .Named(typeInfo.Type.FullName);
                    }
                    else if (typeInfo.BindableAsSingleton)
                    {
                        kernel.Bind(contracts.ToArray())
                                .To(typeInfo.Type)
                                .InSingletonScope()
                                .Named(typeInfo.Type.FullName);
                    }
                    else if (typeInfo.BindableAsFactoryProxy)
                    {
                        kernel.Bind(typeInfo.Type).ToFactory(typeInfo.Type);
                    }
                    else
                    {
                        throw new KernelBindException(typeInfo);
                    }
                }
            }

            var areaConfigs = allTypes.Where(info => info.IsAreaConfigurationType);

            foreach (var areaConfig in areaConfigs)
            {
                var config = areaConfig.ActivateAsAreaConfiguration();
                config.Configure(kernel);
            }
        }
    }
}