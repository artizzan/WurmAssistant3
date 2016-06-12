using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Granger.Parts;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.Advisor.Default;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger.Services
{
    [KernelHint(BindingHint.Singleton), PersistentObject("GrangerFeature_DefaultBreedingEvaluatorOptions")]
    public class DefaultBreedingEvaluatorOptions : PersistentObjectBase
    {
        public DefaultBreedingEvaluatorOptions()
        {
            ignoreNotInMood = true;
            ignorePregnant = true;
            IgnoreRecentlyPregnant = true;
            IgnoreOtherHerds = false;
            IgnorePairedCreatures = false;
            IgnoreSold = false;

            IgnoreFoals = true;
            IgnoreYoung = true;
            IgnoreAdolescent = false;

            AgeIgnoreOnlyOtherCreatures = false;

            IncludePotentialValue = false;
            PotentialValuePositiveWeight = 1.0;
            PotentialValueNegativeWeight = 1.0;

            PreferUniqueTraits = false;
            UniqueTraitWeight = 3.0;

            DiscardOnAnyNegativeTraits = false;
            BadTraitWeight = 1.0;

            DiscardOnInbreeding = true;
            InbreedingPenaltyWeight = 1.0;

            creatureColorValues = new Dictionary<CreatureColorId, float>();
        }

        protected override void OnPersistentDataLoaded()
        {
            BuildInitialColorValuesDict();
        }

        void BuildInitialColorValuesDict()
        {
            foreach (var currentColor in CreatureColor.GetAll())
            {
                if (!creatureColorValues.ContainsKey(currentColor.CreatureColorId))
                {
                    creatureColorValues.Add(currentColor.CreatureColorId, 1.0f);
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
        bool ignorePairedCreatures;

        public bool IgnorePairedCreatures
        {
            get { return ignorePairedCreatures; }
            set { ignorePairedCreatures = value; FlagAsChanged(); }
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
        bool ageIgnoreOnlyOtherCreatures;

        public bool AgeIgnoreOnlyOtherCreatures
        {
            get { return ageIgnoreOnlyOtherCreatures; }
            set { ageIgnoreOnlyOtherCreatures = value; FlagAsChanged(); }
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
        readonly Dictionary<CreatureColorId, float> creatureColorValues;

        public IEnumerable<ColorWeight> CreatureColorValues
        {
            get { return creatureColorValues.Select(x => new ColorWeight(new CreatureColor(x.Key), x.Value)).ToArray(); }
            set
            {
                foreach (var colorWeight in value)
                {
                    creatureColorValues[colorWeight.Color.CreatureColorId] = colorWeight.Weight;
                }
                FlagAsChanged();
            }
        }

        public float GetValueForColor(CreatureColor color)
        {
            float result = 1.0f;
            creatureColorValues.TryGetValue(color.CreatureColorId, out result);
            return result;
        }
    }
}