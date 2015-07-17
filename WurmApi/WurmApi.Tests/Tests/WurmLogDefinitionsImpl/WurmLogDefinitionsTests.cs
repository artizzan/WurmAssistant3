using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.WurmApi.Impl.WurmLogDefinitionsImpl;

using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmLogDefinitionsImpl
{
    public class WurmLogDefinitionsTests : WurmApiFixtureBase
    {
        readonly IWurmLogDefinitions system = new WurmLogDefinitions();

        [TestFixture]
        public class TryGetTypeFromLogFileName : WurmLogDefinitionsTests
        {
            [Test]
            public void Gets()
            {
                var type = system.TryGetTypeFromLogFileName("_Combat.2012-08.txt");
                Expect(type, EqualTo(LogType.Combat));
            }

            [Test]
            public void ReturnsNullIfNotParsed()
            {
                var type = system.TryGetTypeFromLogFileName("nonsense.txt");
                Expect(type, EqualTo(LogType.Unspecified));
            }
        }
    }
}
