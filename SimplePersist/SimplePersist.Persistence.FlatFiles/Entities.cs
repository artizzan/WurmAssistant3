using System.Collections.Generic;

namespace AldurSoft.SimplePersist.Persistence.FlatFiles
{
    public class Entities
    {
        public string DirectoryName { get; set; }
        public string EntityName { get; set; }

        public Dictionary<string, string> EntityKeyToFileName { get; set; }

        public Entities()
        {
            EntityKeyToFileName = new Dictionary<string, string>();
        }
    }
}