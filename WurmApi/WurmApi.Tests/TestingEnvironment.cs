using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aldurcraft.Core;

namespace Aldurcraft.WurmApi.Tests
{
    internal static class TestingEnvironment
    {
        public static string BinDirectory { get; private set; }
        public static string PackagesDirectory { get; private set; }

        static TestingEnvironment()
        {
            BinDirectory = (typeof(TestingEnvironment).Assembly.GetCodeBasePath());
        }
    }
}
