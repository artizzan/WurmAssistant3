using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy.DBlayer
{
    public class GrangerContext : DataContext
    {
        public GrangerContext(IDbConnection connection)
            : base(connection)
        {
        }

        public Table<HorseEntity> Horses
        {
            get { return GetTable<HorseEntity>(); }
        }

        public Table<TraitValueEntity> TraitValues
        {
            get { return GetTable<TraitValueEntity>(); }
        }

        public Table<HerdEntity> Herds
        {
            get { return GetTable<HerdEntity>(); }
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
            HerdEntity newHerd = new HerdEntity();
            newHerd.HerdID = herdName;
            Herds.InsertOnSubmit(newHerd);
            SubmitChanges(); //DuplicateKeyException if duplicate key
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        /// <summary>
        /// throws exception on errors
        /// </summary>
        /// <param name="renamingHerd"></param>
        /// <param name="newHerdName"></param>
        internal void RenameHerd(string renamingHerd, string newHerdName)
        {
            HerdEntity oldherd = Herds.Where(x => x.HerdID == renamingHerd).Single();

            HerdEntity newHerd = oldherd.CloneMe(newHerdName);
            newHerd.HerdID = newHerdName;
            Herds.InsertOnSubmit(newHerd);

            List<HorseEntity> horsesInThisHerd = Horses.Where(x => x.Herd == renamingHerd).ToList();
            horsesInThisHerd.ForEach(x => x.Herd = newHerdName);

            Herds.DeleteOnSubmit(oldherd);

            SubmitChanges(); //exception?
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        /// <summary>
        /// throws exception on errors
        /// </summary>
        /// <param name="herdName"></param>
        internal void DeleteHerd(string herdName)
        {
            HerdEntity herd = Herds.Where(x => x.HerdID == herdName).Single();
            Herds.DeleteOnSubmit(herd);
            HorseEntity[] horsesInThisHerd = Horses.Where(x => x.Herd == herdName).ToArray();
            Horses.DeleteAllOnSubmit(horsesInThisHerd);
            SubmitChanges(); //exception?
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
            HerdEntity sourceHerd = Herds.Where(x => x.HerdID == sourceHerdName).Single();
            HerdEntity destinationHerd = Herds.Where(x => x.HerdID == destinationHerdName).Single();

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

            if (nonUniqueIdentityHorses.Count() > 0)
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
                Herds.DeleteOnSubmit(sourceHerd);
                SubmitChanges();
                if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
            }
        }

        internal void UpdateHerdSelectedState(string herdID, bool newState)
        {
            HerdEntity entity = Herds.Where(x => x.HerdID == herdID).Single();
            entity.Selected = newState;
            SubmitChanges();
            if (OnHerdsModified != null) OnHerdsModified(this, new EventArgs());
        }

        #endregion

        internal void UpdateOrCreateTraitValueMap(Dictionary<HorseTrait.TraitEnum, int> valueMap, string traitValueMapID)
        {
            var entities = TraitValues.Where(x => x.ValueMapID == traitValueMapID).ToArray();
            if (entities.Length > 0) TraitValues.DeleteAllOnSubmit(entities);

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
                TraitValues.InsertOnSubmit(newentity);
                nextIndex++;
            }

            SubmitChanges();
            if (OnTraitValuesModified != null) OnTraitValuesModified(this, new EventArgs());
        }

        internal void DeleteTraitValueMap(string traitValueMapID)
        {
            var entities = TraitValues.Where(x => x.ValueMapID == traitValueMapID).ToArray();
            if (entities.Length > 0) TraitValues.DeleteAllOnSubmit(entities);
            SubmitChanges();
            if (OnTraitValuesModified != null) OnTraitValuesModified(this, new EventArgs());
        }

        /// <summary>
        /// throws DuplicateKeyException
        /// </summary>
        /// <param name="herdName"></param>
        public void InsertHorse(HorseEntity horse)
        {
            Horses.InsertOnSubmit(horse);
            SubmitChanges(); //DuplicateKeyException if duplicate key
            if (OnHorsesModified != null) OnHorsesModified(this, new EventArgs());
        }

        internal void SubmitChangesToHorses()
        {
            SubmitChanges();
            if (OnHorsesModified != null) OnHorsesModified(this, new EventArgs());
        }

        internal void DeleteHorses(HorseEntity[] horses)
        {
            Horses.DeleteAllOnSubmit(horses);
            SubmitChanges();
            if (OnHorsesModified != null) OnHorsesModified(this, new EventArgs());
        }
    }
}