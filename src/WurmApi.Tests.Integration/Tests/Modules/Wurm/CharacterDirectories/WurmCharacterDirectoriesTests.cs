using System.Linq;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Modules.Wurm.CharacterDirectories
{
    class WurmCharacterDirectoriesTests : WurmTests
    {
        // assumption: normalization convention for WurmApi is ToUpperInvariant

        // property-redirect to enable some setup, before wurmapi init 
        IWurmCharacterDirectories System => Fixture.WurmApiManager.WurmCharacterDirectories;

        [Test]
        public void ReturnsValidNormalizedNames()
        {
            var players = SetupDefaultPlayers().Select(s => s.ToUpperInvariant()).OrderBy(s => s);
            var dirnames = System.AllDirectoryNamesNormalized.OrderBy(s => s).ToArray();
            Expect(dirnames, EqualTo(players));
        }

        [Test]
        public void ReturnsValidFullPaths()
        {
            SetupDefaultPlayers();

            var realdirfullpaths = Fixture.WurmClientMock.Players.Select(player => player.PlayerDir.FullName).OrderBy(s => s).ToArray();
            var dirpaths = System.AllDirectoriesFullPaths.OrderBy(s => s).ToArray();
            Expect(dirpaths, EqualTo(realdirfullpaths));
        }

        [Test]
        public void OnChanged_TriggersEvent_UpdatesData()
        {
            var subscriber = new Subscriber<CharacterDirectoriesChanged>(Fixture.WurmApiManager.InternalEventAggregator);
            var batman = ClientMock.AddPlayer("Batman");
            subscriber.WaitMessages(1);

            // verifying event sent
            Expect(subscriber.ReceivedMessages.Count(), GreaterThan(0));

            // verifying data updated
            var allChars = System.GetAllCharacters().ToList();
            var allDirFullPaths = System.AllDirectoriesFullPaths.ToList();
            var allDirNames = System.AllDirectoryNamesNormalized.ToList();
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
