using AldursLab.WurmApi.Tests.TempDirs;

namespace AldursLab.WurmApi.Tests.Builders.WurmClient
{
    static class WurmClientMockBuilder
    {
        public static WurmClientMock Create(Platform targetPlatform = Platform.Windows)
        {
            var dir = TempDirectoriesFactory.CreateEmpty();
            return new WurmClientMock(dir, true, targetPlatform);
        }

        public static WurmClientMock CreateWithoutBasicDirs(Platform targetPlatform = Platform.Windows)
        {
            var dir = TempDirectoriesFactory.CreateEmpty();
            return new WurmClientMock(dir, false, targetPlatform);
        }
    }
}
