using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer
{
    public class GrangerContext
    {
        readonly GrangerSimpleDb grangerSimpleDb;

        public GrangerContext([NotNull] GrangerSimpleDb grangerSimpleDb)
        {
            if (grangerSimpleDb == null) throw new ArgumentNullException("grangerSimpleDb");
            this.grangerSimpleDb = grangerSimpleDb;
        }

        public IEnumerable<CreatureEntity> Creatures
        {
            get { return grangerSimpleDb.Creatures.Values.ToArray(); }
        }

        public IEnumerable<TraitValueEntity> TraitValues
        {
            get { return grangerSimpleDb.TraitValues.Values.ToArray(); }
        }

        public IEnumerable<HerdEntity> Herds
        {
            get { return grangerSimpleDb.Herds.Values.ToArray(); }
        }

        public event EventHandler<EventArgs> OnHerdsModified;
        public event EventHandler<EventArgs> OnTraitValuesModified;
        public event EventHandler<EventArgs> OnEntitiesModified;

        #region HERD OPS

        /// <summary>
        /// throws DuplicateKeyException
        /// </summary>
        /// <param name="herdName"></param>
        public void InsertHerd(string herdName)
        {
            HerdEntity newHerd = new HerdEntity {HerdID = herdName};
            if (grangerSimpleDb.Herds.ContainsKey(herdName))
            {
                throw new ApplicationException("There already exists a herd with id " + herdName);
            }
            grangerSimpleDb.Herds[herdName] = newHerd;
            grangerSimpleDb.Save();
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        /// <summary>
        /// throws exception on errors
        /// </summary>
        /// <param name="renamingHerd"></param>
        /// <param name="newHerdName"></param>
        internal void RenameHerd(string renamingHerd, string newHerdName)
        {
            if (grangerSimpleDb.Herds.ContainsKey(newHerdName))
            {
                throw new ApplicationException("There already exists a herd with id " + newHerdName);
            }

            HerdEntity oldherd = Herds.Single(x => x.HerdID == renamingHerd);

            HerdEntity newHerd = oldherd.CloneMe(newHerdName);
            newHerd.HerdID = newHerdName;
            grangerSimpleDb.Herds[newHerdName] = newHerd;

            List<CreatureEntity> creaturesInThisHerd = Creatures.Where(x => x.Herd == renamingHerd).ToList();
            creaturesInThisHerd.ForEach(x => x.Herd = newHerdName);

            grangerSimpleDb.Herds.Remove(oldherd.HerdID);
            grangerSimpleDb.Save();
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        /// <summary>
        /// throws exception on errors
        /// </summary>
        /// <param name="herdName"></param>
        internal void DeleteHerd(string herdName)
        {
            HerdEntity herd = Herds.Single(x => x.HerdID == herdName);
            CreatureEntity[] creaturesInThisHerd = Creatures.Where(x => x.Herd == herdName).ToArray();
            foreach (var creatureEntity in creaturesInThisHerd)
            {
                grangerSimpleDb.Creatures.Remove(creatureEntity.Id);
            }
            grangerSimpleDb.Herds.Remove(herd.HerdID);
            grangerSimpleDb.Save();
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        public class DuplicateCreatureIdentityException : Exception
        {
            public DuplicateCreatureIdentityException(string message)
                : base(message)
            {
            }
        }

        /// <param name="sourceHerdName"></param>
        /// <param name="destinationHerdName"></param>
        /// <exception cref="DuplicateCreatureIdentityException"></exception>
        internal void MergeHerds(string sourceHerdName, string destinationHerdName)
        {
            HerdEntity sourceHerd = Herds.Single(x => x.HerdID == sourceHerdName);
            HerdEntity destinationHerd = Herds.Single(x => x.HerdID == destinationHerdName);

            CreatureEntity[] creaturesInSource = Creatures.Where(x => x.Herd == sourceHerd.HerdID).ToArray();
            CreatureEntity[] creaturesInDestination = Creatures.Where(x => x.Herd == destinationHerd.HerdID).ToArray();

            List<CreatureEntity> nonUniqueIdentityCreatures = new List<CreatureEntity>();
            foreach (var sourceCreatures in creaturesInSource)
            {
                foreach (var destinationCreature in creaturesInDestination)
                {
                    if (sourceCreatures.IsDifferentIdentityThan(destinationCreature) == false)
                        nonUniqueIdentityCreatures.Add(sourceCreatures);
                }
            }

            if (nonUniqueIdentityCreatures.Any())
            {
                throw new DuplicateCreatureIdentityException("target herd: "
                    + destinationHerd.HerdID
                    + " already contains creature(s) of same identity: "
                    + string.Join(", ", nonUniqueIdentityCreatures));
            }
            else
            {
                foreach (var creature in creaturesInSource)
                {
                    creature.Herd = destinationHerd.HerdID;
                }
                grangerSimpleDb.Herds.Remove(sourceHerd.HerdID);
                grangerSimpleDb.Save();
                if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
            }
        }

        internal void UpdateHerdSelectedState(string herdID, bool newState)
        {
            HerdEntity entity = Herds.Single(x => x.HerdID == herdID);
            entity.Selected = newState;
            grangerSimpleDb.Save();
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        #endregion

        internal void UpdateOrCreateTraitValueMap(Dictionary<CreatureTrait.TraitEnum, int> valueMap, string traitValueMapId)
        {
            var entities = TraitValues.Where(x => x.ValueMapID == traitValueMapId).ToArray();
            if (entities.Length > 0)
            {
                foreach (var traitValueEntity in entities)
                {
                    grangerSimpleDb.TraitValues.Remove(traitValueEntity.Id);
                }
            }

            int nextIndex = TraitValueEntity.GenerateNewTraitValueID(this);
            foreach (var keyval in valueMap)
            {
                TraitValueEntity newentity = new TraitValueEntity()
                {
                    Id = nextIndex,
                    Trait = new CreatureTrait(keyval.Key),
                    Value = keyval.Value,
                    ValueMapID = traitValueMapId
                };
                grangerSimpleDb.TraitValues[newentity.Id] = newentity;
                nextIndex++;
            }

            grangerSimpleDb.Save();
            if (OnTraitValuesModified != null) OnTraitValuesModified(this, new EventArgs());
        }

        internal void DeleteTraitValueMap(string traitValueMapId)
        {
            var entities = TraitValues.Where(x => x.ValueMapID == traitValueMapId).ToArray();
            if (entities.Length > 0)
            {
                foreach (var traitValueEntity in entities)
                {
                    grangerSimpleDb.TraitValues.Remove(traitValueEntity.Id);
                }
            }
            grangerSimpleDb.Save();
            if (OnTraitValuesModified != null) OnTraitValuesModified(this, new EventArgs());
        }

        public void InsertCreature(CreatureEntity creature)
        {
            grangerSimpleDb.Creatures[creature.Id] = creature;
            grangerSimpleDb.Save();
            if (OnEntitiesModified != null) OnEntitiesModified(this, new EventArgs());
        }

        internal void SubmitChanges()
        {
            // Creatures are updated directly now, but we still need this method for event
            grangerSimpleDb.Save();
            if (OnEntitiesModified != null) OnEntitiesModified(this, new EventArgs());
        }

        internal void DeleteCreatures(CreatureEntity[] creatures)
        {
            foreach (var creatureEntity in creatures)
            {
                grangerSimpleDb.Creatures.Remove(creatureEntity.Id);
            }
            grangerSimpleDb.Save();
            if (OnEntitiesModified != null) OnEntitiesModified(this, new EventArgs());
        }
    }
}