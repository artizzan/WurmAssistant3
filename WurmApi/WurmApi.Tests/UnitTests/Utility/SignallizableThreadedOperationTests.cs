using System;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Tests.Helpers;
using AldurSoft.WurmApi.Utility;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.UnitTests.Utility
{
    public class SignallizableThreadedOperationTests : AssertionHelper
    {
        [Test]
        public void ExecutesOnSignals()
        {
            int testVal = 0;

            var op = new RepeatableThreadedOperation(() => testVal++);
            Thread.Sleep(100);

            Expect(testVal, EqualTo(0));
            op.Signal();
            Thread.Sleep(100);

            Expect(testVal, EqualTo(1));
            op.Signal();
            Thread.Sleep(100);

            Expect(testVal, EqualTo(2));
        }

        [Test]
        public void SignalledWhileWorking_ExecutesTwice()
        {
            int testVal = 0;
            var op = new RepeatableThreadedOperation(() =>
            {
                Thread.Sleep(100);
                testVal++;
            });
            // signal to begin first operation
            op.Signal();

            // in middle of first operation, signal multiple times
            Thread.Sleep(50);
            op.Signal();
            op.Signal();
            op.Signal();
            op.Signal();

            // after enough time, assert that operation executed twice
            // once at the beginning, once for multiple further signals
            Thread.Sleep(300);
            Expect(testVal, EqualTo(2));
        }

        [Test]
        public async Task InitialOperationCanBeAwaited()
        {
            int testVal = 0;
            var op = new RepeatableThreadedOperation(() =>
            {
                testVal++;
            });
            Expect(testVal, EqualTo(0));
            Expect(op.OperationCompletedAtLeastOnce, False);

            op.Signal();
            await op.OperationCompletedAtLeastOnceAwaiter;

            Expect(testVal, EqualTo(1));
            Expect(op.OperationCompletedAtLeastOnce, True);

            op.Signal();
            await Task.Delay(100);

            Expect(testVal, EqualTo(2));
            Expect(op.OperationCompletedAtLeastOnce, True);

            op.Signal();
            await Task.Delay(100);

            Expect(testVal, EqualTo(3));
            Expect(op.OperationCompletedAtLeastOnce, True);
        }

        [Test]
        public async Task IfFirstOperationFaulted_DoesNotTriggerAwaiter()
        {
            var op = new RepeatableThreadedOperation(() =>
            {
                throw new DummyException();
            });
            op.Signal();

            bool anyAwaiterCompleted = false;
            Task t1 = new Task(async () =>
            {
                await op.OperationCompletedAtLeastOnceAwaiter;
                anyAwaiterCompleted = true;
            }, TaskCreationOptions.LongRunning);
            t1.Start();
            Task t2 = new Task(() =>
            {
                op.WaitSynchronouslyForInitialOperation();
                anyAwaiterCompleted = true;
            }, TaskCreationOptions.LongRunning);
            t2.Start();

            await Task.Delay(200);

            Expect(op.OperationCompletedAtLeastOnce, False);
            Expect(anyAwaiterCompleted, False);
        }

        [Test]
        public async Task FaultedDoesNotCrashOnFinalization()
        {
            var op = new RepeatableThreadedOperation(() =>
            {
                throw new DummyException();
            });
            op.Signal();

            await Task.Delay(200);

            bool wasUnobservedException = false;

            TaskScheduler.UnobservedTaskException +=
                (s, args) => wasUnobservedException = true;

            op.Dispose();

            WeakReference wr = new WeakReference(op);
            op = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Expect(wr.IsAlive, False);
            Expect(wasUnobservedException, False);
        }

        [Test]
        public void Disposes()
        {
            var op = new RepeatableThreadedOperation(() =>
            {
            });
            op.Signal();
            Thread.Sleep(100);
            op.Dispose();
        }

        [Test]
        public void Disposes_WhenFaulted()
        {
            var op = new RepeatableThreadedOperation(() =>
            {
                throw new DummyException();
            });
            op.Signal();
            Thread.Sleep(100);
            op.Dispose();
        }

        // disabled due to issue in DelayedSignal method
        //[Test]
        //public void DelayedSignals()
        //{
        //    int value = 0;
        //    var op = new RepeatableThreadedOperation(() =>
        //    {
        //        value++;
        //    });
        //    op.DelayedSignal(TimeSpan.FromMilliseconds(50));
        //    Thread.Sleep(25);
        //    Expect(value, EqualTo(0));
        //    Thread.Sleep(50);
        //    Expect(value, EqualTo(1));
        //}
    }
}
