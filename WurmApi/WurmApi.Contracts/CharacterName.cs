using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AldurSoft.WurmApi
{
    [JsonObject(MemberSerialization.Fields)]
    public class CharacterName : IEquatable<CharacterName>
    {
        private readonly string normalizedName;

        public CharacterName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.normalizedName = name.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// All uppercase character name
        /// </summary>
        public string Normalized { get { return normalizedName; } }
        /// <summary>
        /// Name with first letter capitalized.
        /// </summary>
        public string Capitalized { get { return UnnormalizeCharacterName(normalizedName); } }

        private static string UnnormalizeCharacterName(string source)
        {
            if (source.Length > 0)
            {
                var result = source.Substring(0, 1).ToUpperInvariant();
                if (source.Length > 1)
                {
                    result += source.Substring(1, source.Length - 1).ToLowerInvariant();
                }
                return result;
            }
            return string.Empty;
        }

        public bool Equals(CharacterName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(normalizedName, other.normalizedName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as CharacterName);
        }

        public override int GetHashCode()
        {
            return (normalizedName != null ? normalizedName.GetHashCode() : 0);
        }

        public static bool operator ==(CharacterName left, CharacterName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CharacterName left, CharacterName right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Capitalized.ToString(CultureInfo.InvariantCulture);
        }
    }
}
