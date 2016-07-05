using System;
using System.Collections.Generic;
using AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CreatureAge : IComparable, IComparable<CreatureAge>, IEquatable<CreatureAge>
    {
        static readonly Dictionary<string, CreatureAge> WurmStringToAgeMap = new Dictionary<string, CreatureAge>();

        [JsonProperty]
        readonly CreatureAgeId creatureAgeId;

        static CreatureAge()
        {
            WurmStringToAgeMap.Add(GrangerHelpers.Ages.Young.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Young));
            WurmStringToAgeMap.Add(GrangerHelpers.Ages.Adolescent.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Adolescent));
            WurmStringToAgeMap.Add(GrangerHelpers.Ages.Mature.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Mature));
            WurmStringToAgeMap.Add(GrangerHelpers.Ages.Aged.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Aged));
            WurmStringToAgeMap.Add(GrangerHelpers.Ages.Old.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Old));
            WurmStringToAgeMap.Add(GrangerHelpers.Ages.Venerable.ToUpperInvariant(), new CreatureAge(CreatureAgeId.Venerable));
        }

        public CreatureAge(CreatureAgeId creatureAgeId)
        {
            this.creatureAgeId = creatureAgeId;
        }

        public CreatureAge(string dbValue)
        {
            if (string.IsNullOrEmpty(dbValue)) creatureAgeId = CreatureAgeId.Unknown;
            else creatureAgeId = (CreatureAgeId)int.Parse(dbValue);
        }

        public CreatureAgeId CreatureAgeId => creatureAgeId;

        public static string[] GetColorsEnumStrArray()
        {
            return Enum.GetNames(typeof(CreatureAgeId));
        }

        public override string ToString()
        {
            return creatureAgeId.ToString();
        }

        public string ToDbValue()
        {
            return ((int)creatureAgeId).ToString();
        }

        public bool Equals(CreatureAge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return creatureAgeId == other.creatureAgeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreatureAge) obj);
        }

        public override int GetHashCode()
        {
            return (int) creatureAgeId;
        }

        internal static CreatureAge CreateAgeFromEnumString(string enumStr)
        {
            try
            {
                return new CreatureAge((CreatureAgeId)Enum.Parse(typeof(CreatureAgeId), enumStr, true));
            }
            catch (Exception)
            {
                return new CreatureAge(CreatureAgeId.Unknown);
            }
        }

        internal static string GetDefaultAgeStr()
        {
            return Enum.GetName(typeof(CreatureAgeId), CreatureAgeId.Unknown);
        }

        internal static CreatureAge CreateAgeFromRawCreatureName(string objectname)
        {
            objectname = objectname.ToUpperInvariant();
            foreach (var agestring in GrangerHelpers.CreatureAgesUpcase)
            {
                if (objectname.Contains(agestring))
                {
                    return WurmStringToAgeMap[agestring].CreateCopy();
                }
            }

            return new CreatureAge(CreatureAgeId.Unknown);
        }

        internal static CreatureAge CreateAgeFromRawCreatureNameStartsWith(string prefixedobjectname)
        {
            prefixedobjectname = prefixedobjectname.ToUpperInvariant();
            foreach (var agestring in GrangerHelpers.CreatureAgesUpcase)
            {
                if (prefixedobjectname.StartsWith(agestring, StringComparison.InvariantCultureIgnoreCase))
                {
                    return WurmStringToAgeMap[agestring].CreateCopy();
                }
            }

            return new CreatureAge(CreatureAgeId.Unknown);
        }

        CreatureAge CreateCopy()
        {
            return new CreatureAge(this.creatureAgeId);
        }

        public static CreatureAge Foalize([NotNull] CreatureAge creatureAge)
        {
            if (creatureAge == null) throw new ArgumentNullException(nameof(creatureAge));

            if (creatureAge.creatureAgeId == CreatureAgeId.Young)
            {
                return new CreatureAge(CreatureAgeId.YoungFoal);
            }
            if (creatureAge.creatureAgeId == CreatureAgeId.Adolescent)
            {
                return new CreatureAge(CreatureAgeId.AdolescentFoal);
            }

            throw new InvalidOperationException("Foalization is not available for age: "
                                                + creatureAge.creatureAgeId.ToString());
        }

        public int CompareTo(CreatureAge other)
        {
            return ((int)creatureAgeId).CompareTo((int)other.creatureAgeId);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is CreatureAge)
            {
                CreatureAge other = (CreatureAge)obj;
                return CompareTo(other);
            }
            else throw new ArgumentException("Object is not a CreatureAge");
        }

        public static bool operator <(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) > 0;
        }
        public static bool operator <=(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >=(CreatureAge left, CreatureAge right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator ==(CreatureAge left, CreatureAge right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreatureAge left, CreatureAge right)
        {
            return !Equals(left, right);
        }
    }
}