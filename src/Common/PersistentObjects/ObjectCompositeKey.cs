using System;

namespace AldursLab.PersistentObjects
{
    class ObjectCompositeKey : IEquatable<ObjectCompositeKey>
    {
        public string CollectionId { get; private set; }
        public string Key { get; private set; }

        public ObjectCompositeKey(string collectionId, string key)
        {
            CollectionId = collectionId;
            Key = key;
        }

        public bool Equals(ObjectCompositeKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(CollectionId, other.CollectionId) && string.Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ObjectCompositeKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CollectionId.GetHashCode()*397) ^ Key.GetHashCode();
            }
        }

        public static bool operator ==(ObjectCompositeKey left, ObjectCompositeKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObjectCompositeKey left, ObjectCompositeKey right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("CollectionId: {0}, Key: {1}", CollectionId, Key);
        }
    }
}