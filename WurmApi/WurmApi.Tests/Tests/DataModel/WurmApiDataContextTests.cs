using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.SimplePersist;
using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.DataModel
{
    //[TestFixture]
    //public class WurmApiDataContextTests : WurmApiFixtureBase
    //{
    //    private readonly WurmApiDataContext context;

    //    public WurmApiDataContextTests()
    //    {
    //        TestPak testDir = CreateTestPakEmptyDir();
    //        context = new WurmApiDataContext(testDir.DirectoryFullPath, Automocker.Create<ISimplePersistLogger>());
    //    }

    //    [Test]
    //    public void Constructs()
    //    {
    //        var entity = context.ServersData.Get(new EntityKey("test"));
    //        var dateTimeOffset = new DateTimeOffset(2014, 1, 1, 0, 0, 0, TimeSpan.Zero);
    //        entity.Entity.LastScanDate = dateTimeOffset;
    //        entity.Save();

    //        var verifyEntity = context.ServersData.Get(new EntityKey("test"));
    //        Expect(verifyEntity.Entity.LastScanDate, EqualTo(dateTimeOffset));
    //    }
    //}
}
