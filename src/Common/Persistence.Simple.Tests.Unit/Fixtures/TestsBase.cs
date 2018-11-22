using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.Persistence.Simple.Tests.Fixtures
{
    public abstract class TestsBase
    {
        DirectoryHandle dir;
        SqLiteDataStorage sqLiteDataStorage;

        [SetUp]
        public void Setup()
        {

            dir = TempDirectoriesFactory.CreateEmpty();
            sqLiteDataStorage = new SqLiteDataStorage(dir.FullName);
        }

        [TearDown]
        public void Teardown()
        {
            TeardownHelper.DisposeAll(sqLiteDataStorage, dir);
        }

        protected SampleContext CreateContext()
        {
            return new SampleContext(new DefaultJsonSerializer(), sqLiteDataStorage);
        }
    }
}