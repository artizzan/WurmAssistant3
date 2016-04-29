using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Wurm.Characters;
using AldursLab.WurmApi.Tests.Integration.Helpers;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm.Characters
{
    [TestFixture]
    class WurmCharactersTests : WurmTests
    {
        protected IWurmCharacters System => Fixture.WurmApiManager.Characters;

        [SetUp]
        public virtual void Setup()
        {
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "WurmCharactersTests-wurmdir.7z"));
        }

        [Test]
        public void All_ReturnsAny()
        {
            var all = System.All;
            Expect(all.Count(), GreaterThan(0));
        }

        [Test]
        public void Get_ReturnsOne()
        {
            var character = System.Get(new CharacterName("Testguy"));
        }

        [Test]
        public void Get_ThrowsOnNotExisting()
        {
            Assert.Catch<WurmApiException>(() => System.Get(new CharacterName("Idonotexist")));
        }

        [TestFixture]
        class WurmCharacterTests : WurmCharactersTests
        {
            private IWurmCharacter wurmCharacter;

            public override void Setup()
            {
                base.Setup();
                wurmCharacter = System.Get(new CharacterName("Testguy"));
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
                var server = await wurmCharacter.TryGetHistoricServerAtLogStampAsync(new DateTime(2014, 12, 15));
                Expect(server.ServerName, EqualTo(new ServerName("Exodus")));
            }

            [Test]
            public async Task GetCurrentServer()
            {
                var server = await wurmCharacter.TryGetCurrentServerAsync();
                Expect(server.ServerName, EqualTo(new ServerName("Exodus")));
            }

            [Test]
            public async Task ReactsToCurrentServerChange()
            {
                //mocking current time to avoid test breaking precisely on midnight
                using (var scope = TimeStub.CreateStubbedScope())
                {
                    scope.OverrideNow(new DateTime(2015, 10, 06, 03, 10, 30));

                    var subscriber =
                        new Subscriber<CharacterDirectoriesChanged>(Fixture.WurmApiManager.InternalEventAggregator);
                    var serverChangeAwaiter = new EventAwaiter<PotentialServerChangeEventArgs>();

                    var playerdir = ClientMock.AddPlayer("Jack");
                    playerdir.SetConfigName("default");

                    // have to immediatelly create file, because during log monitor creation, existing file contents will not trigger events.
                    playerdir.Logs.CreateEventLogFile();
                    // have to wait until wurmapi picks up this folder
                    subscriber.WaitMessages(1);

                    var character = System.Get("Jack");
                    character.LogInOrCurrentServerPotentiallyChanged += serverChangeAwaiter.GetEventHandler();

                    Trace.WriteLine("writing first event");
                    playerdir.Logs.WriteEventLog("5 other players are online. You are on Exodus (50 totally in Wurm).");

                    serverChangeAwaiter.WaitUntilMatch(
                        list =>
                            list.Any(args => args.ServerName == new ServerName("Exodus")));

                    var server = await character.TryGetCurrentServerAsync();
                    Expect(server.ServerName, EqualTo(new ServerName("Exodus")));

                    Trace.WriteLine(
                        "writing second event");
                    playerdir.Logs.WriteEventLog(
                        "5 other players are online. You are on Deliverance (50 totally in Wurm).");

                    serverChangeAwaiter.WaitUntilMatch(
                        list =>
                            list.Any(args => args.ServerName == new ServerName("Deliverance")));

                    server = await character.TryGetCurrentServerAsync();
                    Expect(server.ServerName, EqualTo(new ServerName("Deliverance")));

                    Trace.WriteLine("writing third event");
                    playerdir.Logs.WriteEventLog(
                        "5 other players are online. You are on Deliverance (50 totally in Wurm).");

                    serverChangeAwaiter.WaitUntilMatch(
                        list =>
                            list.Any(args => args.ServerName == new ServerName("Deliverance")));

                    server = await character.TryGetCurrentServerAsync();
                    Expect(server.ServerName, EqualTo(new ServerName("Deliverance")));
                }
            }
        }
    }
}
