using System;
using System.Diagnostics;
using System.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Synchronization
{
    class FileLockTests : AssertionHelper
    {
        protected DirectoryHandle TempDir;
        WeakReference<FileLock> fileLockWeakRef;
        FileLock fileLock;
        protected const string LockFileName = "test.lock";

        protected FileLock FileLock
        {
            get { return fileLock; }
            set
            {
                fileLock = value;
                fileLockWeakRef = new WeakReference<FileLock>(value);
            }
        }

        [SetUp]
        public void Setup()
        {
            TempDir = TempDirectoriesFactory.CreateEmpty();
        }

        [TearDown]
        public void Teardown()
        {
            FileLock fLock;
            fileLockWeakRef.TryGetTarget(out fLock);
            fLock?.Dispose();
            TempDir.Dispose();
        }

        class EnterTests : FileLockTests
        {
            [Test]
            public void LocksAndReleases()
            {
                var lockFilePath = Path.Combine(TempDir.AbsolutePath, LockFileName);
                File.Create(lockFilePath).Dispose();

                AssertNoLock(lockFilePath);

                FileLock = FileLock.Enter(lockFilePath);

                AssertLockFails(lockFilePath);

                FileLock.Dispose();

                AssertNoLock(lockFilePath);
            }

            [Test]
            public void WritesProcessInfo()
            {
                var lockFilePath = Path.Combine(TempDir.AbsolutePath, LockFileName);
                File.Create(lockFilePath).Dispose();

                FileLock = FileLock.Enter(lockFilePath);

                AssertHasProcessInfo(lockFilePath);
            }
        }

        class EnterWithCreateTests : FileLockTests
        {
            [Test]
            public void LocksAndReleases()
            {
                var lockFilePath = Path.Combine(TempDir.AbsolutePath, LockFileName);

                AssertNoFile(lockFilePath);

                FileLock = FileLock.EnterWithCreate(lockFilePath);

                AssertLockFails(lockFilePath);

                FileLock.Dispose();

                AssertNoLock(lockFilePath);
            }

            [Test]
            public void WritesProcessInfo()
            {
                var lockFilePath = Path.Combine(TempDir.AbsolutePath, LockFileName);

                FileLock = FileLock.EnterWithCreate(lockFilePath);
                
                AssertHasProcessInfo(lockFilePath);
            }

            [Test]
            public void ReleasesOnFinalization()
            {
                var lockFilePath = Path.Combine(TempDir.AbsolutePath, LockFileName);
                FileLock = FileLock.EnterWithCreate(lockFilePath);
                AssertLockFails(lockFilePath);
                FileLock = null;
                GC.Collect(3, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
                GC.Collect(3, GCCollectionMode.Forced, true);
                AssertNoLock(lockFilePath);
            }
        }

        static void AssertLockFails(string lockFilePath)
        {
            // trying file access
            Assert.Throws<IOException>(
                () =>
                {
                    File.Open(lockFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite).Dispose();
                });

            // trying to enter another lock
            Assert.Throws<LockFailedException>(
                () =>
                {
                    FileLock.Enter(lockFilePath).Dispose();
                });
        }

        static void AssertNoLock(string lockFilePath)
        {
            File.Open(lockFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite).Dispose();
            FileLock.Enter(lockFilePath).Dispose();
        }

        void AssertNoFile(string lockFilePath)
        {
            Expect(!File.Exists(lockFilePath));
        }

        protected static void AssertHasProcessInfo(string lockFilePath)
        {
            string contents;
            using (
                StreamReader sr =
                    new StreamReader(new FileStream(lockFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                contents = sr.ReadToEnd();
            }

            var currentProcess = Process.GetCurrentProcess();
            Assert.IsFalse(string.IsNullOrWhiteSpace(contents));
            Assert.IsTrue(contents.Contains(currentProcess.Id.ToString()));
            Assert.IsTrue(contents.Contains(currentProcess.ProcessName));
        }
    }
}
