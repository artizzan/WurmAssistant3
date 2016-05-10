using System;
using System.Collections.Generic;
using System.IO;
using AldursLab.Essentials.Configs;
using AldursLab.Testing;
using NUnit.Framework;

namespace AldursLab.Essentials.Tests.Configs
{
    class FileSimpleConfigTests : AssertionHelper
    {
        const string SampleConfig =
@"#this is a comment, below is empty line

key=value
key2 = value2
  key3  =  value3
 complex key = complex value
Case Insensitive Key = Case Sensitive Value
key to be overriden = original value
";

        const string OverrideConfig =
@"#this is also a comment

key to be overriden = overriden value
";

        static readonly string[] InvalidCfgs = new[]
        {
            @"no delimiter",
            @"= empty key",
            @"empty value =",
        };

        DirectoryHandle tempDir;
        FileSimpleConfig config;
        string cfgPath;
        string cfgUsrPath;

        [SetUp]
        public void Setup()
        {
            tempDir = TempDirectoriesFactory.CreateEmpty();
            cfgPath = Path.Combine(tempDir.AbsolutePath, "config.cfg");
            cfgUsrPath = Path.Combine(tempDir.AbsolutePath, "config.cfg.usr");
        }

        [TearDown]
        public void Teardown()
        {
            tempDir.Dispose();
        }

        void SetupWithOverrides()
        {
            File.WriteAllText(cfgPath, SampleConfig);
            File.WriteAllText(cfgUsrPath, OverrideConfig);

            config = new FileSimpleConfig(cfgPath);
        }

        void SetupWithoutOverrides()
        {
            File.WriteAllText(cfgPath, SampleConfig);

            config = new FileSimpleConfig(cfgPath);
        }

        [Test]
        public void GetValue()
        {
            SetupWithOverrides();
            Expect(config.GetValue("key"), EqualTo("value"));
            Expect(config.GetValue("key2"), EqualTo("value2"));
            Expect(config.GetValue("key3"), EqualTo("value3"));
            Expect(config.GetValue("complex key"), EqualTo("complex value"));
            Expect(config.GetValue("case insensitive key"), EqualTo("Case Sensitive Value"));
            Expect(config.GetValue("key to be overriden"), EqualTo("overriden value"));
        }

        [Test]
        public void GetValue_WithoutOverrides()
        {
            SetupWithoutOverrides();
            Expect(config.GetValue("key to be overriden"), EqualTo("original value"));
        }

        [Test]
        public void HasValue()
        {
            SetupWithOverrides();
            Expect(config.HasValue("key"));
            Expect(config.HasValue("key2"));
            Expect(config.HasValue("key3"));
            Expect(config.HasValue("complex key"));
            Expect(config.HasValue("case insensitive key"));
            Expect(config.HasValue("key to be overriden"));
            Expect(!config.HasValue("i do not exist in config"));
        }

        [Test]
        public void ThrowsOnMissingKey()
        {
            SetupWithOverrides();
            Assert.Throws<KeyNotFoundException>(() => config.GetValue("i do not exist in config"));
        }

        [Test]
        public void ValidatesInvalidLines()
        {
            foreach (var invalidCfg in InvalidCfgs)
            {
                File.WriteAllText(cfgPath, invalidCfg);
                Assert.Catch<Exception>(() => config = new FileSimpleConfig(cfgPath));
            }
        }

        [Test]
        public void ReadsOverridesFromDifferentLocation()
        {
            using (DirectoryHandle tempDir1 = TempDirectoriesFactory.CreateEmpty())
            {
                using (DirectoryHandle tempDir2 = TempDirectoriesFactory.CreateEmpty())
                {
                    var localCfgPath = Path.Combine(tempDir1.AbsolutePath, "config.cfg");
                    var localCfgUsrPath = Path.Combine(tempDir2.AbsolutePath, "config.cfg.usr");
                    File.WriteAllText(localCfgPath, SampleConfig);
                    File.WriteAllText(localCfgUsrPath, OverrideConfig);
                    FileSimpleConfig localConfig = new FileSimpleConfig(localCfgPath, localCfgUsrPath);
                    Expect(localConfig.HasValue("key to be overriden"));
                    Expect(localConfig.GetValue("key to be overriden"), EqualTo("overriden value"));
                }
            }
        }

        [Test]
        public void PrintsConfig()
        {
            const string simpleConfig =
@"#comment
key1=value1
key2=value2
";

//            const string expectedOutputPattern =
//@"Main config file\: .+\\config.cfg
//Override config file\: N/A
//Values\:
//KEY1\=value1
//KEY2\=value2
//End";

            using (DirectoryHandle tempDir1 = TempDirectoriesFactory.CreateEmpty())
            {
                var localCfgPath = Path.Combine(tempDir1.AbsolutePath, "config.cfg");
                File.WriteAllText(localCfgPath, simpleConfig);
                FileSimpleConfig localConfig = new FileSimpleConfig(localCfgPath);
                var result = localConfig.ToString();
                //verify manually if needed
            }
        }
    }
}
