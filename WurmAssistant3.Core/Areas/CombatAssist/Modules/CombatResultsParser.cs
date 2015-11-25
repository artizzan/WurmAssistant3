using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi;

namespace AldursLab.WurmAssistant3.Core.Areas.CombatAssist.Modules
{
    public class CombatResultsParser
    {
        
    }

    sealed class CombatActor : IEquatable<CombatActor>
    {
        public string Name { get; private set; }

        public CombatActor(string name)
        {
            Name = name;
        }

        public List<CombatActor> Targets { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }

        public bool Equals(CombatActor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CombatActor) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(CombatActor left, CombatActor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CombatActor left, CombatActor right)
        {
            return !Equals(left, right);
        }
    }

    sealed class CombatData
    {
        public CombatActor CombatActor { get; private set; }

        public CombatData(CombatActor combat)
        {
            CombatActor = combat;
        }
    }
}
