using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmAssistant.Shared
{
    /// <summary>
    /// Represents version information of a particular WA3 build.
    /// BuildCode and BuildNumber are the unique identifiers of every build. Remaining properties are informational.
    /// Equality for this object includes only identifiers.
    /// </summary>
    public class Wa3VersionInfo : IEquatable<Wa3VersionInfo>
    {
        public Wa3VersionInfo(string buildCode, string buildNumber)
        {
            if (buildCode == null) throw new ArgumentNullException("buildCode");
            if (buildNumber == null) throw new ArgumentNullException("buildNumber");
            BuildCode = buildCode;
            BuildNumber = buildNumber;
        }

        public Wa3VersionInfo(string buildCode, string buildNumber, string minorVersion, DateTimeOffset buildStamp)
            : this(buildCode, buildNumber)
        {
            MinorVersion = minorVersion;
            BuildStamp = buildStamp;
        }

        public static Wa3VersionInfo CreateFromVersionDat(string content)
        {
            var split = content.Replace("\r\n", "\n").Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 4)
            {
                throw new InvalidOperationException("Content does not conform to version.dat format");
            }
            return new Wa3VersionInfo(
                split[1].Trim(),
                split[2].Trim(),
                split[0].Trim(),
                DateTimeOffset.ParseExact(split[3].Trim(), "O", CultureInfo.InvariantCulture));
        }

        public string ConvertIntoVersionDatContents()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}",
                MinorVersion,
                BuildCode,
                BuildNumber,
                DateTimeOffset.Now.ToString("O"));
        }

        /// <summary>
        /// Optional. Informational. The major + minor version identifier eg. 3.1
        /// </summary>
        public string MinorVersion { get; private set; }
        /// <summary>
        /// Code of this build, refrects the source branch this version comes from
        /// </summary>
        public string BuildCode { get; private set; }
        /// <summary>
        /// Unique build number within BuildCode
        /// </summary>
        public string BuildNumber { get; private set; }
        /// <summary>
        /// Optional. Informational. Approximate date of the build.
        /// </summary>
        public DateTimeOffset BuildStamp { get; private set; }

        public bool Equals(Wa3VersionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(BuildCode, other.BuildCode) && string.Equals(BuildNumber, other.BuildNumber);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Wa3VersionInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((BuildCode != null ? BuildCode.GetHashCode() : 0)*397) ^ (BuildNumber != null ? BuildNumber.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Wa3VersionInfo left, Wa3VersionInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Wa3VersionInfo left, Wa3VersionInfo right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("MinorVersion: {0}, BuildCode: {1}, BuildNumber: {2}, BuildStamp: {3}",
                MinorVersion,
                BuildCode,
                BuildNumber,
                BuildStamp);
        }
    }
}
