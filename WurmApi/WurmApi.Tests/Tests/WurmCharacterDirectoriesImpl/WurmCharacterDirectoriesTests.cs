using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Modules.Wurm.Paths;
using AldurSoft.WurmApi.Tests.Builders.WurmClient;
using AldurSoft.WurmApi.Tests.Helpers;
using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.WurmCharacterDirectoriesImpl
{
    public class WurmCharacterDirectoriesTests : AssertionHelper
    {
        WurmApiFixtureV2 fixture;
        WurmClientMock clientMock;
        IWurmCharacterDirectories characterDirectories;

        [SetUp]
        public void Setup()
        {
            fixture = new WurmApiFixtureV2();
            clientMock = fixture.WurmClientMock;
            characterDirectories = fixture.WurmApiManager.WurmCharacterDirectories;
        }

        [Test]
        public void ReturnsValidNormalizedNames()
        {
            var players = SetupDefaultPlayers().OrderBy(s => s);
            var dirnames = characterDirectories.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
            Expect(dirnames, EqualTo(players));
        }

        [Test]
        public void ReturnsValidFullPaths()
        {
            SetupDefaultPlayers();
            var realdirfullpaths = fixture.WurmClientMock.Players.Select(player => player.PlayerDir.FullName).OrderBy(s => s).ToArray();
            var dirpaths = characterDirectories.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
            Expect(dirpaths, EqualTo(realdirfullpaths));
        }

        [Test]
        public void OnChanged_TriggersEvent_UpdatesData()
        {
            var subscriber = CreateSubscriber();
            var batman = clientMock.AddPlayer("Batman");
            Thread.Sleep(20);

            // verifying event sent
            Expect(subscriber.ReceivedMessages, GreaterThan(0));

            // verifying data updated
            var allChars = characterDirectories.GetAllCharacters().ToList();
            var allDirFullPaths = characterDirectories.AllDirectoriesFullPaths.ToList();
            var allDirNames = characterDirectories.AllDirectoryNamesNormalized.ToList();
            Expect(allChars, Member(new CharacterName(batman.Name)).And.Count.EqualTo(1));
            Expect(allDirFullPaths, Member(batman.PlayerDir.FullName).And.Count.EqualTo(1));
            Expect(allDirNames, Member(batman.PlayerDir.Name.ToUpperInvariant()).And.Count.EqualTo(1));
        }

        string[] SetupDefaultPlayers()
        {
            clientMock.AddPlayer("Foo");
            clientMock.AddPlayer("Bar");
            return new string[] {"Foo", "Bar"};
        }

        Subscriber<CharacterDirectoriesChanged> CreateSubscriber()
        {
            return new Subscriber<CharacterDirectoriesChanged>(fixture.WurmApiManager.InternalEventAggregator);
        }
    }
}
