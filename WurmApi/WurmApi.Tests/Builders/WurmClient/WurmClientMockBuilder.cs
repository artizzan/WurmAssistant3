using System.IO;
using AldursLab.Testing;

namespace AldurSoft.WurmApi.Tests.Builders.WurmClient
{
    static class WurmClientMockBuilder
    {
        public static WurmClientMock Create()
        {
            var dir = TempDirectoriesFactory.CreateEmpty();
            return new WurmClientMock(dir);
        }
    }
}
