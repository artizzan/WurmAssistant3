using System;
using System.Linq;
using System.Threading;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Tests.Builders.WurmClient;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.CharacterDirectories
{
    public class WurmCharacterDirectoriesTests : WurmTests
    {
        // assumption: normalization convention for WurmApi is ToUpperInvariant

        IWurmCharacterDirectories characterDirectories;
        Subscriber<CharacterDirectoriesChanged> subscriber;

        [SetUp]
        public void Setup()
        {
            characterDirectories = Fixture.WurmApiManager.WurmCharacterDirectories;
            subscriber = new Subscriber<CharacterDirectoriesChanged>(Fixture.WurmApiManager.InternalEventAggregator);
        }

        [Test]
        public void ReturnsValidNormalizedNames()
        {
            var players = SetupDefaultPlayers().Select(s => s.ToUpperInvariant()).OrderBy(s => s);
            subscriber.WaitMessages(1);
            var dirnames = characterDirectories.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
            Expect(dirnames, EqualTo(players));
        }

        [Test]
        public void ReturnsValidFullPaths()
        {
            SetupDefaultPlayers();
            subscriber.WaitMessages(1);

            var realdirfullpaths = Fixture.WurmClientMock.Players.Select(player => player.PlayerDir.FullName).OrderBy(s => s).ToArray();
            var dirpaths = characterDirectories.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
            Expect(dirpaths, EqualTo(realdirfullpaths));
        }

        [Test]
        public void OnChanged_TriggersEvent_UpdatesData()
        {
            var batman = ClientMock.AddPlayer("Batman");
            subscriber.WaitMessages(1);

            // verifying event sent
            Expect(subscriber.ReceivedMessages.Count(), GreaterThan(0));

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
            ClientMock.AddPlayer("Foo");
            ClientMock.AddPlayer("Bar");
            return new string[] {"Foo", "Bar"};
        }
    }
}
