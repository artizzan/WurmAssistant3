using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    public sealed class CreatureColor : IEquatable<CreatureColor>
    {
        readonly CreatureColorEntity creatureColorEntity;

        public CreatureColor([NotNull] CreatureColorEntity creatureColorEntity)
        {
            if (creatureColorEntity == null) throw new ArgumentNullException(nameof(creatureColorEntity));
            this.creatureColorEntity = creatureColorEntity;
        }

        public string CreatureColorId => creatureColorEntity.Id;

        public static CreatureColor GetDefaultColor()
        {
            return new CreatureColor(CreatureColorEntity.Unknown);
        }

        public Color SystemDrawingColor => creatureColorEntity.Color;

        public bool IsReadOnly => creatureColorEntity.IsReadOnly;

        public string DisplayName => creatureColorEntity.DisplayName;

        public string WurmLogText => creatureColorEntity.WurmLogText;

        public override string ToString()
        {
            return creatureColorEntity.DisplayName;
        }

        public bool Equals(CreatureColor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return creatureColorEntity.Id == other.creatureColorEntity.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreatureColor) obj);
        }

        public override int GetHashCode()
        {
            return (int)creatureColorEntity.Id.GetHashCode();
        }

        public static bool operator ==(CreatureColor left, CreatureColor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreatureColor left, CreatureColor right)
        {
            return !Equals(left, right);
        }
    }
}
