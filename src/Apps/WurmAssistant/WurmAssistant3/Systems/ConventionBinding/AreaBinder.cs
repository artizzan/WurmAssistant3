using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Systems.ConventionBinding.Exceptions;
using AldursLab.WurmAssistant3.Utils.IoC;
using JetBrains.Annotations;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Factory;
using Ninject.Infrastructure.Language;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    class AreaBinder
    {
        [CanBeNull]
        readonly Type areaConfigurationType;

        readonly IKernel kernel;

        public AreaReflectionInfo AreaReflectionInfo { get; }
        public AreaTypesManager AreaTypesManager { get; }

        public AreaBinder(
            [NotNull] AreaReflectionInfo areaReflectionInfo, 
            [CanBeNull] Type areaConfigurationType,
            [NotNull] IKernel kernel)
        {
            if (areaReflectionInfo == null) throw new ArgumentNullException(nameof(areaReflectionInfo));
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            this.AreaReflectionInfo = areaReflectionInfo;
            this.areaConfigurationType = areaConfigurationType;
            this.kernel = kernel;
            this.AreaTypesManager = new AreaTypesManager(areaReflectionInfo);
        }

        public IAreaConfiguration TryActivateCustomConfig()
        {
            if (areaConfigurationType != null)
            {
                try
                {
                    return (IAreaConfiguration)Activator.CreateInstance(areaConfigurationType);
                }
                catch (Exception exception)
                {
                    throw new InvalidAreaConfigTypeException(areaConfigurationType, exception);
                }
            }

            return null;
        }

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

            var areaScoped = AreaTypesManager.GetAllAreaScoped();
            foreach (var areaScope in areaScoped)
            {
                var services = GetAllImplementedServices(areaScope);
                var binding = kernel.Bind(services.ToArray()).To(areaScope).InSingletonScope().WithOptionalAreaScopeArgument();
                binding.BindingConfiguration.ScopeCallback = context =>
                {
                    var type = context.Request.ParentContext?.Request.Service;
                    if (type == null)
                    {
                        throw new NullReferenceException(
                            $"Resolved AreaScoped service {context.Binding.Service.FullName} is not within any ParentContext");
                    }
                    var scopeName = AreaReflectionInfo.ParseAreaNameFromType(type);
                    return scopeName;
                };
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
                        $"Type {autoFactory.FullName} must be an interface to be properly proxied into an automatic Ninject factory.");
                }
                kernel.Bind(autoFactory).ToFactory();
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
    }
}