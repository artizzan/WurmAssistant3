namespace AldursLab.Deprec.Core.Testing
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
