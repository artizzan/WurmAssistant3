using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.PersistentObjects.FlatFiles;
using AldursLab.WurmApi.Tests.TempDirs;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.PersistentCollectionsLibraryTests
{
    class TestsBase : AssertionHelper
    {
        DirectoryHandle dir;

        [SetUp]
        public virtual void Setup()
        {
            dir = TempDirectoriesFactory.CreateEmpty();
        }

        [TearDown]
        public virtual void TearDown()
        {
            dir.Dispose();
        }

        protected PersistentCollectionsLibrary CreateLibrary(CustomDeserializationErrorHandler customDeserializationErrorHandler = null)
        {
            return new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(dir.AbsolutePath), customDeserializationErrorHandler);
        }
    }
}