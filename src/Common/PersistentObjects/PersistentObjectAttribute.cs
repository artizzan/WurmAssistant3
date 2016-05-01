using System;
using JetBrains.Annotations;

namespace AldursLab.PersistentObjects
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistentObjectAttribute : Attribute
    {
        public string CollectionId { get; private set; }

        public PersistentObjectAttribute([NotNull] string collectionId)
        {
            if (collectionId == null)
            {
                throw new ArgumentNullException("collectionId");
            }
            if (collectionId.Length > 1000)
            {
                throw new ArgumentException("CollectionId must be less than 1000 characters. Lets be sane here.");
            }

            CollectionId = collectionId;
        }
    }
}