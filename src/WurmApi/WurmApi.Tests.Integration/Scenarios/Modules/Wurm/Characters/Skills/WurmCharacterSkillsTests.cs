using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi.Tests.Integration.Builders.WurmClient;
using AldursLab.WurmApi.Tests.Integration.Helpers;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm.Characters.Skills
{
    class WurmCharacterSkillsTests : WurmTests
    {
        StubbableTime.StubScope timeScope;

        public IWurmCharacterSkills TestguySkills => Fixture.WurmApiManager.Characters.Get("Testguy").Skills;

        public IWurmCharacterSkills TestguytwoSkills => Fixture.WurmApiManager.Characters.Get("Testguytwo").Skills;

        [SetUp]
        public void Setup()
        {
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "WurmCharacterSkillsTests-wurmdir.7z"));

            if (timeScope != null)
            {
                timeScope.Dispose();
                timeScope = null;
            }
            timeScope = TimeStub.CreateStubbedScope();
            var date = new DateTime(2012, 09, 23, 23, 37, 13);
            timeScope.OverrideNow(date);
            timeScope.OverrideNowOffset(date);
        }

        [TearDown]
        public void Teardown()
        {
            timeScope.Dispose();
            timeScope = null;
        }

        [Test]
        public async Task GetsCurrentSkills()
        {
            var skill = await TestguySkills.TryGetCurrentSkillLevelAsync("Masonry",
                new ServerGroup("FREEDOM"), 
                TimeSpan.FromDays(1000));
            Expect(skill, !Null);
            Expect(skill.Value, EqualTo(58.751f));
        }

        [Test]
        public async Task FallsBackToDumpsWhenNoSkillLogs()
        {
            var skill = await TestguytwoSkills.TryGetCurrentSkillLevelAsync("Masonry",
                new ServerGroup("FREEDOM"),
                TimeSpan.FromDays(1000));
            Expect(skill.Value, EqualTo(73.73132f));
        }

        /// <summary>
        /// Ingoring on build server.
        /// The test works on dev machine, but doesn't on build machine, frakking file watcher is fishy. 
        /// Todo: [opt] replace filewatcher with polling-based system.
        /// </summary>
        /// <returns></returns>
        [Test, Category("Manual")]
        public async Task RefreshesDumpsWhenNewFound()
        {
            var skill = await TestguytwoSkills.TryGetCurrentSkillLevelAsync("Masonry",
                new ServerGroup("FREEDOM"),
                TimeSpan.FromDays(1000));
            Expect(skill.Value, EqualTo(73.73132f));

            // allow file watcher to begin tracking directories
            await Task.Delay(500);

            DumpFileBuilder builder = new DumpFileBuilder(
                new DirectoryInfo(Path.Combine(ClientMock.InstallDirectory.FullPath, "players", "Testguytwo", "dumps")));

            builder.SetStamp(TimeStub.LocalNowOffset);
            builder.Add(new DumpEntry() { SkillName = "Masonry", Level = 80f });
            builder.CreateFile();

            timeScope.AdvanceTime(15);

            // allow system to pick up the file change and set cache as dirty
            await Task.Delay(500);

            skill = await TestguytwoSkills.TryGetCurrentSkillLevelAsync("Masonry",
                new ServerGroup("FREEDOM"),
                TimeSpan.FromDays(1000));
            Expect(skill.Value, EqualTo(80f));
        }

        [Test]
        public async Task ReactsToLiveEvents()
        {
            timeScope.AdvanceTime(1);

            var player = ClientMock.AddPlayer("Newguy");
            player.Logs.WriteEventLog("42 other players are online. You are on Exodus (765 totally in Wurm).");

            var skillApi = Fixture.WurmApiManager.Characters.Get("Newguy")
                                  .Skills;

            // allow WurmApi to pick everything
            await Task.Delay(1000);

            timeScope.AdvanceTime(1);

            var awaiter = new EventAwaiter<SkillsChangedEventArgs>();
            skillApi.SkillsChanged += awaiter.GetEventHandler();

            player.Logs.WriteSkillLog("Masonry", 58.754f);
            awaiter.WaitInvocations(1);
            awaiter.WaitUntilMatch(list => list.Any(args => args.HasSkillChanged("Masonry")));

            var skill = await skillApi.TryGetCurrentSkillLevelAsync("Masonry", new ServerGroup("FREEDOM"), TimeSpan.MaxValue);
            Expect(skill.Value, EqualTo(58.754f));
        }
    }
}
