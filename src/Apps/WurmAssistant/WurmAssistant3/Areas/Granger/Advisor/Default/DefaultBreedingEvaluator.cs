using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmAssistant3.Areas.Granger.ValuePreset;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Advisor.Default
{
    public class DefaultBreedingEvaluator : BreedingEvaluator
    {
        static readonly System.Drawing.Color? DefaultIgnoredHintColor = (System.Drawing.Color)(new HslColor(0D, 0D, 210D));
        static readonly System.Drawing.Color? DefaultExcludedHintColor = (System.Drawing.Color)(new HslColor(210D, 240D, 0D));

        readonly ILogger logger;

        readonly DefaultBreedingEvaluatorOptions options;

        public DefaultBreedingEvaluator(
            [NotNull] ILogger logger, 
            [NotNull] DefaultBreedingEvaluatorOptions options)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (options == null) throw new ArgumentNullException(nameof(options));
            this.logger = logger;
            this.options = options;
        }

        public override bool EditOptions(FormGrangerMain formGrangerMain)
        {
            BreedingEvaluatorDefaultConfig ui = new BreedingEvaluatorDefaultConfig(options, logger);
            return ui.ShowDialogCenteredOnForm(formGrangerMain) == DialogResult.OK;
        }

        CreatureTrait[] GetUniqueTraits(CreatureTrait[] allPresentTraits, CreatureTrait[] traitGroup1, CreatureTrait[] traitGroup2)
        {
            List<CreatureTrait> uniqueTraits = new List<CreatureTrait>();
            foreach (var trait in allPresentTraits)
            {
                if (!(traitGroup1.Contains(trait) && traitGroup2.Contains(trait)))
                    uniqueTraits.Add(trait);
            }
            return uniqueTraits.ToArray();
        }

        public override BreedingEvalResults? Evaluate(Creature creature1, Creature creature2, TraitValuator valuator)
        {
            if (creature1 == creature2) return null;

            BreedingEvalResults results = new BreedingEvalResults();

            var allPossibleTraits = CreatureTrait.GetAllPossibleTraits();
            var traits1 = creature1.Traits;
            var traits2 = creature2.Traits;
            var concatTraits = traits1.Concat<CreatureTrait>(traits2).ToArray();
            var distinctConcatTraits = concatTraits.Distinct().ToArray();

            if (creature1.IsMale == creature2.IsMale)
            {
                results.Ignored = true;
            }

            if (options.IgnoreNotInMood)
            {
                if (creature1.NotInMood || creature2.NotInMood)
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnorePregnant)
            {
                if (creature1.Pregnant || creature2.Pregnant)
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnoreRecentlyPregnant)
            {
                if (creature1.PregnantInLast24H || creature2.PregnantInLast24H)
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnoreOtherHerds)
            {
                if (creature1.Herd != creature2.Herd)
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnorePairedCreatures)
            {
                if (creature1.HasMate() || creature2.HasMate())
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnoreSold)
            {
                if (creature1.CheckTag("sold") || creature2.CheckTag("sold"))
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnoreDead)
            {
                if (creature1.CheckTag("dead") || creature2.CheckTag("dead"))
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnoreFoals)
                if ((creature1.IsFoal() && !options.AgeIgnoreOnlyOtherCreatures) ||
                    creature2.IsFoal())
                    results.Ignored = true;

            if (options.IgnoreYoung)
            {
                if (((creature1.Age.CreatureAgeId == CreatureAgeId.Young) && !options.AgeIgnoreOnlyOtherCreatures) ||
                    creature2.Age.CreatureAgeId == CreatureAgeId.Young)
                {
                    results.Ignored = true;
                }
            }

            if (options.IgnoreAdolescent)
            {
                if (((creature1.Age.CreatureAgeId == CreatureAgeId.Adolescent) && !options.AgeIgnoreOnlyOtherCreatures)
                    ||
                    creature2.Age.CreatureAgeId == CreatureAgeId.Adolescent)
                {
                    results.Ignored = true;
                }
            }

            if (options.ExcludeExactAgeEnabled)
            {

                if (DateTime.Now - creature1.BirthDate < options.ExcludeExactAgeValue ||
                    DateTime.Now - creature2.BirthDate < options.ExcludeExactAgeValue)
                {
                    results.Ignored = true;
                }
            }

            if (creature1.IsInbreedWith(creature2))
            {
                if (options.DiscardOnInbreeding)
                {
                    results.Discarded = true;
                }
                else
                {
                    var potentialBadTraits = CreatureTrait.GetInbreedBadTraits().Where(x => !distinctConcatTraits.Contains(x)).ToArray();
                    double sum = 0;
                    foreach (var trait in potentialBadTraits)
                    {
                        sum += trait.GetTraitValue(valuator);
                    }
                    sum /= potentialBadTraits.Length;
                    sum *= options.InbreedingPenaltyWeight * 2;
                    results.Value += sum;
                }
            }

            if (options.DiscardOnAnyNegativeTraits)
            {
                if (creature2.Traits.Any(x => x.GetTraitValue(valuator) < 0))
                {
                    results.Discarded = true;
                }
            }

            if (results.Discarded != true && results.Ignored != true)
            {
                Dictionary<CreatureTrait, int> uniqueTraitCounter = new Dictionary<CreatureTrait, int>();
                foreach (var trait in concatTraits)
                {
                    if (uniqueTraitCounter.ContainsKey(trait))
                    {
                        uniqueTraitCounter[trait] += 1;
                    }
                    else
                    {
                        uniqueTraitCounter[trait] = 1;
                    }
                    var traitval = trait.GetTraitValue(valuator);
                    double result = 0;
                    if (traitval < 0) result += traitval * options.BadTraitWeight;
                    else if (traitval > 0) result += traitval;

                    results.Value += result;
                }

                if (options.PreferUniqueTraits)
                {
                    foreach (var keyval in uniqueTraitCounter)
                    {
                        if (keyval.Value == 1)
                        {
                            var traitval = keyval.Key.GetTraitValue(valuator);
                            if (traitval > 0)
                            {
                                results.Value += (traitval * options.UniqueTraitWeight) - traitval;
                            }
                        }
                    }

                }

                if (options.IncludePotentialValue)
                {
                    foreach (var trait in allPossibleTraits)
                    {
                        double result = 0;
                        result += EvaluatePotentialValue(creature1, valuator, trait);
                        result += EvaluatePotentialValue(creature2, valuator, trait);
                        results.Value += result;
                    }
                }

                if (results.Value > 0)
                {
                    var h1ColVal = options.GetValueForColor(creature1.Color);
                    var h2ColVal = options.GetValueForColor(creature2.Color);
                    var colValAdj = (h1ColVal + h2ColVal)*0.5f;
                    results.Value *= colValAdj;
                }
            }
            return results;
        }

        private double EvaluatePotentialValue(Creature creature, TraitValuator valuator, CreatureTrait trait)
        {
            double result = 0;
            if (trait.IsUnknownForThisCreature(creature))
            {
                var traitval = trait.GetTraitValue(valuator);
                if (traitval > 0)
                {
                    result += traitval * options.PotentialValuePositiveWeight;
                }
                else if (traitval < 0)
                {
                    result += traitval * options.PotentialValueNegativeWeight;
                }
            }
            return result;
        }

        public override System.Drawing.Color? GetHintColor(Creature creature, double minBreedValue, double maxBreedValue)
        {
            if (creature.CachedBreedValue.HasValue == false) { return DefaultIgnoredHintColor; }
            if (creature.CachedBreedValue == double.PositiveInfinity) { return DefaultExcludedHintColor; }

            HslColor color = new HslColor
            {
                Luminosity = 210D,
                Saturation = 240D,
                Hue = 35D //0 is red, 35 is yellow, 70 is green
            };
            
            double spectrum = Math.Max(Math.Abs(minBreedValue), Math.Abs(maxBreedValue));
            double normalizedBValue = creature.CachedBreedValue.Value / spectrum;
            color.Hue += normalizedBValue * 35;
            return color;
        }
    }
}