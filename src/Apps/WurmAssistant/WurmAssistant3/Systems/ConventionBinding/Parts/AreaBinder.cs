using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using JetBrains.Annotations;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Infrastructure.Language;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding.Parts
{
    class AreaBinder
    {
        AreaTypesManager AreaTypesManager { get; }

        [CanBeNull]
        readonly AreaTypeReflectionInfo areaConfigurationType;

        readonly IKernel kernel;

        public AreaBinder(
            [NotNull] string areaName,
            [NotNull] AreaTypeLibrary areaTypeLibrary,
            [NotNull] IKernel kernel)
        {
            if (areaName == null) throw new ArgumentNullException(nameof(areaName));
            if (areaTypeLibrary == null) throw new ArgumentNullException(nameof(areaTypeLibrary));
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            AreaName = areaName;

            this.kernel = kernel;
            this.AreaTypesManager = new AreaTypesManager(areaName, areaTypeLibrary);
            this.areaConfigurationType = this.AreaTypesManager.TryGetAreaConfigurationType();
        }

        public string AreaName { get; }

        public void BindByConvention()
        {
            var singletons = AreaTypesManager.GetAllSingletons();
            foreach (var singleton in singletons)
            {
                var services = GetAllImplementedServices(singleton);
                kernel.Bind(services.ToArray()).To(singleton).InSingletonScope();
            }

            var transients = AreaTypesManager.GetAllTransients();
            foreach (var transient in transients)
            {
                var services = GetAllImplementedServices(transient);
                kernel.Bind(services.ToArray()).To(transient).InTransientScope();
            }

            var viewModels = AreaTypesManager.GetAllViewModels();
            foreach (var viewModel in viewModels)
            {
                var services = GetAllImplementedServices(viewModel);
                kernel.Bind(services.ToArray()).To(viewModel).InTransientScope();
            }

            var customViews = AreaTypesManager.GetAllCustomViews();
            foreach (var customView in customViews)
            {
                var services = GetAllImplementedServices(customView);
                kernel.Bind(services.ToArray()).To(customView).InTransientScope();
            }

            var autoFactories = AreaTypesManager.GetAllAutoFactories();
            foreach (var autoFactory in autoFactories)
            {
                if (!autoFactory.IsInterface)
                {
                    throw new InvalidOperationException(
                        $"Type {autoFactory.FullName} must be an interface in order to be properly proxied into an automatic Ninject factory.");
                }
                kernel.Bind(autoFactory).ToFactory(autoFactory);
            }
        }
            
        IEnumerable<Type> GetAllImplementedServices(Type type)
        {
            List<Type> services = new List<Type> { type };
            var interfaces = type.GetInterfaces();
            if (interfaces.Any())
            {
                services.AddRange(interfaces);
            }
            var baseAbstractTypes = type.GetAllBaseTypes().Where(type1 => type1.IsAbstract).ToArray();
            if (baseAbstractTypes.Any())
            {
                services.AddRange(baseAbstractTypes);
            }

            return services;
        }

        public void RunCustomConfigIfDefined()
        {
            if (areaConfigurationType != null)
            {
                var config = areaConfigurationType.ActivateAsAreaConfiguration();
                config.Configure(kernel);
            }
        }
    }
}