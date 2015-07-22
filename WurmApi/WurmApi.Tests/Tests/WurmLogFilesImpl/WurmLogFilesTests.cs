using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core.Testing;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.CharacterDirectories.WurmCharacterDirectoriesModule;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Logs;
using AldurSoft.WurmApi.Wurm.Logs.Searching;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogDefinitionsModule;
using AldurSoft.WurmApi.Wurm.Logs.WurmLogFilesModule;
using AldurSoft.WurmApi.Wurm.Paths;
using AldurSoft.WurmApi.Wurm.Paths.WurmPathsModule;
using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace AldurSoft.WurmApi.Tests.Tests.WurmLogFilesImpl
{
    public class WurmLogFilesTests : WurmApiFixtureBase
    {
        protected WurmLogFiles system;
        protected TestPak wurmDir;
        protected WurmCharacterDirectories wurmCharacterDirectories;
        protected IWurmCharacterLogFiles testGuyLogFiles;

        protected int TotalFileCount;
        protected DirectoryInfo TestGuyDirectoryInfo;

        [SetUp]
        public void Setup()
        {
            wurmDir = CreateTestPakFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-emptyfiles"));

            TestGuyDirectoryInfo =
                new DirectoryInfo(wurmDir.DirectoryFullPath)
                    .GetDirectories("players")
                    .Single()
                    .GetDirectories("Testguy")
                    .Single();

            TotalFileCount = TestGuyDirectoryInfo.GetDirectories("logs").Single().GetFiles().Length;

            var installDir = Automocker.Create<IWurmInstallDirectory>();
            installDir.GetMock()
                .Setup(directory => directory.FullPath)
                .Returns(wurmDir.DirectoryFullPath);
            wurmCharacterDirectories = new WurmCharacterDirectories(new WurmPaths(installDir));
            system = new WurmLogFiles(wurmCharacterDirectories, Mock.Of<ILogger>(), new WurmLogDefinitions());
            testGuyLogFiles = system.GetManagerForCharacter(new CharacterName("Testguy"));
        }

        [TearDown]
        public override void Teardown()
        {
            ExecuteAll(system.Dispose, wurmCharacterDirectories.Dispose, base.Teardown);
        }

        class WurmCharacterLogFilesTests : WurmLogFilesTests
        {
            [TestFixture]
            class GetLogFiles_3ParamOverload : WurmCharacterLogFilesTests
            {
                [Test]
                public void GetsCorrectNumberOfFiles()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Combat).ToList();
                    Expect(logFiles.Count, EqualTo(5));
                }

                [Test]
                public void GetsFilesInCorrectOrder()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Combat).ToList();
                    var resortedLogFiles = logFiles.OrderBy(info => info.LogFileDate.DateTime).ToList();
                    Expect(resortedLogFiles, EqualTo(logFiles));
                }

                [Test]
                public void RespectsRange()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(new DateTime(2012, 01, 04), new DateTime(2012, 02, 01), LogType.Combat).ToList();
                    Expect(logFiles.Count, EqualTo(3));
                }

                [Test]
                public void RespectsLogType()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Event).ToList();
                    Expect(logFiles.Any(), True);
                }

                [Test]
                public void RetrievesPmLogs()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Pm).ToList();
                    Expect(logFiles.Any(), True);
                }

                [Test]
                public void GetsOldestDate()
                {
                    var oldestDate = testGuyLogFiles.OldestLogFileDate;
                    Expect(oldestDate, EqualTo(new DateTime(2012, 1, 1)));
                }
            }

            [TestFixture]
            class GetLogFiles_2ParamOverload : WurmCharacterLogFilesTests
            {
                [Test]
                public void GetsCorrectNumberOfFiles()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue).ToList();
                    Expect(logFiles.Count, EqualTo(TotalFileCount));
                }

                [Test]
                public void GetsFilesInCorrectOrder()
                {
                    var logFiles = testGuyLogFiles.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue).ToList();
                    var resortedLogFiles = logFiles.OrderBy(info => info.LogFileDate.DateTime).ToList();
                    Expect(resortedLogFiles, EqualTo(logFiles));
                }
            }

            [TestFixture]
            class GetLogFilesForSpecificPm : WurmCharacterLogFilesTests
            {
                [Test]
                public void GetsCorrectNumberOfFiles()
                {
                    var logFiles =
                        testGuyLogFiles.TryGetLogFilesForSpecificPm(
                            DateTime.MinValue,
                            DateTime.MaxValue,
                            new CharacterName("John")).ToList();
                    Expect(logFiles.Count, EqualTo(2));
                }

                [Test]
                public void GetsFilesInCorrectOrder()
                {
                    var logFiles =
                        testGuyLogFiles.TryGetLogFilesForSpecificPm(
                            DateTime.MinValue,
                            DateTime.MaxValue,
                            new CharacterName("John")).ToList();
                    var resortedLogFiles = logFiles.OrderBy(info => info.LogFileDate.DateTime).ToList();
                    Expect(resortedLogFiles, EqualTo(logFiles));
                }

                [Test]
                public void GivenNonExistingRecipient_ReturnsEmpty()
                {
                    var logFiles =
                        testGuyLogFiles.TryGetLogFilesForSpecificPm(
                            DateTime.MinValue,
                            DateTime.MaxValue,
                            new CharacterName("Idonotexist")).ToList();
                    Expect(logFiles.Count(), EqualTo(0));
                }
            }

            [TestFixture]
            class FilesAddedOrRemoved_Event : WurmCharacterLogFilesTests
            {
                [Test]
                public void NotifiesWhenAdded()
                {
                    bool notified = false;
                    testGuyLogFiles.FilesAddedOrRemoved += (sender, args) => notified = true;
                    Expect(notified, False);
                    using (File.Create(Path.Combine(TestGuyDirectoryInfo.FullName, "logs", "_Event.2012-04.txt"))) { }
                    WaitForFileSystem();
                    RefreshSystem();
                    Expect(notified, True);
                }

                [Test]
                public void NotifiesWhenRemoved()
                {
                    bool notified = false;
                    testGuyLogFiles.FilesAddedOrRemoved += (sender, args) => notified = true;
                    Expect(notified, False);
                    File.Delete(Path.Combine(TestGuyDirectoryInfo.FullName, "logs", "_Event.2012-03.txt"));
                    WaitForFileSystem();
                    RefreshSystem();
                    Expect(notified, True);
                }
            }
        }

        [TestFixture]
        class GetManagerForCharacter : WurmLogFilesTests
        {
            [Test]
            public void GetsManagers()
            {
                var manager = system.GetManagerForCharacter(new CharacterName("Testguy"));
                Expect(manager, EqualTo(testGuyLogFiles));
                var manager2 = system.GetManagerForCharacter(new CharacterName("Anotherguy"));
                Expect(manager2, !Null);
                Assert.Throws<WurmApiException>(() => system.GetManagerForCharacter(new CharacterName("Idonotexist")));
            }

            [Test]
            public void CreatesManagerForNewCharacter()
            {
                CreateNewCharacterEmptyDir("Newguy");             
                WaitForFileSystem();
                RefreshSystem();
                var manager = system.GetManagerForCharacter(new CharacterName("Newguy"));
                var files = manager.TryGetLogFiles(DateTime.MinValue, DateTime.MaxValue);
                Expect(files.Count(), EqualTo(0));
            }

            private void CreateNewCharacterEmptyDir(string characterName)
            {
                var dirinfo = new DirectoryInfo(wurmDir.DirectoryFullPath).GetDirectories("players").Single();
                var info = Directory.CreateDirectory(Path.Combine(dirinfo.FullName, characterName));
                Directory.CreateDirectory(Path.Combine(info.FullName, "logs"));
            }
        }

        [TestFixture]
        class TryGetLogFiles : WurmLogFilesTests
        {
            [Test]
            public void GetsFiles()
            {
                var results =
                    system.TryGetLogFiles(
                        new LogSearchParameters()
                        {
                            CharacterName = new CharacterName("Testguy"),
                            DateFrom = DateTime.MinValue,
                            DateTo = DateTime.MaxValue,
                            LogType = LogType.Event
                        });
                Expect(results.Count(), EqualTo(4));
            }

            [Test]
            public void GetLogFilesForNewCharacter()
            {
                CreateNewCharacterDirWithALog("Figurant", "_Event.2012-07.txt");
                WaitForFileSystem();
                RefreshSystem();

                var results =
                    system.TryGetLogFiles(
                        new LogSearchParameters()
                        {
                            CharacterName = new CharacterName("Figurant"),
                            DateFrom = DateTime.MinValue,
                            DateTo = DateTime.MaxValue,
                            LogType = LogType.Event
                        });

                Expect(results.Count(), EqualTo(1));
            }

            [Test]
            public void GivenNonExistingCharacter_ReturnsEmpty()
            {
                var results =
                    system.TryGetLogFiles(
                        new LogSearchParameters()
                        {
                            CharacterName = new CharacterName("Idonotexist"),
                            DateFrom = DateTime.MinValue,
                            DateTo = DateTime.MaxValue,
                            LogType = LogType.Event
                        });
                Expect(results.Count(), EqualTo(0));
            }

            private void CreateNewCharacterDirWithALog(string characterName, string logFileName)
            {
                var playersDir = new DirectoryInfo(wurmDir.DirectoryFullPath).GetDirectories("players").Single();
                var playerDir = Directory.CreateDirectory(Path.Combine(playersDir.FullName, characterName));
                var logsDir = Directory.CreateDirectory(Path.Combine(playerDir.FullName, "logs"));
                File.Create(Path.Combine(logsDir.FullName, logFileName)).Dispose();
            }
        }

        private void RefreshSystem()
        {
            ((IRequireRefresh)wurmCharacterDirectories).Refresh();
            ((IRequireRefresh)system).Refresh();
        }
    }
}
