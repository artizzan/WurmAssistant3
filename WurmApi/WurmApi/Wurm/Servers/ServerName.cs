using System;
using System.Globalization;
using Newtonsoft.Json;

namespace AldurSoft.WurmApi.Wurm.Servers
{
    [JsonObject(MemberSerialization.Fields)]
    public class ServerName : IEquatable<ServerName>
    {
        private readonly string normalizedName;
        private readonly string originalName;

        public ServerName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            originalName = name;
            this.normalizedName = name.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// All uppercase character name
        /// </summary>
        public string Normalized { get { return normalizedName; } }

        public string Original { get { return originalName; } }

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
            return (normalizedName != null ? normalizedName.GetHashCode() : 0);
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
    }
}