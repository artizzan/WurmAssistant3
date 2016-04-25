using System;
using System.Globalization;
using Newtonsoft.Json;

namespace AldursLab.WurmApi
{
    [JsonObject(MemberSerialization.Fields)]
    public class ServerName : IEquatable<ServerName>
    {
        private readonly string normalizedName;
        private readonly string originalName;

        public ServerName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            originalName = name;
            normalizedName = name.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// All uppercase server name
        /// </summary>
        public string Normalized => normalizedName;

        public string Original => originalName;

        public bool Equals(ServerName other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(normalizedName, other.normalizedName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as ServerName);
        }

        public override int GetHashCode()
        {
            return normalizedName?.GetHashCode() ?? 0;
        }

        public static bool operator ==(ServerName left, ServerName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ServerName left, ServerName right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return originalName.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks if string matches this server name, culture invariant ignore case.
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public bool Matches(string serverName)
        {
            return originalName.Equals(serverName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}