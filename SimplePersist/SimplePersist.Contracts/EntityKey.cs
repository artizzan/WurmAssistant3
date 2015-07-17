using System;

namespace AldurSoft.SimplePersist
{
    /// <summary>
    /// Entity keys have no limitations and are case sensitive.
    /// Note that for optimal performance keys should be short and use common subset of ASCII code page.
    /// </summary>
    public struct EntityKey : IEquatable<EntityKey>
    {
        private readonly string value;

        public EntityKey(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public bool Equals(EntityKey other)
        {
            return string.Equals(value, other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is EntityKey && Equals((EntityKey)obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }

        public static bool operator ==(EntityKey left, EntityKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityKey left, EntityKey right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format(Value);
        }

        public static implicit operator EntityKey(string key)
        {
            return new EntityKey(key);
        }
    }
}