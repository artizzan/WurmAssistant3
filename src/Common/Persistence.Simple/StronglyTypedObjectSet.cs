using System;

namespace AldursLab.Persistence.Simple
{
    public class StronglyTypedObjectSet<T> : IObjectSet<T> where T : class, new()
    {
        readonly ObjectSet objectSet;

        public StronglyTypedObjectSet(ObjectSet objectSet)
        {
            if (objectSet == null) throw new ArgumentNullException(nameof(objectSet));
            this.objectSet = objectSet;
        }

        public void SaveChanged()
        {
            objectSet.SaveChanged();
        }

        public string Id => objectSet.Id;

        public T GetOrCreate(string objectId)
        {
            return (T) objectSet.GetOrCreate<T>(objectId);
        }

        public void Delete(string objectId)
        {
            objectSet.Delete(objectId);
        }
    }
}