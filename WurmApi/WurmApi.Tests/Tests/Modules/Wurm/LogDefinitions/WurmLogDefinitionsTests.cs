using AldurSoft.WurmApi.Modules.Wurm.LogDefinitions;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.LogDefinitions
{
    public class WurmLogDefinitionsTests : WurmTests
    {
        IWurmLogDefinitions System { get { return Fixture.WurmApiManager.WurmLogDefinitions; } }

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
