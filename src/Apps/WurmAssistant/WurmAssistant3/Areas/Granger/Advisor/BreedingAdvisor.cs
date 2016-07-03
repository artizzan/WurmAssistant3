using System;
using System.Collections.Generic;
using AldursLab.WurmAssistant3.Areas.Granger.Advisor.Default;
using AldursLab.WurmAssistant3.Areas.Granger.Advisor.Disabled;
using AldursLab.WurmAssistant3.Areas.Granger.DataLayer;
using AldursLab.WurmAssistant3.Areas.Logging;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Granger.Advisor
{
    public class BreedingAdvisor
    {
        public const string DisabledId = "DISABLED";
        public const string DefaultId = "default";
        public static readonly IReadOnlyCollection<string> DefaultAdvisorIDs = new string[] { DisabledId, DefaultId };

        readonly GrangerContext context;
        readonly ILogger logger;
        readonly FormGrangerMain mainForm;
        readonly BreedingEvaluator breedEvalutator;

        public bool IsDisabled { get; private set; }
        public string AdvisorId { get; private set; }

        public BreedingAdvisor(
            [NotNull] FormGrangerMain mainForm,
            [NotNull] string advisorId,
            [NotNull] GrangerContext context,
            [NotNull] ILogger logger, 
            [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));
            if (advisorId == null) throw new ArgumentNullException(nameof(advisorId));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (defaultBreedingEvaluatorOptions == null) throw new ArgumentNullException(nameof(defaultBreedingEvaluatorOptions));
            this.mainForm = mainForm;
            this.AdvisorId = advisorId;
            this.context = context;
            this.logger = logger;

            IsDisabled = false;
            if (advisorId == DisabledId)
            {
                breedEvalutator = new DisabledBreedingEvaluator(logger);
                IsDisabled = true;
            }

            if (advisorId == DefaultId)
            {
                breedEvalutator = new DefaultBreedingEvaluator(logger, defaultBreedingEvaluatorOptions);
            }
        }

        internal BreedingEvalResults? GetBreedingValue(Creature creature)
        {
            if (IsDisabled) return null;
            if (mainForm.SelectedSingleCreature != null)
            {
                Creature evaluatedCreature = mainForm.SelectedSingleCreature;
                return breedEvalutator.Evaluate(evaluatedCreature, creature, mainForm.CurrentValuator);
            }
            else return null;
        }

        internal bool ShowOptions(FormGrangerMain formGrangerMain)
        {
            if (breedEvalutator != null)
            {
                if (breedEvalutator.EditOptions(formGrangerMain))
                {
                    return true;
                }
                else return false;
            }
            else
            {
                logger.Error("BreedEvalutator was null on ShowOptions");
                return false;
            }
        }

        internal System.Drawing.Color? GetHintColor(Creature creature, double minBreedValue, double maxBreedValue)
        {
            return breedEvalutator?.GetHintColor(creature, minBreedValue, maxBreedValue);
        }
    }
}
