using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldurSoft.WurmApi.Tests.Tests.Modules.Wurm.LogFiles
{
    public class WurmLogFilesTests : WurmTests
    {
        protected IWurmLogFiles System;
        protected IWurmCharacterLogFiles TestGuyLogFiles;

        protected int TotalFileCount;
        protected DirectoryInfo TestGuyDirectoryInfo;

        protected readonly CharacterName TestGuyCharacterName = new CharacterName("Testguy");

        [SetUp]
        public void Setup()
        {
            //ClientMock.PopulateFromDir(Path.Combine(TestPaksDirFullPath, "logs-samples-emptyfiles"));
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "logs-samples-emptyfiles.7z"));

            TestGuyDirectoryInfo =
                new DirectoryInfo(ClientMock.InstallDirectory.FullPath)
                    .GetDirectories("players")
                    .Single()
                    .GetDirectories("Testguy")
                    .Single();

            TotalFileCount = TestGuyDirectoryInfo.GetDirectories("logs").Single().GetFiles().Length;

            System = Fixture.WurmApiManager.WurmLogFiles;
            TestGuyLogFiles = System.GetForCharacter(TestGuyCharacterName);
        }

        class WurmCharacterLogFilesTests : WurmLogFilesTests
        {
            class GetLogFiles_3ParamOverload : WurmCharacterLogFilesTests
            {
                [Test]
                public void GetsCorrectNumberOfFiles()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Combat).ToList();
                    Expect(logFiles.Count, EqualTo(5));
                }

                [Test]
                public void GetsFilesInCorrectOrder()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Combat).ToList();
                    var resortedLogFiles = logFiles.OrderBy(info => info.LogFileDate.DateTime).ToList();
                    Expect(resortedLogFiles, EqualTo(logFiles));
                }

                [Test]
                public void RespectsRange()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(new DateTime(2012, 01, 04), new DateTime(2012, 02, 01), LogType.Combat).ToList();
                    Expect(logFiles.Count, EqualTo(3));
                }

                [Test]
                public void RespectsLogType()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Event).ToList();
                    Expect(logFiles.Any(), True);
                }

                [Test]
                public void RetrievesPmLogs()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Pm).ToList();
                    Expect(logFiles.Any(), True);
                }

                [Test]
                public void GetsOldestDate()
                {
                    var oldestDate = TestGuyLogFiles.OldestLogFileDate;
                    Expect(oldestDate, EqualTo(new DateTime(2012, 1, 1)));
                }
            }

            class GetLogFiles_2ParamOverload : WurmCharacterLogFilesTests
            {
                [Test]
                public void GetsCorrectNumberOfFiles()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(DateTime.MinValue, DateTime.MaxValue).ToList();
                    Expect(logFiles.Count, EqualTo(TotalFileCount));
                }

                [Test]
                public void GetsFilesInCorrectOrder()
                {
                    var logFiles = TestGuyLogFiles.GetLogFiles(DateTime.MinValue, DateTime.MaxValue).ToList();
                    var resortedLogFiles = logFiles.OrderBy(info => info.LogFileDate.DateTime).ToList();
                    Expect(resortedLogFiles, EqualTo(logFiles));
                }
            }

            class GetLogFilesForSpecificPm : WurmCharacterLogFilesTests
            {
                [Test]
                public void GetsCorrectNumberOfFiles()
                {
                    var logFiles =
                        TestGuyLogFiles.TryGetLogFilesForSpecificPm(
                            DateTime.MinValue,
                            DateTime.MaxValue,
                            new CharacterName("John")).ToList();
                    Expect(logFiles.Count, EqualTo(2));
                }

                [Test]
                public void GetsFilesInCorrectOrder()
                {
                    var logFiles =
                        TestGuyLogFiles.TryGetLogFilesForSpecificPm(
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
                        TestGuyLogFiles.TryGetLogFilesForSpecificPm(
                            DateTime.MinValue,
                            DateTime.MaxValue,
                            new CharacterName("Idonotexist")).ToList();
                    Expect(logFiles.Count(), EqualTo(0));
                }
            }

            class FilesAddedOrRemoved_Event : WurmCharacterLogFilesTests
            {
                Subscriber<CharacterLogFilesAddedOrRemoved> subscriber;
                Func<CharacterLogFilesAddedOrRemoved, bool> characterIsMatch;

                [SetUp]
                public void SetupLocal()
                {
                    subscriber = new Subscriber<CharacterLogFilesAddedOrRemoved>(Fixture.WurmApiManager.InternalEventAggregator);
                    characterIsMatch = m => m.CharacterName == TestGuyCharacterName;
                }

                [Test]
                public void NotifiesWhenAdded()
                {
                    Expect(subscriber.ReceivedMessages.Count(), EqualTo(0));
                    using (File.Create(Path.Combine(TestGuyDirectoryInfo.FullName, "logs", "_Event.2012-04.txt"))) { }
                    subscriber.WaitMessages(1, characterIsMatch);
                }

                [Test]
                public void NotifiesWhenRemoved()
                {
                    Expect(subscriber.ReceivedMessages.Count(), EqualTo(0));
                    File.Delete(Path.Combine(TestGuyDirectoryInfo.FullName, "logs", "_Event.2012-03.txt"));

                    subscriber.WaitMessages(1, characterIsMatch);
                }
            }
        }

        class GetManagerForCharacter : WurmLogFilesTests
        {
            Subscriber<CharacterLogFilesAddedOrRemoved> subscriber;

            [SetUp]
            public void SetupLocal()
            {
                subscriber = new Subscriber<CharacterLogFilesAddedOrRemoved>(Fixture.WurmApiManager.InternalEventAggregator);
            }

            [Test]
            public void GetsManagers()
            {
                var manager = System.GetForCharacter(new CharacterName("Testguy"));
                Expect(manager, EqualTo(TestGuyLogFiles));
                var manager2 = System.GetForCharacter(new CharacterName("Anotherguy"));
                Expect(manager2, !Null);
                Assert.Throws<DataNotFoundException>(() => System.GetForCharacter(new CharacterName("Idonotexist")));
            }

            [Test]
            public void CreatesManagerForNewCharacter()
            {
                CreateNewCharacterEmptyDir("Newguy");    
                // there is a delay before new directory gets observed
                IWurmCharacterLogFiles manager = null;
                WaitUntilTrue(() =>
                {
                    try
                    {
                        manager = System.GetForCharacter(new CharacterName("Newguy"));
                        return true;
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        return false;
                    }
                });
                
                var files = manager.GetLogFiles(DateTime.MinValue, DateTime.MaxValue);
                Expect(files.Count(), EqualTo(0));
            }

            private void CreateNewCharacterEmptyDir(string characterName)
            {
                ClientMock.AddPlayer(characterName);
            }
        }

        class TryGetLogFiles : WurmLogFilesTests
        {
            Subscriber<CharacterLogFilesAddedOrRemoved> subscriber;

            [SetUp]
            public void SetupLocal()
            {
                subscriber = new Subscriber<CharacterLogFilesAddedOrRemoved>(Fixture.WurmApiManager.InternalEventAggregator);
            }

            [Test]
            public void GetsFiles()
            {
                var manager = System.GetForCharacter(TestGuyCharacterName);
                var results = manager.GetLogFiles(DateTime.MinValue,DateTime.MaxValue,LogType.Event);
                Expect(results.Count(), EqualTo(4));
            }

            [Test]
            public void GetLogFilesForNewCharacter()
            {
                var newCharacterName = new CharacterName("Figurant");

                Expect(subscriber.ReceivedMessages.Count(m => m.CharacterName == newCharacterName), EqualTo(0));

                CreateNewCharacterDirWithALog("Figurant", "_Event.2012-07.txt");

                IWurmCharacterLogFiles manager = null;
                WaitUntilTrue(() =>
                {
                    try
                    {
                        manager = System.GetForCharacter(newCharacterName);
                        return true;
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        return false;
                    }
                });

                subscriber.WaitMessages(1, m => m.CharacterName == newCharacterName);
                var results = manager.GetLogFiles(DateTime.MinValue, DateTime.MaxValue, LogType.Event);
                Expect(results.Count(), EqualTo(1));
            }

            [Test]
            public void GivenNonExistingCharacter_ReturnsEmpty()
            {
                var wrongCharacterName = new CharacterName("Figurant");
                Assert.Throws<DataNotFoundException>(() => System.GetForCharacter(wrongCharacterName));
            }

            private void CreateNewCharacterDirWithALog(string characterName, string logFileName)
            {
                var player = ClientMock.AddPlayer(characterName);
                player.Logs.CreateLogFile(logFileName);
            }
        }
    }
}
