using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    public class GrangerContext
    {
        readonly GrangerSimpleDb grangerSimpleDb;

        public GrangerContext([NotNull] GrangerSimpleDb grangerSimpleDb)
        {
            if (grangerSimpleDb == null) throw new ArgumentNullException("grangerSimpleDb");
            this.grangerSimpleDb = grangerSimpleDb;
        }

        public IEnumerable<HorseEntity> Horses
        {
            get { return grangerSimpleDb.Horses.Values.ToArray(); }
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
        public event EventHandler<EventArgs> OnHorsesModified;

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

            List<HorseEntity> horsesInThisHerd = Horses.Where(x => x.Herd == renamingHerd).ToList();
            horsesInThisHerd.ForEach(x => x.Herd = newHerdName);

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
            HorseEntity[] horsesInThisHerd = Horses.Where(x => x.Herd == herdName).ToArray();
            foreach (var horseEntity in horsesInThisHerd)
            {
                grangerSimpleDb.Horses.Remove(horseEntity.ID);
            }
            grangerSimpleDb.Herds.Remove(herd.HerdID);
            grangerSimpleDb.Save();
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        public class DuplicateHorseIdentityException : Exception
        {
            public DuplicateHorseIdentityException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// throws DuplicateHorseIdentityException
        /// </summary>
        /// <param name="sourceHerd"></param>
        /// <param name="destinationHerd"></param>
        internal void MergeHerds(string sourceHerdName, string destinationHerdName)
        {
            HerdEntity sourceHerd = Herds.Single(x => x.HerdID == sourceHerdName);
            HerdEntity destinationHerd = Herds.Single(x => x.HerdID == destinationHerdName);

            HorseEntity[] horsesInSource = Horses.Where(x => x.Herd == sourceHerd.HerdID).ToArray();
            HorseEntity[] horsesInDestination = Horses.Where(x => x.Herd == destinationHerd.HerdID).ToArray();

            List<HorseEntity> nonUniqueIdentityHorses = new List<HorseEntity>();
            foreach (var sourcehorse in horsesInSource)
            {
                foreach (var destinationhorse in horsesInDestination)
                {
                    if (sourcehorse.IsDifferentIdentityThan(destinationhorse) == false)
                        nonUniqueIdentityHorses.Add(sourcehorse);
                }
            }

            if (nonUniqueIdentityHorses.Any())
            {
                throw new DuplicateHorseIdentityException("target herd: "
                    + destinationHerd.HerdID
                    + " already contains horse(s) of same identity: "
                    + string.Join(", ", nonUniqueIdentityHorses));
            }
            else
            {
                foreach (var horse in horsesInSource)
                {
                    horse.Herd = destinationHerd.HerdID;
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

        internal void UpdateOrCreateTraitValueMap(Dictionary<HorseTrait.TraitEnum, int> valueMap, string traitValueMapID)
        {
            var entities = TraitValues.Where(x => x.ValueMapID == traitValueMapID).ToArray();
            if (entities.Length > 0)
            {
                foreach (var traitValueEntity in entities)
                {
                    grangerSimpleDb.TraitValues.Remove(traitValueEntity.ID);
                }
            }

            int nextIndex = TraitValueEntity.GenerateNewTraitValueID(this);
            foreach (var keyval in valueMap)
            {
                TraitValueEntity newentity = new TraitValueEntity()
                {
                    ID = nextIndex,
                    Trait = new HorseTrait(keyval.Key),
                    Value = keyval.Value,
                    ValueMapID = traitValueMapID
                };
                grangerSimpleDb.TraitValues[newentity.ID] = newentity;
                nextIndex++;
            }

            grangerSimpleDb.Save();
            if (OnTraitValuesModified != null) OnTraitValuesModified(this, new EventArgs());
        }

        internal void DeleteTraitValueMap(string traitValueMapID)
        {
            var entities = TraitValues.Where(x => x.ValueMapID == traitValueMapID).ToArray();
            if (entities.Length > 0)
            {
                foreach (var traitValueEntity in entities)
                {
                    grangerSimpleDb.TraitValues.Remove(traitValueEntity.ID);
                }
            }
            grangerSimpleDb.Save();
            if (OnTraitValuesModified != null) OnTraitValuesModified(this, new EventArgs());
        }

        /// <summary>
        /// throws DuplicateKeyException
        /// </summary>
        /// <param name="horse"></param>
        public void InsertHorse(HorseEntity horse)
        {
            grangerSimpleDb.Horses[horse.ID] = horse;
            grangerSimpleDb.Save();
            if (OnHorsesModified != null) OnHorsesModified(this, new EventArgs());
        }

        internal void SubmitChangesToHorses()
        {
            // Horses are updated directly now, but we still need this method for event
            grangerSimpleDb.Save();
            if (OnHorsesModified != null) OnHorsesModified(this, new EventArgs());
        }

        internal void DeleteHorses(HorseEntity[] horses)
        {
            foreach (var horseEntity in horses)
            {
                grangerSimpleDb.Horses.Remove(horseEntity.ID);
            }
            grangerSimpleDb.Save();
            if (OnHorsesModified != null) OnHorsesModified(this, new EventArgs());
        }
    }
}