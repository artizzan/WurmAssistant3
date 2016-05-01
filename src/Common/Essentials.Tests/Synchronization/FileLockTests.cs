using System;
using System.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Synchronization
{
    class FileLockTests : AssertionHelper
    {
        DirectoryHandle tempDir;
        const string LockFileName = "test.lock";

        [SetUp]
        public void Setup()
        {
            tempDir = TempDirectoriesFactory.CreateEmpty();
        }

        [TearDown]
        public void Teardown()
        {
            tempDir.Dispose();
        }
        
        [Test]
        public void Enter_LocksAndReleases()
        {
            var lockFilePath = Path.Combine(tempDir.AbsolutePath, LockFileName);
            File.Create(lockFilePath).Dispose();

            AssertNoLock(lockFilePath);

            var exclusiveLock = FileLock.Enter(lockFilePath);
            
            AssertLockFails(lockFilePath);

            exclusiveLock.Dispose();

            AssertNoLock(lockFilePath);
        }

        [Test]
        public void EnterWithCreate_LocksAndReleases()
        {
            var lockFilePath = Path.Combine(tempDir.AbsolutePath, LockFileName);

            AssertNoFile(lockFilePath);

            var exclusiveLock = FileLock.EnterWithCreate(lockFilePath);

            AssertLockFails(lockFilePath);

            exclusiveLock.Dispose();

            AssertNoLock(lockFilePath);
        }

        [Test]
        public void ReleasesOnFinalization()
        {
            var lockFilePath = Path.Combine(tempDir.AbsolutePath, LockFileName);
            var handle = FileLock.EnterWithCreate(lockFilePath);
            AssertLockFails(lockFilePath);
            handle = null;
            GC.Collect(3, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(3, GCCollectionMode.Forced, true);
            AssertNoLock(lockFilePath);
        }

        static void AssertLockFails(string lockFilePath)
        {
            // trying least exclusive file access
            Assert.Throws<IOException>(
                () =>
                {
                    File.Open(lockFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite).Dispose();
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
    }
}
