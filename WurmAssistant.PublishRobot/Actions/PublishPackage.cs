using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Essentials.Configs;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldursLab.Essentials.FileSystem;
using AldursLab.SevenZipper;
using AldursLab.WurmAssistant.PublishRobot.Parts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.PublishRobot.Actions
{
    class PublishPackage
    {
        readonly string tempDir;

        readonly string releaseLogPath;
        readonly string packageBinPath;
        readonly string webServiceRootUrl;
        readonly string webServiceControllerPath;
        readonly string webServiceLogin;
        readonly string webServicePassword;
        readonly Version lastPublishedVersion;
        readonly string teamcityLastPublishedVersionParamName;
        readonly string buildType;

        readonly IOutput output;

        public PublishPackage(IConfig config, [NotNull] string tempDir, [NotNull] IOutput output)
        {
            if (tempDir == null) throw new ArgumentNullException("tempDir");
            if (output == null) throw new ArgumentNullException("output");
            this.tempDir = tempDir;
            this.output = output;

            releaseLogPath = config.GetValue("release log path");
            packageBinPath = config.GetValue("package bin path");
            webServiceRootUrl = config.GetValue("web service root url");
            webServiceControllerPath = config.GetValue("web service controller path");
            webServiceLogin = config.GetValue("web service login");
            webServicePassword = config.GetValue("web service password");
            lastPublishedVersion = Version.Parse(config.GetValue("last published version"));
            teamcityLastPublishedVersionParamName = config.GetValue("teamcity version param name");
            buildType = config.GetValue("build type");
        }

        public void Execute()
        {
            output.Write("last version is " + lastPublishedVersion);

            var releaseDirs = GetReleaseDirInfos();

            var latestVersion = releaseDirs.Max(info => info.Version);
            output.Write("latest version is " + latestVersion);
            if (latestVersion > lastPublishedVersion)
            {
                var builder = new ChangelogBuilder(releaseDirs, output);
                var changelog = builder.Build();
                var binDir = new DirectoryInfo(packageBinPath);
                var tempPackageDir = new DirectoryInfo(Path.Combine(tempDir, "package"));

                DirectoryOps.CopyRecursively(binDir.FullName, tempPackageDir.FullName);

                var targetChangelogFile = new FileInfo(Path.Combine(tempPackageDir.FullName, "changelog.txt"));
                File.WriteAllText(targetChangelogFile.FullName, changelog);
                var zipper = new Zipper();
                
                var zipFile = new FileInfo(Path.Combine(tempDir, "package.7z"));
                zipper.Zip(tempPackageDir, zipFile);

                var uploader = new PackageWebPublisher(output);
                uploader.Publish(zipFile, latestVersion, buildType);

                UpdateTeamCity(latestVersion);
            }
            else
            {
                output.Write("latest version was already published, no publish executed");
            }
        }

        void UpdateTeamCity(Version newVersion)
        {
            output.Write(string.Format("##teamcity[setParameter name='{0}' value='{1}']",
                teamcityLastPublishedVersionParamName,
                newVersion.ToString()));
        }

        ReleaseDirInfo[] GetReleaseDirInfos()
        {
            var releaseDirInfo = new DirectoryInfo(releaseLogPath);
            var releaseDirs = releaseDirInfo.GetDirectories().Select(info =>
            {
                Version v = null;
                Version.TryParse(info.Name, out v);
                return new ReleaseDirInfo(info, v);
            }).Where(r => r.Version != null).ToArray();

            if (releaseDirs.None())
            {
                throw new InvalidOperationException(string.Format("No release subdirectories found at location {0}",
                    releaseDirInfo.FullName));
            }
            return releaseDirs;
        }
    }

    class ReleaseDirInfo
    {
        public ReleaseDirInfo(DirectoryInfo directoryInfo, Version version)
        {
            DirectoryInfo = directoryInfo;
            Version = version;
        }

        public DirectoryInfo DirectoryInfo { get; private set; }
        public Version Version { get; private set; }
    }
}
