using System.Collections.Generic;

namespace AldursLab.PersistentObjects.Persistence
{
    public class Collections
    {
        public Dictionary<string, Keys> CollectionIdToObjectsMap { get; private set; }

        public Collections()
        {
            CollectionIdToObjectsMap = new Dictionary<string, Keys>();
        }
    }
}