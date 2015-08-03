using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.WurmCharactersImpl
{
    [TestFixture]
    class WurmCharactersTests : WurmApiIntegrationFixtureBase
    {
        protected IWurmCharacters WurmCharacters;

        [SetUp]
        public virtual void Setup()
        {
            ConstructApi(Path.Combine(TestPaksDirFullPath, "WurmCharactersTests-wurmdir"));

            WurmCharacters = WurmApiManager.WurmCharacters;
        }

        [Test]
        public void All_ReturnsAny()
        {
            var all = WurmCharacters.All;
            Expect(all.Count(), GreaterThan(0));
        }

        [Test]
        public void Get_ReturnsOne()
        {
            var character = WurmCharacters.Get(new CharacterName("Testguy"));
        }

        [Test]
        public void Get_ThrowsOnNotExisting()
        {
            Assert.Catch<WurmApiException>(() => WurmCharacters.Get(new CharacterName("Idonotexist")));
        }

        [TestFixture]
        class WurmCharacterTests : WurmCharactersTests
        {
            private IWurmCharacter wurmCharacter;

            public override void Setup()
            {
                base.Setup();
                wurmCharacter = WurmCharacters.Get(new CharacterName("Testguy"));
            }

            [Test]
            public void Name_Gets()
            {
                Expect(wurmCharacter.Name, EqualTo(new CharacterName("Testguy")));
            }

            [Test]
            public void CurrentConfig_Gets()
            {
                Expect(wurmCharacter.CurrentConfig, !Null);
            }

            [Test]
            public async Task GetHistoricServerAtLogStamp_Gets()
            {
                var server = await wurmCharacter.GetHistoricServerAtLogStampAsync(new DateTime(2014, 12, 15));
                Expect(server.ServerName, EqualTo(new ServerName("Exodus")));
            }

            [Test]
            public async Task GetCurrentServer()
            {
                var server = await wurmCharacter.GetCurrentServerAsync();
                Expect(server.ServerName, EqualTo(new ServerName("Exodus")));
            }
        }
    }
}
