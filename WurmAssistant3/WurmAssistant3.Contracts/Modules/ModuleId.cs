using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace AldurSoft.WurmAssistant3.Modules
{
    /// <summary>
    /// Module ID allows only letters and numbers, no whitespaces and is case insensitive.
    /// </summary>
    public sealed class ModuleId : IEquatable<ModuleId>
    {
        private readonly string idNormalized;
        private readonly string idOriginal;
        private const string ValidationRegexPattern = @"^[a-zA-Z0-9]+$";

        public ModuleId([NotNull] string id)
        {
            idOriginal = id;
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id is invalid, value: " + idNormalized);
            }
            if (!Regex.IsMatch(id, ValidationRegexPattern))
            {
                throw new ArgumentException(string.Format("Id {1} does not match regex pattern: {0}", ValidationRegexPattern, id));
            }
            this.idNormalized = id.ToUpperInvariant();
        }

        public string GetFilePathFriendlyString()
        {
            return idOriginal;
        }

        public override string ToString()
        {
            return idOriginal;
        }

        public bool Equals(ModuleId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(idNormalized, other.idNormalized);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleId)obj);
        }

        public override int GetHashCode()
        {
            return idNormalized.GetHashCode();
        }

        public static bool operator ==(ModuleId left, ModuleId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModuleId left, ModuleId right)
        {
            return !Equals(left, right);
        }
    }
}
