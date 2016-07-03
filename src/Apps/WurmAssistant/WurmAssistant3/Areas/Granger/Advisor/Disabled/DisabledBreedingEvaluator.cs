using System;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Areas.Granger.ValuePreset;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Advisor.Disabled
{
    public class DisabledBreedingEvaluator : BreedingEvaluator
    {
        readonly ILogger logger;

        public DisabledBreedingEvaluator([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }

        public override bool EditOptions(FormGrangerMain formGrangerMain)
        {
            MessageBox.Show("No advisor selected");
            return false;
        }

        public override BreedingEvalResults? Evaluate(Creature creature1, Creature creature2, TraitValuator valuator)
        {
            return null;
        }

        public override System.Drawing.Color? GetHintColor(Creature creature, double minBreedValue, double maxBreedValue)
        {
            return null;
        }
    }
}