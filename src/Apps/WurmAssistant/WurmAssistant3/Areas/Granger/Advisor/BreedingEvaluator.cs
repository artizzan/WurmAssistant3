using AldursLab.WurmAssistant3.Areas.Granger.ValuePreset;

namespace AldursLab.WurmAssistant3.Areas.Granger.Advisor
{
    public abstract class BreedingEvaluator
    {
        /// <summary>
        /// get evaluation results for this creature,
        /// null if evaluation does not return any valid results
        /// </summary>
        /// <param name="creature1">first evaluated creature</param>
        /// <param name="creature2">other evaluated creature</param>
        /// <param name="valuator">current trait valuator used by granger</param>
        /// <returns></returns>
        public abstract BreedingEvalResults? Evaluate(Creature creature1, Creature creature2, TraitValuator valuator);
        /// <summary>
        /// get current options
        /// </summary>
        /// <returns></returns>
        public abstract object GetOptions();

        /// <summary>
        /// let user modify the options, true if any modifications made
        /// </summary>
        /// <param name="formGrangerMain"></param>
        /// <returns></returns>
        public abstract bool EditOptions(FormGrangerMain formGrangerMain);
        /// <summary>
        /// Determines color for color-cued highlights
        /// </summary>
        /// <param name="creature">evaluated creature</param>
        /// <param name="minBreedValue">minimum breed value for the batch</param>
        /// <param name="maxBreedValue">maximum breed value for the batch</param>
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
