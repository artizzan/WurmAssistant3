using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.DataLayer
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class GrangerContext
    {
        readonly GrangerSimpleDb grangerSimpleDb;

        public GrangerContext([NotNull] GrangerSimpleDb grangerSimpleDb)
        {
            if (grangerSimpleDb == null) throw new ArgumentNullException(nameof(grangerSimpleDb));
            this.grangerSimpleDb = grangerSimpleDb;
        }

        public IEnumerable<CreatureEntity> Creatures => grangerSimpleDb.Creatures.Values.ToArray();

        public IEnumerable<TraitValueEntity> TraitValues => grangerSimpleDb.TraitValues.Values.ToArray();

        public IEnumerable<HerdEntity> Herds => grangerSimpleDb.Herds.Values.ToArray();

        public IEnumerable<CreatureColorEntity> CreatureColorEntities => grangerSimpleDb.CreatureColors.Values.ToArray();

        public event EventHandler<EventArgs> OnHerdsModified;
        public event EventHandler<EventArgs> OnTraitValuesModified;
        public event EventHandler<EventArgs> OnCreaturesModified;
        public event EventHandler<EventArgs> OnCreatureColorsModified; 

        #region HERD OPS

        public void InsertHerd(string herdName)
        {
            HerdEntity newHerd = new HerdEntity {HerdId = herdName};
            if (grangerSimpleDb.Herds.ContainsKey(herdName))
            {
                throw new ApplicationException("There already exists a herd with id " + herdName);
            }
            grangerSimpleDb.Herds[herdName] = newHerd;
            grangerSimpleDb.Save();
            OnHerdsModified?.Invoke(this, new EventArgs());
        }

        internal void RenameHerd(string renamingHerd, string newHerdName)
        {
            if (grangerSimpleDb.Herds.ContainsKey(newHerdName))
            {
                throw new ApplicationException("There already exists a herd with id " + newHerdName);
            }

            HerdEntity oldherd = Herds.Single(x => x.HerdId == renamingHerd);

            HerdEntity newHerd = oldherd.CloneMe(newHerdName);
            newHerd.HerdId = newHerdName;
            grangerSimpleDb.Herds[newHerdName] = newHerd;

            List<CreatureEntity> creaturesInThisHerd = Creatures.Where(x => x.Herd == renamingHerd).ToList();
            creaturesInThisHerd.ForEach(x => x.Herd = newHerdName);

            grangerSimpleDb.Herds.Remove(oldherd.HerdId);
            grangerSimpleDb.Save();
            OnHerdsModified?.Invoke(this, new EventArgs());
        }

        internal void DeleteHerd(string herdName)
        {
            HerdEntity herd = Herds.Single(x => x.HerdId == herdName);
            CreatureEntity[] creaturesInThisHerd = Creatures.Where(x => x.Herd == herdName).ToArray();
            foreach (var creatureEntity in creaturesInThisHerd)
            {
                grangerSimpleDb.Creatures.Remove(creatureEntity.Id);
            }
            grangerSimpleDb.Herds.Remove(herd.HerdId);
            grangerSimpleDb.Save();
            OnHerdsModified?.Invoke(this, new EventArgs());
        }

        public class DuplicateCreatureIdentityException : Exception
        {
            public DuplicateCreatureIdentityException(string message)
                : base(message)
            {
            }
        }

        /// <exception cref="DuplicateCreatureIdentityException"></exception>
        internal void MergeHerds(string sourceHerdName, string destinationHerdName)
        {
            HerdEntity sourceHerd = Herds.Single(x => x.HerdId == sourceHerdName);
            HerdEntity destinationHerd = Herds.Single(x => x.HerdId == destinationHerdName);

            CreatureEntity[] creaturesInSource = Creatures.Where(x => x.Herd == sourceHerd.HerdId).ToArray();
            CreatureEntity[] creaturesInDestination = Creatures.Where(x => x.Herd == destinationHerd.HerdId).ToArray();

            List<CreatureEntity> nonUniqueIdentityCreatures = new List<CreatureEntity>();
            foreach (var sourceCreatures in creaturesInSource)
            {
                foreach (var destinationCreature in creaturesInDestination)
                {
                    if (sourceCreatures.IsUniquelyIdentifiableWhenComparedTo(destinationCreature) == false)
                        nonUniqueIdentityCreatures.Add(sourceCreatures);
                }
            }

            if (nonUniqueIdentityCreatures.Any())
            {
                throw new DuplicateCreatureIdentityException("target herd: "
                    + destinationHerd.HerdId
                    + " already contains creature(s) of same identity: "
                    + string.Join(", ", nonUniqueIdentityCreatures));
            }
            else
            {
                foreach (var creature in creaturesInSource)
                {
                    creature.Herd = destinationHerd.HerdId;
                }
                grangerSimpleDb.Herds.Remove(sourceHerd.HerdId);
                grangerSimpleDb.Save();
                OnHerdsModified?.Invoke(this, new EventArgs());
            }
        }

        internal void UpdateHerdSelectedState(string herdId, bool newState)
        {
            HerdEntity entity = Herds.Single(x => x.HerdId == herdId);
            entity.Selected = newState;
            grangerSimpleDb.Save();
            OnHerdsModified?.Invoke(this, new EventArgs());
        }

        #endregion

        internal void UpdateOrCreateTraitValueMap(IReadOnlyDictionary<CreatureTraitId, int> valueMap, string traitValueMapId)
        {
            var entities = TraitValues.Where(x => x.ValueMapId == traitValueMapId).ToArray();
            if (entities.Length > 0)
            {
                foreach (var traitValueEntity in entities)
                {
                    grangerSimpleDb.TraitValues.Remove(traitValueEntity.Id);
                }
            }

            int nextIndex = TraitValueEntity.GenerateNewTraitValueId(this);
            foreach (var keyval in valueMap)
            {
                TraitValueEntity newentity = new TraitValueEntity()
                {
                    Id = nextIndex,
                    Trait = new CreatureTrait(keyval.Key),
                    Value = keyval.Value,
                    ValueMapId = traitValueMapId
                };
                grangerSimpleDb.TraitValues[newentity.Id] = newentity;
                nextIndex++;
            }

            grangerSimpleDb.Save();
            OnTraitValuesModified?.Invoke(this, new EventArgs());
        }

        internal void DeleteTraitValueMap(string traitValueMapId)
        {
            var entities = TraitValues.Where(x => x.ValueMapId == traitValueMapId).ToArray();
            if (entities.Length > 0)
            {
                foreach (var traitValueEntity in entities)
                {
                    grangerSimpleDb.TraitValues.Remove(traitValueEntity.Id);
                }
            }
            grangerSimpleDb.Save();
            OnTraitValuesModified?.Invoke(this, new EventArgs());
        }

        public void InsertCreature(CreatureEntity creature)
        {
            grangerSimpleDb.Creatures[creature.Id] = creature;
            grangerSimpleDb.Save();
            OnCreaturesModified?.Invoke(this, new EventArgs());
        }

        internal void SubmitChanges()
        {
            // Todo: refactor
            // Submit changes is no longer transactional, like in LinqToSql, 
            // however this method is preserved because of the published event.
            grangerSimpleDb.Save();
            OnCreaturesModified?.Invoke(this, new EventArgs());
        }

        internal void DeleteCreatures(CreatureEntity[] creatures)
        {
            foreach (var creatureEntity in creatures)
            {
                grangerSimpleDb.Creatures.Remove(creatureEntity.Id);
            }
            grangerSimpleDb.Save();
            OnCreaturesModified?.Invoke(this, new EventArgs());
        }

        internal CreatureColorEntity GetCreatureColor(string id)
        {
            return grangerSimpleDb.CreatureColors.TryGetByKey(id) ?? CreatureColorEntity.Unknown;
        }

        /// <summary>
        /// Adds the color if it does not yet exist in the database.
        /// </summary>
        /// <param name="entity"></param>
        internal void SeedCreatureColor([NotNull] CreatureColorEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            CreatureColorEntity existingEntity;
            if (!grangerSimpleDb.CreatureColors.TryGetValue(entity.Id, out existingEntity))
            {
                grangerSimpleDb.CreatureColors.Add(entity.Id, entity);
                OnCreatureColorsModified?.Invoke(this, new EventArgs());
            }
        }

        internal void AddOrUpdateCreatureColor([NotNull] CreatureColorEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            CreatureColorEntity existingEntity;
            if (!grangerSimpleDb.CreatureColors.TryGetValue(entity.Id, out existingEntity))
            {
                grangerSimpleDb.CreatureColors.Add(entity.Id, entity);
            }
            else
            {
                if (existingEntity.IsReadOnly)
                {
                    throw new InvalidOperationException($"Existing {nameof(CreatureColorEntity)} with Id: {entity.Id} is flagged ReadOnly.");
                }
                
                grangerSimpleDb.CreatureColors[entity.Id] = entity;
            }
            grangerSimpleDb.FlagAsChanged();
            OnCreatureColorsModified?.Invoke(this, new EventArgs());
        }

        internal void RemoveCreatureColor([NotNull] CreatureColorEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.IsReadOnly)
            {
                return;
            }
            grangerSimpleDb.CreatureColors.Remove(entity.Id);
            grangerSimpleDb.FlagAsChanged();
            OnCreatureColorsModified?.Invoke(this, new EventArgs());
        }
    }
}