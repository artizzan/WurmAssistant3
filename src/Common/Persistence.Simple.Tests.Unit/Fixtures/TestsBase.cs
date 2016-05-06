using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.Persistence.Simple.Tests.Fixtures
{
    public abstract class TestsBase
    {
        DirectoryHandle dir;
        FlatFilesDataStorage flatFilesDataStorage;

        [SetUp]
        public void Setup()
        {

            dir = TempDirectoriesFactory.CreateEmpty();
            flatFilesDataStorage = new FlatFilesDataStorage(dir.FullName);
        }

        [TearDown]
        public void Teardown()
        {
            TeardownHelper.DisposeAll(flatFilesDataStorage, dir);
        }

        protected SampleContext CreateContext()
        {
            return new SampleContext(new DefaultJsonSerializer(), flatFilesDataStorage);
        }
    }
}