using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmApi
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ServerGroup : IEquatable<ServerGroup>
    {
        const string ServerScoped = "SERVERSCOPED:";
        public const string FreedomId = "FREEDOM";
        public const string EpicId = "EPIC";
        public const string UnknownId = "CONST:UNKNOWN";

        [JsonProperty]
        readonly string serverGroupId;

        /// <summary>
        /// Creates a server group scoped to a particular server name.
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static ServerGroup CreateServerScoped([NotNull] ServerName serverName)
        {
            if (serverName == null) throw new ArgumentNullException(nameof(serverName));
            return new ServerGroup(ServerScoped + serverName.Normalized);
        }

        /// <summary>
        /// Creates a server group with specific Id.
        /// Id's are always UpperCased.
        /// </summary>
        /// <param name="serverGroupId">Case insensitive</param>
        public ServerGroup([NotNull] string serverGroupId)
        {
            if (serverGroupId == null) throw new ArgumentNullException(nameof(serverGroupId));
            this.serverGroupId = serverGroupId.ToUpperInvariant();
        }

        /// <summary>
        /// Returns normalized server group id.
        /// </summary>
        public string ServerGroupId => serverGroupId;

        /// <summary>
        /// Determines if this server group is scoped to a particular server name.
        /// </summary>
        public bool IsServerScoped => serverGroupId.StartsWith(ServerScoped);

        public bool Equals(ServerGroup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(serverGroupId, other.serverGroupId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ServerGroup) obj);
        }

        public override int GetHashCode()
        {
            return serverGroupId.GetHashCode();
        }

        public static bool operator ==(ServerGroup left, ServerGroup right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ServerGroup left, ServerGroup right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return serverGroupId;
        }
    }
}
