using System;
using System.Runtime.CompilerServices;
using AldursLab.PersistentObjects.Persistence;
using AldursLab.PersistentObjects.Serialization;
using AldursLab.PersistentObjects.Tests.PersistentObjectSamples;
using AldursLab.Testing.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace AldursLab.PersistentObjects.Tests
{
    public class PersistenceManagerTests : TestWithFileSystem
    {
        [Test]
        public void LoadsAndStartsTracking()
        {
            var pm = CreatePersistenceManager();
            var fsc = new FakeSubsystemComponent();
            var fs = new FakeSubsystem(fsc);
            pm.LoadAndStartTracking(fs);

            AssertAllAreDefault(fs);
        }

        [Test]
        public void WhenNotPending_DoesNotSave()
        {
            {
                var pm = CreatePersistenceManager();
                var fsc = new FakeSubsystemComponent();
                var fs = new FakeSubsystem(fsc);

                pm.LoadAndStartTracking(fs);

                ChangeAll(fs);

                pm.SavePending();
            }

            {
                var pm = CreatePersistenceManager();
                var fsc = new FakeSubsystemComponent();
                var fs = new FakeSubsystem(fsc);

                pm.LoadAndStartTracking(fs);

                AssertAllAreDefault(fs);
            }
        }

        [Test]
        public void WhenPending_SavesAll()
        {
            {
                var pm = CreatePersistenceManager();
                var fsc = new FakeSubsystemComponent();
                var fs = new FakeSubsystem(fsc);

                pm.LoadAndStartTracking(fs);

                ChangeAll(fs);
                fs.PersistibleChangesPending = true;
                pm.SavePending();
            }

            {
                var pm = CreatePersistenceManager();
                var fsc = new FakeSubsystemComponent();
                var fs = new FakeSubsystem(fsc);

                pm.LoadAndStartTracking(fs);

                AssertAllChangedAfterReload(fs);
            }
        }

        [Test]
        public void PreservesExtensionData()
        {
            {
                var of = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(of);
                of.Data = "test";
                pm.SaveAll();
            }

            {
                var o = new ObjWithExtensionData("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o);
                pm.SaveAll();
            }

            {
                var of = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(of);

                Expect(of.Data, EqualTo("test"));
            }
        }

        [Test]
        public void WhenDeserializationError_Throws()
        {
            {
                var o = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o);
                o.Data = "test";
                pm.SaveAll();
            }

            {
                var oInt = new SimpleObjWithIntData("1");
                var pm = CreatePersistenceManager();
                Assert.Throws<JsonReaderException>(() => pm.LoadAndStartTracking(oInt));
            }
        }

        [Test]
        public void WhenForcedSave_Saves()
        {
            {
                var o = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o);
                o.Data = "test";
                pm.SaveAll();
            }

            {
                var o = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o);
                
                Expect(o.Data, EqualTo("test"));
            }
        }

        [Test]
        public void WhenSameCollectionAndKey_Throws()
        {
            {
                var o1 = new SimpleObj("1");
                var o2 = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o1);
                Assert.Throws<PersistentObjectAlreadyTracked>(() => pm.LoadAndStartTracking(o1));
                Assert.Throws<PersistentObjectAlreadyTracked>(() => pm.LoadAndStartTracking(o2));
            }
        }

        [Test]
        public void WhenSameCollectionAndKey_AfterStopTracking_DoesNotThrow()
        {
            {
                var o1 = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o1);
                pm.StopTracking(o1);
                var o2 = new SimpleObj("1");
                pm.LoadAndStartTracking(o2);
            }
        }

        [Test]
        public void WhenStopTracked_DoesNotTrack()
        {
            {
                var o1 = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o1);
                pm.StopTracking(o1);
                o1.Data = "changed";
                pm.SaveAll();
            }
            {
                var o = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o);
                Expect(o.Data, EqualTo(null));
            }
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WhenReferenceDead_DoesNotPreventGc()
        {
            var o = new SimpleObj("1");
            var wr = new WeakReference(o);
            var pm = CreatePersistenceManager();
            pm.LoadAndStartTracking(o);
            o = null;
            GC.Collect(3, GCCollectionMode.Forced, true);
            Expect(wr.IsAlive, False);
        }

        [Test]
        public void WhenKeyNull_Throws()
        {
            var o = new SimpleObj(null);
            var pm = CreatePersistenceManager();
            Assert.Throws<ArgumentException>(() => pm.LoadAndStartTracking(o));
        }

        [Test]
        public void WhenNoClassAttribute_Throws()
        {
            var o = new SimpleObjWithNoAttribute("1");
            var pm = CreatePersistenceManager();
            Assert.Throws<ArgumentException>(() => pm.LoadAndStartTracking(o));
        }

        [Test]
        public void WhenInterfaceNotImplemented_Throws()
        {
            var o = new SimpleObjWithNoInterface("1");
            var pm = CreatePersistenceManager();
            // error only possible in the "object" overload
            Assert.Throws<ArgumentException>(() => pm.LoadAndStartTracking((object)o));
        }

        class CustomErrorHandlingStrategy : TraceableErrorHandlingStrategy
        {
            public bool Invoked { get; private set; }

            public override void HandleErrorOnSerialize(object o, ErrorEventArgs args)
            {
                base.HandleErrorOnSerialize(o, args);
                args.ErrorContext.Handled = true;
                Invoked = true;
            }

            public override void HandleErrorOnDeserialize(object o, ErrorEventArgs args)
            {
                base.HandleErrorOnDeserialize(o, args);
                args.ErrorContext.Handled = true;
                Invoked = true;
            }
        }

        [Test]
        public void CustomErrorHandlerOverrides()
        {
            {
                var o = new SimpleObj("1");
                var pm = CreatePersistenceManager();
                pm.LoadAndStartTracking(o);
                o.Data = "test";
                pm.SaveAll();
            }

            {
                //bool invoked = false;
                var oInt = new SimpleObjWithIntData("1");
                var strategy = new CustomErrorHandlingStrategy();
                var ss = new JsonSerializationStrategy()
                {
                    ErrorStrategy = strategy
                };
                var pm = CreatePersistenceManager(ss);
                pm.LoadAndStartTracking(oInt);
                Expect(oInt.Data, EqualTo(SimpleObjWithIntData.DataDefaultValue));
                Expect(strategy.Invoked, True);
            }
        }

        [Test]
        public void GivenReturnExisting_WhenExisiting_ReturnsExisting()
        {
            var pm = CreatePersistenceManager();
            var fsc = new FakeSubsystemComponent();
            var fs = new FakeSubsystem(fsc);
            pm.LoadAndStartTracking(fs);

            var newFs = new FakeSubsystem(new FakeSubsystemComponent());

            var o = pm.LoadAndStartTracking(newFs, returnExistingInsteadOfException: true);
            Expect(o, EqualTo(fs));
        }

        PersistenceManager CreatePersistenceManager(JsonSerializationStrategy ss = null)
        {
            if (ss == null) ss = new JsonSerializationStrategy();
            var cfg = new PersistenceManagerConfig()
            {
                DataStoreDirectoryPath = TempDir.FullName
            };
            var m = new PersistenceManager(cfg, ss, new FlatFilesPersistenceStrategy(cfg));
            return m;
        }

        private void AssertAllAreDefault(FakeSubsystem fs)
        {
            Expect(fs.InnerDataContainer_PrivateDataFieldAcsr, EqualTo("privateDataField").IgnoreCase);
            Expect(fs.InnerDataContainer_PublicDataPropertyAcsr, EqualTo("PublicDataProperty").IgnoreCase);
            Expect(fs.InnerDataContainer_publicDataFieldAcsr, EqualTo("publicDataField").IgnoreCase);
            Expect(fs.OuterDataContainer.PrivateDataFieldAcsr, EqualTo("PrivateDataField").IgnoreCase);
            Expect(fs.OuterDataContainer.PublicDataProperty, EqualTo("PublicDataProperty").IgnoreCase);
            Expect(fs.OuterDataContainer.publicDataField, EqualTo("publicDataField").IgnoreCase);
            Expect(fs.SomeLooseStringAcsr, EqualTo("SomeLooseString").IgnoreCase);
            Expect(fs.SubsystemComponent.SomePrivateDataAcsr, EqualTo("SomePrivateData").IgnoreCase);
            Expect(fs.SubsystemComponent.SomePublicData, EqualTo("SomePublicData").IgnoreCase);
        }

        private void ChangeAll(FakeSubsystem fs)
        {
            fs.InnerDataContainer_PrivateDataFieldAcsr = "1";
            fs.InnerDataContainer_PublicDataPropertyAcsr = "2";
            fs.InnerDataContainer_publicDataFieldAcsr = "3";
            fs.OuterDataContainer.PrivateDataFieldAcsr = "4";
            fs.OuterDataContainer.PublicDataProperty = "5";
            fs.OuterDataContainer.publicDataField = "6";
            fs.SomeLooseStringAcsr = "7";
            fs.SubsystemComponent.SomePrivateDataAcsr = "-1";
            fs.SubsystemComponent.SomePublicData = "-2";
        }

        private void AssertAllChangedAfterReload(FakeSubsystem fs)
        {
            Expect(fs.InnerDataContainer_PrivateDataFieldAcsr, EqualTo("1").IgnoreCase);
            Expect(fs.InnerDataContainer_PublicDataPropertyAcsr, EqualTo("2").IgnoreCase);
            Expect(fs.InnerDataContainer_publicDataFieldAcsr, EqualTo("3").IgnoreCase);
            Expect(fs.OuterDataContainer.PrivateDataFieldAcsr, EqualTo("4").IgnoreCase);
            Expect(fs.OuterDataContainer.PublicDataProperty, EqualTo("5").IgnoreCase);
            Expect(fs.OuterDataContainer.publicDataField, EqualTo("6").IgnoreCase);
            Expect(fs.SomeLooseStringAcsr, EqualTo("7").IgnoreCase);
            // subsystem is excluded from serialization, should be default
            Expect(fs.SubsystemComponent.SomePrivateDataAcsr, EqualTo("SomePrivateData").IgnoreCase);
            Expect(fs.SubsystemComponent.SomePublicData, EqualTo("SomePublicData").IgnoreCase);
        }
    }
}
