using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AldursLab.Essentials.Extensions.DotNet.Drawing;
using AldursLab.WurmAssistant3.Areas.Granger.Parts.ValuePreset;
using AldursLab.WurmAssistant3.Areas.Granger.Services;
using AldursLab.WurmAssistant3.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Parts.Advisor.Default
{
    public class DefaultBreedingEvaluator : BreedingEvaluator
    {
        readonly ILogger logger;

        readonly DefaultBreedingEvaluatorOptions options;

        public DefaultBreedingEvaluator([NotNull] ILogger logger, DefaultBreedingEvaluatorOptions options)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
            this.options = options;
        }

        public override object GetOptions()
        {
            return (object)options;
        }

        public override bool EditOptions(FormGrangerMain formGrangerMain)
        {
            BreedingEvaluatorDefaultConfig ui = new BreedingEvaluatorDefaultConfig(options, logger);
            if (ui.ShowDialogCenteredOnForm(formGrangerMain) == DialogResult.OK)
            {
                return true;
            }
            else return false;
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
            var concatTraits = traits1.Concat<CreatureTrait>(traits2).ToArray(); //creature1 + creature2
            var presentTraits = concatTraits.Distinct().ToArray(); //creature1 + creature2 but without duplicates
            // not using these for now:
            //var uniqueTraits = GetUniqueTraits(presentTraits, traits1, traits2);  //traits which only one creature have
            //var missingTraits = CreatureTrait.GetAllTraits().Where(x => !presentTraits.Contains(x)).ToArray(); //all traits that creatures dont have
            double value2 = creature2.Value;

            if (creature1.IsMale == creature2.IsMale) results.Ignored = true;

            if (options.IgnoreNotInMood)
                if (creature1.NotInMood || creature2.NotInMood) results.Ignored = true;

            if (options.IgnorePregnant)
                if (creature1.Pregnant || creature2.Pregnant) results.Ignored = true;

            if (options.IgnoreRecentlyPregnant)
                if (creature1.PregnantInLast24H || creature2.PregnantInLast24H) results.Ignored = true;

            if (options.IgnoreOtherHerds)
                if (creature1.Herd != creature2.Herd) results.Ignored = true;

            if (options.IgnorePairedCreatures)
                if (creature1.HasMate() || creature2.HasMate())
                    results.Ignored = true;

            if (options.IgnoreSold)
                if (creature1.CheckTag("sold") || creature2.CheckTag("sold"))
                    results.Ignored = true;

            if (options.IgnoreDead)
                if (creature1.CheckTag("dead") || creature2.CheckTag("dead"))
                    results.Ignored = true;

            if (options.IgnoreFoals)
                if ((creature1.IsFoal() && !options.AgeIgnoreOnlyOtherCreatures) ||
                    creature2.IsFoal()) results.Ignored = true;

            if (options.IgnoreYoung)
                if (((creature1.Age.CreatureAgeId == CreatureAgeId.Young) && !options.AgeIgnoreOnlyOtherCreatures) ||
                    creature2.Age.CreatureAgeId == CreatureAgeId.Young)
                    results.Ignored = true;

            if (options.IgnoreAdolescent)
                if (((creature1.Age.CreatureAgeId == CreatureAgeId.Adolescent) && !options.AgeIgnoreOnlyOtherCreatures) ||
                    creature2.Age.CreatureAgeId == CreatureAgeId.Adolescent)
                    results.Ignored = true;

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
                if (options.DiscardOnInbreeding) results.Discarded = true;
                else
                {
                    // get all potential inbreeding-specific bad traits this creature doesnt yet have,
                    // average a value out of these traits,
                    // multiply by 2 (because this is like both creatures having one bad trait)
                    // multiply by inbreed weight (NOT bad trait weight)
                    // we add this to results
                    var potentialBadTraits = CreatureTrait.GetInbreedBadTraits().Where(x => !presentTraits.Contains(x)).ToArray();
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
                if (creature2.Traits.Where(x => x.GetTraitValue(valuator) < 0).Count() > 0)
                    results.Discarded = true;
            }

            // continue only if creature is still evaluated
            if (results.Discarded != true && results.Ignored != true)
            {
                // calculate value for each trait:
                // use 1.0, bad trait or unique trait weights if appropriate
                // use dict to check, which traits were already handled, 
                // the value of keys is meaningless, only key presence check is needed
                Dictionary<CreatureTrait, int> uniqueTraitCounter = new Dictionary<CreatureTrait, int>();
                foreach (var trait in concatTraits)
                {
                    //add this trait to counter for PreferUniqueTraits
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

                //apply bonus for unique traits
                if (options.PreferUniqueTraits)
                {
                    foreach (var keyval in uniqueTraitCounter)
                    {
                        if (keyval.Value == 1) //this trait was unique in this evaluation
                        {
                            var traitval = keyval.Key.GetTraitValue(valuator);
                            if (traitval > 0) //apply bonus if the trait is positive value
                            {
                                results.Value += (traitval * options.UniqueTraitWeight) - traitval;
                                //subtracting initial traitval because it was already applied
                                //this works for any weight, a 0.5 weight causes unique trait to have half value for result
                                //0.0 weight causes trait to have 0 value for result (effectively nullifying this trait value)

                                // NOTE: if in future good trait value has any other weights applied,
                                // this WILL break. This class is not expected to be improved,
                                // please write your own, new evaluator by subclassing BreedingEvaluator class
                                // and writing your own complete logic!
                            }
                        }
                    }

                }

                if (options.IncludePotentialValue)
                {
                    // here we need to take care of potential trait values
                    // this is hard to figure, because creature can contain many different hidden traits,
                    // that all can participate in breeding

                    // we handle this naively, asume creatures have all of their potential traits
                    // we regulate how much this affects result with the weight

                    // we need to loop over all possible traits twice, for each creature
                    // pick traits that req AH above their inspect skill and do eval for these
                    // we do this explicitly rather than in methods to improve readability
                    foreach (var trait in allPossibleTraits)
                    {
                        double result = 0;
                        result += EvaluatePotentialValue(creature1, valuator, trait);
                        result += EvaluatePotentialValue(creature2, valuator, trait);
                        results.Value += result;
                    }
                }

                // boost or lower value based on potential color of offspring
                if (results.Value > 0)
                {
                    var h1colVal = options.GetValueForColor(creature1.Color);
                    var h2colVal = options.GetValueForColor(creature2.Color);
                    var colValAdj = (h1colVal + h2colVal)*0.5f;
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
                // we dont care when its 0
            }
            return result;
        }

        static System.Drawing.Color? DefaultIgnoredHintColor = (System.Drawing.Color)(new HslColor(0D, 0D, 210D));
        static System.Drawing.Color? DefaultExcludedHintColor = (System.Drawing.Color)(new HslColor(210D, 240D, 0D));

        public override System.Drawing.Color? GetHintColor(Creature creature, double minBreedValue, double maxBreedValue)
        {
            if (creature.CachedBreedValue.HasValue == false) { return DefaultIgnoredHintColor; }
            if (creature.CachedBreedValue == double.PositiveInfinity) { return DefaultExcludedHintColor; }

            HslColor color = new HslColor();
            color.Luminosity = 210D;
            color.Saturation = 240D;
            color.Hue = 35D; //0 is red, 35 is yellow, 70 is green
            //for best candidate (maxBreedValue) name-only highlights: HSLColor(120D, 240D, 180D)

            double spectrum = Math.Max(Math.Abs(minBreedValue), Math.Abs(maxBreedValue));
            //normalize breed value to the spectrum
            double normalizedBValue = creature.CachedBreedValue.Value / spectrum;
            color.Hue += normalizedBValue * 35;
            return color;
        }
    }
}