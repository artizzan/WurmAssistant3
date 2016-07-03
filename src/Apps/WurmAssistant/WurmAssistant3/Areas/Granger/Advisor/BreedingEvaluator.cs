using AldursLab.WurmAssistant3.Areas.Granger.ValuePreset;

namespace AldursLab.WurmAssistant3.Areas.Granger.Advisor
{
    public abstract class BreedingEvaluator
    {
        /// <summary>
        /// Calculates evaluation for this creature,
        /// null if evaluation does not return any valid results.
        /// </summary>
        /// <param name="creature1">First evaluated creature</param>
        /// <param name="creature2">Second evaluated creature</param>
        /// <param name="valuator">Trait values ruleset</param>
        public abstract BreedingEvalResults? Evaluate(Creature creature1, Creature creature2, TraitValuator valuator);

        /// <summary>
        /// Shows dialog window with evaluator options.
        /// </summary>
        /// <param name="formGrangerMain"></param>
        /// <returns></returns>
        public abstract bool EditOptions(FormGrangerMain formGrangerMain);

        /// <summary>
        /// Determines color for color-cued highlights.
        /// </summary>
        /// <param name="creature">evaluated creature.</param>
        /// <param name="minBreedValue">minimum of the value spectrum.</param>
        /// <param name="maxBreedValue">maximum of the value spectrum.</param>
        /// <returns></returns>
        public abstract System.Drawing.Color? GetHintColor(Creature creature, double minBreedValue, double maxBreedValue);
    }

    public struct BreedingEvalResults
    {
        public bool Ignored;
        public bool Discarded;
        public double Value;
    }
}
