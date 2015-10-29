using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Testing;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Components;
using AldursLab.WurmAssistant3.Core.Areas.Persistence.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.SoundEngine.Modules;
using AldursLab.WurmAssistant3.Core.IoC;
using Ninject;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace AldursLab.WurmAssistant3.Tests.Unit.Areas.Persistence
{
    class PersistentObjectResolverTests : UnitTest<PersistentObjectResolver>
    {
        const string PersistentObjectId = "Sample";
        readonly Sample sample = new Sample(PersistentObjectId);
        readonly Sample defaultSample = new Sample(string.Empty);

        [SetUp]
        public void Setup()
        {
            var genericResolver = Mock.Create<IPersistentObjectResolver<Sample>>();
            genericResolver.Arrange(resolver => resolver.Get(PersistentObjectId)).Returns(sample);
            genericResolver.Arrange(resolver => resolver.GetDefault()).Returns(defaultSample);
            Kernel.Bind<IPersistentObjectResolver<Sample>>().ToConstant(genericResolver);

            var superFactory = Mock.Create<ISuperFactory>();
            superFactory.Arrange(x => x.Get<IPersistentObjectResolver<Sample>>()).Returns(genericResolver);
            Kernel.Bind<ISuperFactory>().ToConstant(superFactory);
        }

        [Test]
        public void GetsGeneric()
        {
            var obj = Service.Get<Sample>(PersistentObjectId);
            Expect(obj, EqualTo(sample));
        }

        [Test]
        public void GetsNonGeneric()
        {
            var obj = Service.Get(PersistentObjectId, typeof(Sample));
            Expect(obj, EqualTo(sample));
        }

        [Test]
        public void GetsDefaultGeneric()
        {
            var obj = Service.GetDefault<Sample>();
            Expect(obj, EqualTo(defaultSample));
        }

        [Test]
        public void GetsDefaultNonGeneric()
        {
            var obj = Service.GetDefault(typeof(Sample));
            Expect(obj, EqualTo(defaultSample));
        }

        [Test]
        public void UnloadGeneric()
        {
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.Unload(sample)).OccursOnce();
            Service.Unload<Sample>(sample);
            Kernel.AssertAll();
        }

        [Test]
        public void UnloadNonGeneric()
        {
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.Unload(sample)).OccursOnce();
            Service.Unload((object)sample);
            Kernel.AssertAll();
        }

        [Test]
        public void StartTrackingGeneric()
        {
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.LoadAndStartTracking(sample)).OccursOnce();
            Service.StartTracking<Sample>(sample);
            Kernel.AssertAll();
        }

        [Test]
        public void StartTrackingNonGeneric()
        {
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.LoadAndStartTracking(sample)).OccursOnce();
            Service.StartTracking((object)sample);
            Kernel.AssertAll();
        }

        [Test]
        public void GivenAnyNonGenericMethod_WhenException_Unwraps()
        {
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.Get(PersistentObjectId)).Throws<TestfulException>();
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.GetDefault()).Throws<TestfulException>();
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.Unload(sample)).Throws<TestfulException>();
            Kernel.Arrange<IPersistentObjectResolver<Sample>>(resolver => resolver.LoadAndStartTracking(sample)).Throws<TestfulException>();
            Assert.Throws<TestfulException>(() => Service.Get(PersistentObjectId, typeof (Sample)));
            Assert.Throws<TestfulException>(() => Service.GetDefault(typeof(Sample)));
            Assert.Throws<TestfulException>(() => Service.Unload(sample));
            Assert.Throws<TestfulException>(() => Service.StartTracking(sample));
        }
    }
}
