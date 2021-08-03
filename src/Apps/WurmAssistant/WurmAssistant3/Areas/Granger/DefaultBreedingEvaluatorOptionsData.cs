using System;
using System.Collections.Generic;
using AldursLab.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [KernelBind(BindingHint.Singleton), PersistentObject("GrangerFeature_DefaultBreedingEvaluatorOptions")]
    public class DefaultBreedingEvaluatorOptionsData : PersistentObjectBase
    {
        [JsonProperty]
        bool ignoreNotInMood;

        [JsonProperty]
        bool ignorePregnant;

        [JsonProperty]
        bool ignoreRecentlyPregnant;

        [JsonProperty]
        bool ignoreOtherHerds;

        [JsonProperty]
        bool ignorePairedCreatures;

        [JsonProperty]
        bool ignoreSold;

        [JsonProperty]
        bool ignoreDead;

        [JsonProperty]
        bool ignoreFoals;

        [JsonProperty]
        bool ignoreYoung;

        [JsonProperty]
        bool ignoreAdolescent;

        [JsonProperty]
        bool ageIgnoreOnlyOtherCreatures;

        [JsonProperty]
        bool preferUniqueTraits;

        [JsonProperty]
        bool discardOnInbreeding;

        [JsonProperty]
        int numPotentialTraitsToConsider;

        [JsonProperty]
        bool excludeExactAgeEnabled;

        [JsonProperty]
        TimeSpan excludeExactAgeValue;

        [JsonProperty]
        bool discardOnAnyNegativeTraits;

        [JsonProperty]
        double uniqueTraitWeight;

        [JsonProperty]
        double negativeTraitPenaltyWeight;

        [JsonProperty]
        double inbreedingPenaltyWeight;

        [JsonProperty]
        readonly Dictionary<string, float> creatureColorValuesForEntityId;

        public DefaultBreedingEvaluatorOptionsData()
        {
            IgnoreNotInMood = true;
            IgnorePregnant = true;
            IgnoreRecentlyPregnant = true;
            IgnoreOtherHerds = false;
            IgnorePairedCreatures = false;
            IgnoreSold = false;

            IgnoreFoals = true;
            IgnoreYoung = true;
            IgnoreAdolescent = false;

            AgeIgnoreOnlyOtherCreatures = false;

            PreferUniqueTraits = false;
            UniqueTraitWeight = 3.0;

            DiscardOnAnyNegativeTraits = false;
            BadTraitWeight = 1.0;

            DiscardOnInbreeding = true;
            InbreedingPenaltyWeight = 1.0;

            creatureColorValuesForEntityId = new Dictionary<string, float>();
        }

        public bool IgnoreNotInMood
        {
            get { return ignoreNotInMood; }
            set { ignoreNotInMood = value; FlagAsChanged(); }
        }

        public bool IgnorePregnant
        {
            get { return ignorePregnant; }
            set { ignorePregnant = value; FlagAsChanged(); }
        }

        public bool IgnoreRecentlyPregnant
        {
            get { return ignoreRecentlyPregnant; }
            set { ignoreRecentlyPregnant = value; FlagAsChanged(); }
        }

        public bool IgnoreOtherHerds
        {
            get { return ignoreOtherHerds; }
            set { ignoreOtherHerds = value; FlagAsChanged(); }
        }

        public bool IgnorePairedCreatures
        {
            get { return ignorePairedCreatures; }
            set { ignorePairedCreatures = value; FlagAsChanged(); }
        }

        public bool IgnoreSold
        {
            get { return ignoreSold; }
            set { ignoreSold = value; FlagAsChanged(); }
        }

        public bool IgnoreDead
        {
            get { return ignoreDead; }
            set { ignoreDead = value; FlagAsChanged(); }
        }

        public bool IgnoreFoals
        {
            get { return ignoreFoals; }
            set { ignoreFoals = value; FlagAsChanged(); }
        }

        public bool IgnoreYoung
        {
            get { return ignoreYoung; }
            set { ignoreYoung = value; FlagAsChanged(); }
        }

        public bool IgnoreAdolescent
        {
            get { return ignoreAdolescent; }
            set { ignoreAdolescent = value; FlagAsChanged(); }
        }

        public bool AgeIgnoreOnlyOtherCreatures
        {
            get { return ageIgnoreOnlyOtherCreatures; }
            set { ageIgnoreOnlyOtherCreatures = value; FlagAsChanged(); }
        }

        public bool PreferUniqueTraits
        {
            get { return preferUniqueTraits; }
            set { preferUniqueTraits = value; FlagAsChanged(); }
        }

        public bool DiscardOnInbreeding
        {
            get { return discardOnInbreeding; }
            set { discardOnInbreeding = value; FlagAsChanged(); }
        }

        public int NumPotentialTraitsToConsider
        {
            get { return numPotentialTraitsToConsider; }
            set { numPotentialTraitsToConsider = value; FlagAsChanged(); }
        }

        public bool ExcludeExactAgeEnabled
        {
            get { return excludeExactAgeEnabled; }
            set { excludeExactAgeEnabled = value; FlagAsChanged(); }
        }

        public TimeSpan ExcludeExactAgeValue
        {
            get { return excludeExactAgeValue; }
            set { excludeExactAgeValue = value; FlagAsChanged(); }
        }

        public bool DiscardOnAnyNegativeTraits
        {
            get { return discardOnAnyNegativeTraits; }
            set { discardOnAnyNegativeTraits = value; FlagAsChanged(); }
        }

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

        public Dictionary<string, float> CreatureColorValuesForEntityId => creatureColorValuesForEntityId;
    }
}