using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Testing;
using AldursLab.WurmAssistant3.Core.Root.Components;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using NUnit.Framework;
using Telerik.JustMock;
using Telerik.JustMock.AutoMock;

namespace AldursLab.WurmAssistant3.Tests.Unit.Root
{
    class ChangelogManagerTests : AssertionHelper
    {
        DirectoryHandle tempDir;
        string fileName = "changelog-raw.txt";

        MockingContainer<ChangelogManager> container;

        [SetUp]
        public void Setup()
        {
            tempDir = TempDirectoriesFactory.CreateEmpty();

            container = new MockingContainer<ChangelogManager>();
            container.Arrange<IBinDirectory>(directory => directory.FullPath).Returns(tempDir.FullName);

            WriteSampleChangelog();
        }

        [Test]
        public void ParsingResultShouldMatchExpectedOutput()
        {
            var changes = container.Instance.GetNewChanges();
            Expect(changes, Contains(ExpectedOutput1));
            Expect(changes, Contains(ExpectedOutput2));
        }

        [Test]
        public void GivenLastKnownParseDate_ParsingResultShouldMatchExpectedOutput()
        {
            container.Instance.LastKnownChangeDate = new DateTimeOffset(2015, 9, 29, 21, 18, 39, TimeSpan.FromHours(2));
            var changes = container.Instance.GetNewChanges();
            Expect(changes, Contains(ExpectedPartialOutput1));
            Expect(changes, Contains(ExpectedPartialOutput2));
        }

        void WriteSampleChangelog()
        {
            WriteChangelogFile("2015-09-29 15:18:39 +0200|Jack|change one complete");
            WriteChangelogFile("2015-09-29 21:18:39 +0200|Anna|change two complete");
            WriteChangelogFile("2015-09-29 23:18:39 +0200|Anna|change three complete");
            WriteChangelogFile("2015-09-30 13:18:39 +0200|Anna|change four complete|extra");
            WriteChangelogFile("2015-09-30 18:18:39 +0200|Jack|change five complete");
            WriteChangelogFile("2015-09-30 17:18:39 +0000|Jack|change six complete");
            WriteChangelogFile("2015-09-30 20:18:39 +0200|Jack|");
        }

        void WriteChangelogFile(params string[] lines)
        {
            File.AppendAllLines(Path.Combine(tempDir.FullName, fileName), lines, Encoding.UTF8);
        }

        const string ExpectedOutput1 =
@"By Anna:
- change four complete|extra
By Jack:
- change five complete
- change six complete";
        const string ExpectedOutput2 =
@"By Jack:
- change one complete
By Anna:
- change two complete
- change three complete";

        const string ExpectedPartialOutput1 =
@"By Anna:
- change four complete|extra
By Jack:
- change five complete
- change six complete";
        const string ExpectedPartialOutput2 = 
@"By Anna:
- change three complete";
    }
}
