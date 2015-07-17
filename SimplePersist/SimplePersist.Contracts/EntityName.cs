using System;

namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Entity names have no limitations and are case sensitive.
    /// Note that for optimal performance names should be short and use common subset of ASCII code page.
    /// </summary>
    public class EntityName : IEquatable<EntityName>
    {
        private readonly string value;

        public EntityName(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public bool Equals(EntityName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(value, other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityName)obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }

        public static bool operator ==(EntityName left, EntityName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityName left, EntityName right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format(Value);
        }

        public static implicit operator EntityName(string key)
        {
            return new EntityName(key);
        }
    }
}