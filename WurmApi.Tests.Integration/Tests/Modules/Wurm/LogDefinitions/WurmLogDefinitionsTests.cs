using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Modules.Wurm.LogDefinitions
{
    class WurmLogDefinitionsTests : WurmTests
    {
        IWurmLogDefinitions System => Fixture.WurmApiManager.LogDefinitions;

        [Test]
        public void RecognizesTypes()
        {
            var type = System.TryGetTypeFromLogFileName("_Combat.2012-08.txt");
            Expect(type, EqualTo(LogType.Combat));
        }

        [Test]
        public void ReturnsUnspecifiedForUnknown()
        {
            var type = System.TryGetTypeFromLogFileName("nonsense.txt");
            Expect(type, EqualTo(LogType.Unspecified));
        }
    }
}
