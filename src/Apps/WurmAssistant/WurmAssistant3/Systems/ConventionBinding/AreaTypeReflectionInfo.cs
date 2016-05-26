using System;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    public sealed class AreaTypeReflectionInfo : IEquatable<AreaTypeReflectionInfo>
    {
        public AreaReflectionInfo AreaReflectionInfo { get; }
        public Type Type { get; }

        public AreaTypeReflectionInfo([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            AreaReflectionInfo = new AreaReflectionInfo(type);
            Type = type;
        }

        #region IEquatable

        public bool Equals(AreaTypeReflectionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type.Equals(other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AreaTypeReflectionInfo && Equals((AreaTypeReflectionInfo) obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public static bool operator ==(AreaTypeReflectionInfo left, AreaTypeReflectionInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AreaTypeReflectionInfo left, AreaTypeReflectionInfo right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}