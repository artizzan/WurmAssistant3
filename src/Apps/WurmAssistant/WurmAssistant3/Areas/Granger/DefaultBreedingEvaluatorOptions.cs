using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.PersistentObjects;
using AldursLab.WurmAssistant3.Areas.Granger.Advisor.Default;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger
{
    [KernelBind(BindingHint.Singleton), PersistentObject("GrangerFeature_DefaultBreedingEvaluatorOptions")]
    public class DefaultBreedingEvaluatorOptions
    {
        [NotNull] readonly DefaultBreedingEvaluatorOptionsData defaultBreedingEvaluatorOptionsData;
        [NotNull] readonly CreatureColorDefinitions creatureColorDefinitions;

        public DefaultBreedingEvaluatorOptions([NotNull] DefaultBreedingEvaluatorOptionsData defaultBreedingEvaluatorOptionsData, [NotNull] CreatureColorDefinitions creatureColorDefinitions)
        {
            if (creatureColorDefinitions == null) throw new ArgumentNullException(nameof(creatureColorDefinitions));
            if (defaultBreedingEvaluatorOptionsData == null) throw new ArgumentNullException(nameof(defaultBreedingEvaluatorOptionsData));
            this.creatureColorDefinitions = creatureColorDefinitions;
            this.defaultBreedingEvaluatorOptionsData = defaultBreedingEvaluatorOptionsData;

            SetupColorValuesDict();
        }

        void SetupColorValuesDict()
        {
            foreach (var currentColor in creatureColorDefinitions.GetColors())
            {
                if (!CreatureColorValuesForEntityId.ContainsKey(currentColor.CreatureColorId))
                {
                    CreatureColorValuesForEntityId.Add(currentColor.CreatureColorId, 1.0f);
                }
            }
        }

        public bool IgnoreNotInMood
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreNotInMood;
            set => defaultBreedingEvaluatorOptionsData.IgnoreNotInMood = value;
        }

        public bool IgnorePregnant
        {
            get => defaultBreedingEvaluatorOptionsData.IgnorePregnant;
            set => defaultBreedingEvaluatorOptionsData.IgnorePregnant = value;
        }

        public bool IgnoreRecentlyPregnant
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreRecentlyPregnant;
            set => defaultBreedingEvaluatorOptionsData.IgnoreRecentlyPregnant = value;
        }

        public bool IgnoreOtherHerds
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreOtherHerds;
            set => defaultBreedingEvaluatorOptionsData.IgnoreOtherHerds = value;
        }

        public bool IgnorePairedCreatures
        {
            get => defaultBreedingEvaluatorOptionsData.IgnorePairedCreatures;
            set => defaultBreedingEvaluatorOptionsData.IgnorePairedCreatures = value;
        }

        public bool IgnoreSold
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreSold;
            set => defaultBreedingEvaluatorOptionsData.IgnoreSold = value;
        }

        public bool IgnoreDead
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreDead;
            set => defaultBreedingEvaluatorOptionsData.IgnoreDead = value;
        }

        public bool IgnoreFoals
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreFoals;
            set => defaultBreedingEvaluatorOptionsData.IgnoreFoals = value;
        }

        public bool IgnoreYoung
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreYoung;
            set => defaultBreedingEvaluatorOptionsData.IgnoreYoung = value;
        }

        public bool IgnoreAdolescent
        {
            get => defaultBreedingEvaluatorOptionsData.IgnoreAdolescent;
            set => defaultBreedingEvaluatorOptionsData.IgnoreAdolescent = value;
        }

        public bool AgeIgnoreOnlyOtherCreatures
        {
            get => defaultBreedingEvaluatorOptionsData.AgeIgnoreOnlyOtherCreatures;
            set => defaultBreedingEvaluatorOptionsData.AgeIgnoreOnlyOtherCreatures = value;
        }

        public bool PreferUniqueTraits
        {
            get => defaultBreedingEvaluatorOptionsData.PreferUniqueTraits;
            set => defaultBreedingEvaluatorOptionsData.PreferUniqueTraits = value;
        }

        public bool DiscardOnInbreeding
        {
            get => defaultBreedingEvaluatorOptionsData.DiscardOnInbreeding;
            set => defaultBreedingEvaluatorOptionsData.DiscardOnInbreeding = value;
        }

        public bool ExcludeExactAgeEnabled
        {
            get => defaultBreedingEvaluatorOptionsData.ExcludeExactAgeEnabled;
            set => defaultBreedingEvaluatorOptionsData.ExcludeExactAgeEnabled = value;
        }

        public TimeSpan ExcludeExactAgeValue
        {
            get => defaultBreedingEvaluatorOptionsData.ExcludeExactAgeValue;
            set => defaultBreedingEvaluatorOptionsData.ExcludeExactAgeValue = value;
        }

        public bool DiscardOnAnyNegativeTraits
        {
            get => defaultBreedingEvaluatorOptionsData.DiscardOnAnyNegativeTraits;
            set => defaultBreedingEvaluatorOptionsData.DiscardOnAnyNegativeTraits = value;
        }

        public double UniqueTraitWeight
        {
            get => defaultBreedingEvaluatorOptionsData.UniqueTraitWeight;
            set => defaultBreedingEvaluatorOptionsData.UniqueTraitWeight = value;
        }

        public double BadTraitWeight
        {
            get => defaultBreedingEvaluatorOptionsData.BadTraitWeight;
            set => defaultBreedingEvaluatorOptionsData.BadTraitWeight = value;
        }

        public double InbreedingPenaltyWeight
        {
            get => defaultBreedingEvaluatorOptionsData.InbreedingPenaltyWeight;
            set => defaultBreedingEvaluatorOptionsData.InbreedingPenaltyWeight = value;
        }

        public Dictionary<string, float> CreatureColorValuesForEntityId =>  defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId;

        public IEnumerable<ColorWeight> CreatureColorValues
        {
            get
            {
                SetupColorValuesDict();
                return
                    defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId
                        .Where(x => creatureColorDefinitions.Exists(x.Key))
                        .Select(x => new ColorWeight(creatureColorDefinitions.GetForId(x.Key), x.Value))
                        .ToArray();
            }
            set
            {
                foreach (var colorWeight in value)
                {
                    defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId[colorWeight.Color.CreatureColorId] = colorWeight.Weight;
                }
                defaultBreedingEvaluatorOptionsData.FlagAsChanged();
            }
        }

        public float GetValueForColor(CreatureColor color)
        {
            float result = 1.0f;
            defaultBreedingEvaluatorOptionsData.CreatureColorValuesForEntityId.TryGetValue(color.CreatureColorId, out result);
            return result;
        }
    }
}