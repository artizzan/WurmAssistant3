using Aldurcraft.Core;

namespace WurmAssistant3.TestResources
{
    internal static class TestingEnvironment
    {
        public static string BinDirectory { get; private set; }

        static TestingEnvironment()
        {
            BinDirectory = (typeof(TestingEnvironment).Assembly.GetCodeBasePath());
        }
    }
}