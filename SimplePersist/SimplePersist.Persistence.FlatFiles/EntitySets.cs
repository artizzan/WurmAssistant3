using System.Collections.Generic;

namespace AldurSoft.SimplePersist.Persistence.FlatFiles
{
    public class EntitySets
    {
        public Dictionary<string, Entities> EntitySetNameToEntities { get; set; }

        public EntitySets()
        {
            EntitySetNameToEntities = new Dictionary<string, Entities>();
        }
    }
}