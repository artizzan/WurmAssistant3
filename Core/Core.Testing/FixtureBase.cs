using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AldurSoft.Core.Testing.Automocking;

using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace AldurSoft.Core.Testing
{
    public abstract class FixtureBase : AssertionHelper
    {
        private readonly List<TestPak> testPaks = new List<TestPak>(); 

        public FixtureBase()
        {
            MockableClock = TestEnv.MockableClock;

            BinDirectory = new DirectoryInfo(TestEnv.BinDirectory);

            Func<ISpecimenBuilder, bool> concreteFilter = sb => !(sb is MethodInvoker);
 
            var relays = new FilteringRelays(concreteFilter);

            Automocker = new Fixture(relays).Customize(
                new AutoMoqCustomization(
                    new MockRelay(
                        new TrueRequestSpecification())));
        }

        public DirectoryInfo BinDirectory { get; private set; }
        public IFixture Automocker { get; private set; }
        public MockableClock MockableClock { get; private set; }

        public TestPak CreateTestPakEmptyDir()
        {
            var pak = new EmptyDirTestPak();
            testPaks.Add(pak);
            return pak;
        }

        public TestPak CreateTestPakFromDir(string sourceDirFullPath)
        {
            var pak = new DirCopyTestPak(sourceDirFullPath);
            testPaks.Add(pak);
            return pak;
        }

        public TestPak CreateTestPakFromZip(string sourceZipFullPath)
        {
            var pak = new UnzipFileTestPak(sourceZipFullPath);
            testPaks.Add(pak);
            return pak;
        }

        [TearDown]
        public virtual void Teardown()
        {
            if (testPaks.Any())
            {
                ExecuteAll(testPaks.Select(pak => new Action(pak.Dispose)).ToArray());
            }
        }

        public void ExecuteAll(params Action[] actions)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException("Encountered errors during execution of action chain. See inner exceptions for details.", exceptions);
            }
        }

        public void DoNothing() { }
        public void DoNothing<T1>(T1 arg1) { }
        public void DoNothing<T1, T2>(T1 arg1, T2 arg2) { }
    }

    public static class TestingExtensions
    {
        public static Mock<T> GetMock<T>(this T obj) where T : class
        {
            return Mock.Get(obj);
        } 
    }
}