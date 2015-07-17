using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.SimplePersist
{
    public class PersistentManager : IPersistentManagerInternal
    {
        private readonly IPersistenceStrategy persistenceStrategy;
        private readonly ISimplePersistLogger logger;
        private readonly ISerializationStrategy serializationStrategy;

        private readonly Dictionary<EntityName, IPersistentSet> persistentSets = new Dictionary<EntityName, IPersistentSet>();

        public PersistentManager(ISerializationStrategy serializationStrategy, IPersistenceStrategy persistenceStrategy, ISimplePersistLogger logger)
        {
            if (persistenceStrategy == null) throw new ArgumentNullException("persistenceStrategy");
            if (serializationStrategy == null) throw new ArgumentNullException("serializationStrategy");
            this.persistenceStrategy = persistenceStrategy;
            this.logger = logger;
            this.serializationStrategy = serializationStrategy;
        }

        public IPersistenceStrategy PersistenceStrategy { get { return persistenceStrategy; } }
        public ISerializationStrategy SerializationStrategy { get { return serializationStrategy; } }
        public ISimplePersistLogger Logger
        {
            get { return logger; }
        }

        public IPersistentSet<TEntity> GetPersistentCollection<TEntity>(EntityName entitySetId)
            where TEntity : class, new()
        {
            IPersistentSet set;
            if (!persistentSets.TryGetValue(entitySetId, out set))
            {
                var typedSet = new PersistentSet<TEntity> { PersistentManager = this, EntityName = entitySetId };
                persistentSets.Add(entitySetId, typedSet);
                return typedSet;
            }
            else
            {
                var typedSet = set as IPersistentSet<TEntity>;
                if (typedSet == null)
                {
                    throw new EntityTypeMismatchException(
                        string.Format(
                            "{0} already contains entity set with this name, but is of different TEntity type: {1}.",
                            this.GetType().Name,
                            set.GetType().FullName));
                }
                return typedSet;
            }
        }

        public void SaveAllChanged()
        {
            foreach (var persistentSet in persistentSets.Values)
            {
                persistentSet.SaveAllChanged();
            }
        }
    }
}
