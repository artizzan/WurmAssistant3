using System.Collections.Generic;

namespace AldursLab.PersistentObjects.Persistence
{
    public class Keys
    {
        public string DirectoryName { get; set; }
        public string CollectionId { get; set; }

        public Dictionary<string, string> KeyToFileNameMap { get; private set; }

        public Keys()
        {
            KeyToFileNameMap = new Dictionary<string, string>();
        }
    }
}