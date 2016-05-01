using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Singletons;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Singletons
{
    [TestFixture]
    public class SingletonTests : AssertionHelper
    {
        [Test]
        public void CreatesOnlySingleSingleton()
        {
            var system = TestfulSingleton1.Instance;
            var secondsystem = TestfulSingleton1.Instance;
            Expect(ReferenceEquals(system, secondsystem));
        }

        [Test]
        public void UsesCustomProvider()
        {
            var provider = new CustomInstanceProvider2();
            TestfulSingleton2.InstanceProvider = provider;

            var system = TestfulSingleton2.Instance;

            Expect(provider.AccessCount, EqualTo(1));
        }

        class CustomInstanceProvider2 : ISingletonInstanceProvider<TestfulSingleton2>
        {
            public int AccessCount { get; private set; }

            public TestfulSingleton2 CreateInstance()
            {
                AccessCount++;
                return new TestfulSingleton2();
            }
        }

        [Test]
        public void WhenConcurrentAccess_CreatesOnlySingleSingleton()
        {
            TestfulSingleton3.InstanceProvider = new CustomInstanceProvider3();

            AutoResetEvent wh = new AutoResetEvent(false);
            ConcurrentStack<TestfulSingleton3> singletons = new ConcurrentStack<TestfulSingleton3>();

            // synchronize access to Instance across N threads
            // we are hoping at least one of these threads runs on a different CPU
            const int threadCount = 20;
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(() =>
                {
                    wh.WaitOne(5000);
                    var instance = TestfulSingleton3.Instance;
                    singletons.Push(instance);
                });
                threads[i] = thread;
                thread.Start();
            }

            // let the threads settle at WaitOne
            Thread.Sleep(10);

            // release the handle
            wh.Set();

            // wait for threads to finish
            foreach (var thread in threads)
            {
                var joined = thread.Join(5000);
                if (!joined) Assert.Fail();
            }

            // every instance should be equal to every other instance
            TestfulSingleton3 lastSingleton = null;

            foreach (var s in singletons)
            {
                if (lastSingleton != null)
                {
                    Expect(ReferenceEquals(lastSingleton, s));
                }
                lastSingleton = s;
            }
        }

        class CustomInstanceProvider3 : ISingletonInstanceProvider<TestfulSingleton3>
        {
            public TestfulSingleton3 CreateInstance()
            {
                Thread.Sleep(10);
                return new TestfulSingleton3();
            }
        }
    }

    class TestfulSingleton1 : Singleton<TestfulSingleton1>
    {
    }

    class TestfulSingleton2 : Singleton<TestfulSingleton2>
    {
    }

    class TestfulSingleton3 : Singleton<TestfulSingleton3>
    {
    }
}