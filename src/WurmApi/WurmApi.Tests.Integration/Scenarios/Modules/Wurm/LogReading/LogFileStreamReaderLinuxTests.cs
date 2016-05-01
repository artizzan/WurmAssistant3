using System.Collections.Generic;
using System.IO;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Tests.Integration.TempDirs;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Integration.Scenarios.Modules.Wurm.LogReading
{
    class LogFileStreamReaderLinuxTests : TestsBase
    {
        LogFileStreamReaderFactory system;
        WurmApiConfig wurmApiConfig;

        DirectoryHandle ubuntuDir;
        string sampleLogFilePath;

        [SetUp]
        public void Setup()
        {
            wurmApiConfig = new WurmApiConfig {Platform = Platform.Linux};

            system = new LogFileStreamReaderFactory(wurmApiConfig);

            ubuntuDir = TempDirectoriesFactory.CreateByUnzippingFile(Path.Combine(TestPaksZippedDirFullPath,
                "ubuntu-wurm-dir.7z"));

            sampleLogFilePath = Path.Combine(ubuntuDir.AbsolutePath,
                "players",
                "aldur",
                "logs",
                "_Event.2015-08.txt");
        }

        public void TearDown()
        {
            ubuntuDir.Dispose();
        }

        [Test]
        public void LinuxLogs_ResolvesAndReadsCorrectly()
        {
            wurmApiConfig.Platform = Platform.Linux;

            using (var reader = system.Create(sampleLogFilePath))
            {
                List<string> lines = new List<string>();
                string line;
                while ((line = reader.TryReadNextLine()) != null)
                {
                    lines.Add(line);
                }
                Expect(lines[0], EqualTo("Logging started 2015-08-16"));
                Expect(lines[1], EqualTo("[00:08:05] You will now fight normally."));
                Expect(lines[27], EqualTo("[00:19:43] You are no longer invulnerable."));
            }
        }

        [Test]
        public void LinuxLogs_ResolvesAndReadsCorrectly_WithLineSeek()
        {
            using (var reader = system.CreateWithLineCountFastForward(sampleLogFilePath, 3))
            {
                List<string> lines = new List<string>();
                string line;
                while ((line = reader.TryReadNextLine()) != null)
                {
                    lines.Add(line);
                }
                Expect(lines[0], EqualTo("[00:08:05] Baloo is logged in on Xanadu."));
                Expect(lines[24], EqualTo("[00:19:43] You are no longer invulnerable."));
            }
        }

        [Test]
        public void LinuxLogs_WithLineSeek_SkipAll()
        {
            using (var reader = system.CreateWithLineCountFastForward(sampleLogFilePath, 28))
            {
                Expect(reader.TryReadNextLine(), Null);
            }
        }
    }
}
