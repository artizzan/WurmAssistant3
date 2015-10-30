using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.Advisor.Default
{
    [PersistentObject("GrangerFeature_DefaultBreedingEvaluatorOptions")]
    public class DefaultBreedingEvaluatorOptions : PersistentObjectBase
    {
        public DefaultBreedingEvaluatorOptions()
        {
            ignoreNotInMood = true;
            ignorePregnant = true;
            IgnoreRecentlyPregnant = true;
            IgnoreOtherHerds = false;
            IgnorePairedHorses = false;
            IgnoreSold = false;

            IgnoreFoals = true;
            IgnoreYoung = true;
            IgnoreAdolescent = false;

            AgeIgnoreOnlyOtherHorses = false;

            IncludePotentialValue = false;
            PotentialValuePositiveWeight = 1.0;
            PotentialValueNegativeWeight = 1.0;

            PreferUniqueTraits = false;
            UniqueTraitWeight = 3.0;

            DiscardOnAnyNegativeTraits = false;
            BadTraitWeight = 1.0;

            DiscardOnInbreeding = true;
            InbreedingPenaltyWeight = 1.0;

            horseColorValues = new Dictionary<HorseColorId, float>();
        }

        protected override void OnPersistentDataLoaded()
        {
            BuildInitialColorValuesDict();
        }

        void BuildInitialColorValuesDict()
        {
            foreach (var currentColor in HorseColor.GetAll())
            {
                if (!horseColorValues.ContainsKey(currentColor.HorseColorId))
                {
                    horseColorValues.Add(currentColor.HorseColorId, 1.0f);
                }
            }
        }

        [JsonProperty]
        bool ignoreNotInMood;

        public bool IgnoreNotInMood
        {
            get { return ignoreNotInMood; }
            set { ignoreNotInMood = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignorePregnant;

        public bool IgnorePregnant
        {
            get { return ignorePregnant; }
            set { ignorePregnant = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreRecentlyPregnant;

        public bool IgnoreRecentlyPregnant
        {
            get { return ignoreRecentlyPregnant; }
            set { ignoreRecentlyPregnant = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreOtherHerds;

        public bool IgnoreOtherHerds
        {
            get { return ignoreOtherHerds; }
            set { ignoreOtherHerds = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignorePairedHorses;

        public bool IgnorePairedHorses
        {
            get { return ignorePairedHorses; }
            set { ignorePairedHorses = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreSold;

        public bool IgnoreSold
        {
            get { return ignoreSold; }
            set { ignoreSold = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreDead;

        public bool IgnoreDead
        {
            get { return ignoreDead; }
            set { ignoreDead = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreFoals;

        public bool IgnoreFoals
        {
            get { return ignoreFoals; }
            set { ignoreFoals = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreYoung;

        public bool IgnoreYoung
        {
            get { return ignoreYoung; }
            set { ignoreYoung = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ignoreAdolescent;

        public bool IgnoreAdolescent
        {
            get { return ignoreAdolescent; }
            set { ignoreAdolescent = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool ageIgnoreOnlyOtherHorses;

        public bool AgeIgnoreOnlyOtherHorses
        {
            get { return ageIgnoreOnlyOtherHorses; }
            set { ageIgnoreOnlyOtherHorses = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool includePotentialValue;

        public bool IncludePotentialValue
        {
            get { return includePotentialValue; }
            set { includePotentialValue = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool preferUniqueTraits;

        public bool PreferUniqueTraits
        {
            get { return preferUniqueTraits; }
            set { preferUniqueTraits = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool discardOnInbreeding;

        public bool DiscardOnInbreeding
        {
            get { return discardOnInbreeding; }
            set { discardOnInbreeding = value; FlagAsChanged(); }
        }

        [JsonProperty] 
        int numPotentialTraitsToConsider;

        public int NumPotentialTraitsToConsider
        {
            get { return numPotentialTraitsToConsider; }
            set { numPotentialTraitsToConsider = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool excludeExactAgeEnabled;

        public bool ExcludeExactAgeEnabled
        {
            get { return excludeExactAgeEnabled; }
            set { excludeExactAgeEnabled = value; FlagAsChanged(); }
        }

        [JsonProperty]
        TimeSpan excludeExactAgeValue;

        public TimeSpan ExcludeExactAgeValue
        {
            get { return excludeExactAgeValue; }
            set { excludeExactAgeValue = value; FlagAsChanged(); }
        }

        [JsonProperty]
        bool discardOnAnyNegativeTraits;

        public bool DiscardOnAnyNegativeTraits
        {
            get { return discardOnAnyNegativeTraits; }
            set { discardOnAnyNegativeTraits = value; FlagAsChanged(); }
        }

        [JsonProperty]
        double potentialValuePositiveWeight;

        public double PotentialValuePositiveWeight
        {
            get { return potentialValuePositiveWeight; }
            set
            {
                if (potentialValuePositiveWeight < 0) potentialValuePositiveWeight = 0;
                else potentialValuePositiveWeight = value; 
                FlagAsChanged();
            }
        }

        [JsonProperty]
        double potentialValueNegativeWeight;

        public double PotentialValueNegativeWeight
        {
            get { return potentialValueNegativeWeight; }
            set
            {
                if (potentialValueNegativeWeight < 0) potentialValueNegativeWeight = 0;
                else potentialValueNegativeWeight = value;
                FlagAsChanged();
            }
        }

        [JsonProperty]
        double uniqueTraitWeight;

        public double UniqueTraitWeight
        {
            get { return uniqueTraitWeight; }
            set
            {
                if (uniqueTraitWeight < 0) uniqueTraitWeight = 0;
                else uniqueTraitWeight = value;
                FlagAsChanged();
            }
        }


        [JsonProperty]
        double negativeTraitPenaltyWeight;

        public double BadTraitWeight
        {
            get { return negativeTraitPenaltyWeight; }
            set
            {
                if (negativeTraitPenaltyWeight < 0) negativeTraitPenaltyWeight = 0;
                else negativeTraitPenaltyWeight = value;
                FlagAsChanged();
            }
        }

        [JsonProperty]
        double inbreedingPenaltyWeight;

        public double InbreedingPenaltyWeight
        {
            get { return inbreedingPenaltyWeight; }
            set
            {
                if (inbreedingPenaltyWeight < 0) inbreedingPenaltyWeight = 0;
                else inbreedingPenaltyWeight = value;
                FlagAsChanged();
            }
        }

        [JsonProperty]
        readonly Dictionary<HorseColorId, float> horseColorValues;

        public IEnumerable<ColorWeight> HorseColorValues
        {
            get { return horseColorValues.Select(x => new ColorWeight(new HorseColor(x.Key), x.Value)).ToArray(); }
            set
            {
                foreach (var colorWeight in value)
                {
                    horseColorValues[colorWeight.Color.HorseColorId] = colorWeight.Weight;
                }
                FlagAsChanged();
            }
        }

        public float GetValueForColor(HorseColor color)
        {
            float result = 1.0f;
            horseColorValues.TryGetValue(color.HorseColorId, out result);
            return result;
        }
    }
}