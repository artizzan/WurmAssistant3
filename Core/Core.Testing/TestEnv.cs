using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.Core.Testing
{
    public static class TestEnv
    {
        public static string BinDirectory { get; private set; }
        public static MockableClock MockableClock { get; private set; }

        static TestEnv()
        {
            MockableClock = new MockableClock();
            Time.Clock = MockableClock;

            BinDirectory = (typeof(TestEnv).Assembly.GetCodeBasePath());
        }
    }
}
