using System;
using System.Linq;
using System.Reflection;
using AldursLab.WurmAssistant3.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Areas.TestArea1;
using AldursLab.WurmAssistant3.Areas.TestArea1.Contracts;
using AldursLab.WurmAssistant3.Areas.TestArea1.Contracts.Nested;
using AldursLab.WurmAssistant3.Areas.TestArea1.CustomViews;
using AldursLab.WurmAssistant3.Areas.TestArea1.CustomViews.Nested;
using AldursLab.WurmAssistant3.Areas.TestArea1.OutsideConvention;
using AldursLab.WurmAssistant3.Areas.TestArea1.Singletons;
using AldursLab.WurmAssistant3.Areas.TestArea1.Singletons.Nested;
using AldursLab.WurmAssistant3.Areas.TestArea1.Transients;
using AldursLab.WurmAssistant3.Areas.TestArea1.Transients.Nested;
using AldursLab.WurmAssistant3.Areas.TestArea1.ViewModels;
using AldursLab.WurmAssistant3.Areas.TestArea1.ViewModels.Nested;
using AldursLab.WurmAssistant3.Systems.ConventionBinding;
using FluentAssertions;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using NUnit.Framework;

namespace AldursLab.WurmAssistant3.Tests.Systems.ConventionBinding
{
    public class ConventionBindingManagerTests
    {
        IKernel kernel;
        ConventionBindingManager manager;

        [SetUp]
        public void Setup()
        {
            kernel = new StandardKernel();
            kernel.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();
            manager = new ConventionBindingManager(kernel, new string[0], new[]
            {
                typeof(Areas.TestArea1.Anchor).Assembly,
                typeof(Areas.TestArea1.Contracts.Anchor).Assembly
            });
            manager.BindAreasByConvention();
        }

        [Test]
        public void TypesInNamespacesShouldBeBound()
        {
            {
                var singleton = kernel.Get<SampleSingleton>();
                singleton.Should().NotBeNull();

                var transient = kernel.Get<SampleTransient>();
                transient.Should().NotBeNull();

                var viewmodel = kernel.Get<SampleViewModel>();
                viewmodel.Should().NotBeNull();

                var customview = kernel.Get<SampleCustomView>();
                customview.Should().NotBeNull();
            }
            {
                var singleton = kernel.Get<ISampleSingleton>();
                singleton.Should().NotBeNull();

                var transient = kernel.Get<ISampleTransient>();
                transient.Should().NotBeNull();

                var viewmodel = kernel.Get<ISampleViewModel>();
                viewmodel.Should().NotBeNull();

                var customview = kernel.Get<ISampleCustomView>();
                customview.Should().NotBeNull();
            }
            {
                var factory = kernel.Get<ISampleViewModelFactory>();
                factory.Should().NotBeNull();
            }
            {
                var feature = kernel.GetAll<IFeature>();
                feature.Should().HaveCount(2);
            }
        }

        [Test]
        public void TypesInNestedNamespacesShouldBeBound()
        {
            {
                var singleton = kernel.Get<SampleNestedSingleton>();
                singleton.Should().NotBeNull();

                var transient = kernel.Get<SampleNestedTransient>();
                transient.Should().NotBeNull();

                var viewmodel = kernel.Get<SampleNestedViewModel>();
                viewmodel.Should().NotBeNull();

                var customview = kernel.Get<SampleNestedCustomView>();
                customview.Should().NotBeNull();
            }
            {
                var singleton = kernel.Get<ISampleNestedSingleton>();
                singleton.Should().NotBeNull();

                var transient = kernel.Get<ISampleNestedTransient>();
                transient.Should().NotBeNull();

                var viewmodel = kernel.Get<ISampleNestedViewModel>();
                viewmodel.Should().NotBeNull();

                var customview = kernel.Get<ISampleNestedCustomView>();
                customview.Should().NotBeNull();
            }
            {
                var factory = kernel.Get<ISampleNestedViewModelFactory>();
                factory.Should().NotBeNull();
            }
        }

        [Test]
        public void ConfigHasBeenRun()
        {
            AreaConfiguration.HasBeenRun.Should().BeTrue();
        }

        [Test]
        public void SingletonsShouldBeSingletons()
        {
            {
                var singleton1 = kernel.Get<SampleSingleton>();
                var singleton2 = kernel.Get<SampleSingleton>();
                var singleton3 = kernel.Get<ISampleSingleton>();
                singleton1.Should().BeSameAs(singleton2).And.BeSameAs(singleton3);
            }
            {
                var singleton1 = kernel.Get<SampleNestedSingleton>();
                var singleton2 = kernel.Get<SampleNestedSingleton>();
                var singleton3 = kernel.Get<ISampleNestedSingleton>();
                singleton1.Should().BeSameAs(singleton2).And.BeSameAs(singleton3);
            }
            {
                var sampleFeature = kernel.Get<SampleFeature>();
                var sampleFeatureBis = kernel.GetAll<IFeature>().Single(feature => feature.Name == "SampleFeature");
                sampleFeature.Should().BeSameAs(sampleFeatureBis);

            }
            {
                var sampleFeature2 = kernel.Get<SampleFeature2>();
                var sampleFeature2Bis = kernel.GetAll<IFeature>().Single(feature => feature.Name == "SampleFeature2");
                sampleFeature2.Should().BeSameAs(sampleFeature2Bis);
            }
        }

        [Test]
        public void AutoFactoriesShouldProduceObjects()
        {
            {
                var product = kernel.Get<ISampleViewModelFactory>().CreateSampleViewModel();
                product.Should().NotBeNull();
            }
            {
                var product = kernel.Get<ISampleNestedViewModelFactory>().CreateSampleNestedViewModel();
                product.Should().NotBeNull();
            }
        }

        [Test]
        public void TypesOutsideConventionShouldNotBeBound()
        {
            kernel.Invoking(k => k.Get<SomeTypeOutsideConvention>()).ShouldThrow<ActivationException>();
        }

        [TearDown]
        public void Teardown()
        {
            kernel?.Dispose();
        }
    }
}
