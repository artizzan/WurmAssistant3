using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatureColor
    {
        [JsonProperty] 
        readonly CreatureColorId creatureColorId;

        public CreatureColor(CreatureColorId creatureColorId)
        {
            this.creatureColorId = creatureColorId;
        }

        public CreatureColor(string dbValue)
        {
            if (string.IsNullOrEmpty(dbValue)) creatureColorId = CreatureColorId.Unknown;
            else creatureColorId = (CreatureColorId)int.Parse(dbValue);
        }

        public CreatureColorId CreatureColorId => creatureColorId;

        public static CreatureColor GetDefaultColor()
        {
            return new CreatureColor(CreatureColorId.Unknown);
        }

        public static string[] GetColorsEnumStrArray()
        {
            return Enum.GetNames(typeof(CreatureColorId));
        }

        public static IEnumerable<CreatureColor> GetAll()
        {
            return Enum.GetValues(typeof (CreatureColorId)).Cast<CreatureColorId>().Select(x => new CreatureColor(x));
        }

        public override string ToString()
        {
            return creatureColorId.ToString();
        }

        public string ToDbValue()
        {
            return ((int)creatureColorId).ToString();
        }

        public bool Equals(CreatureColor other)
        {
            return creatureColorId.Equals(other.creatureColorId);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CreatureColor)) return false;
            CreatureColor other = (CreatureColor)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return creatureColorId.GetHashCode();
        }

        public static bool operator ==(CreatureColor left, CreatureColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CreatureColor left, CreatureColor right)
        {
            return !(left == right);
        }

        internal static CreatureColor CreateColorFromEnumString(string enumStr)
        {
            try
            {
                return new CreatureColor((CreatureColorId)Enum.Parse(typeof(CreatureColorId), enumStr, true));
            }
            catch (Exception)
            {
                return new CreatureColor(CreatureColorId.Unknown);
            }
        }

        internal static string GetDefaultColorStr()
        {
            return Enum.GetName(typeof(CreatureColorId), CreatureColorId.Unknown);
        }

        internal System.Drawing.Color? ToSystemDrawingColor()
        {
            switch (creatureColorId)
            {
                case CreatureColorId.Unknown:
                    return null;
                case CreatureColorId.White:
                    return System.Drawing.Color.GhostWhite;
                case CreatureColorId.Black:
                    return System.Drawing.Color.DarkSlateGray;
                case CreatureColorId.Brown:
                    return System.Drawing.Color.Brown;
                case CreatureColorId.Gold:
                    return System.Drawing.Color.Gold;
                case CreatureColorId.Grey:
                    return System.Drawing.Color.LightGray;
                case CreatureColorId.BloodBay:
                    return System.Drawing.Color.RosyBrown;
                case CreatureColorId.EbonyBlack:
                    return System.Drawing.Color.Black;
                case CreatureColorId.PiebaldPinto:
                    return System.Drawing.Color.DarkGray;
                default:
                    return null;
            }
        }
    }
}
