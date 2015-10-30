using System;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Legacy;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.Advisor.Default;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.Advisor.Disabled;
using AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.DataLayer;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Granger.Modules.Advisor
{
    public class BreedingAdvisor
    {
        public const string DISABLED_id = "DISABLED";
        public const string DEFAULT_id = "default";
        public static string[] DefaultAdvisorIDs = new string[] { DISABLED_id, DEFAULT_id };

        public bool IsDisabled { get; private set; }
        public string AdvisorID { get; private set; }
        private GrangerContext Context;
        readonly ILogger logger;
        private FormGrangerMain MainForm;

        private BreedingEvaluator BreedEvalutator;

        public BreedingAdvisor(FormGrangerMain mainForm, string advisorID, GrangerContext Context,
            [NotNull] ILogger logger, [NotNull] DefaultBreedingEvaluatorOptions defaultBreedingEvaluatorOptions)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (defaultBreedingEvaluatorOptions == null)
                throw new ArgumentNullException("defaultBreedingEvaluatorOptions");
            this.MainForm = mainForm;
            this.AdvisorID = advisorID;
            this.Context = Context;
            this.logger = logger;

            IsDisabled = false;
            if (advisorID == DISABLED_id)
            {
                BreedEvalutator = new DisabledBreedingEvaluator(logger);
                IsDisabled = true;
            }

            if (advisorID == DEFAULT_id)
            {
                BreedEvalutator = new DefaultBreedingEvaluator(logger, defaultBreedingEvaluatorOptions);
            }
        }

        internal BreedingEvalResults? GetBreedingValue(Creature creature)
        {
            if (IsDisabled) return null;
            if (MainForm.SelectedSingleCreature != null) //this is cached value
            {
                // this is the currently user-selected creature, while parameter creatures are iterated by display process
                Creature evaluatedCreature = MainForm.SelectedSingleCreature;
                return BreedEvalutator.Evaluate(evaluatedCreature, creature, MainForm.CurrentValuator);
            }
            else return null;
        }

        internal System.Drawing.Color? GetColorForThisValue(int? compareValue)
        {
            return null;
        }

        /// <summary>
        /// Options persistence is handled automatically,
        /// advisor rebuild is not needed
        /// </summary>
        /// <param name="formGrangerMain"></param>
        /// <returns></returns>
        internal bool ShowOptions(FormGrangerMain formGrangerMain)
        {
            if (BreedEvalutator != null)
            {
                if (BreedEvalutator.EditOptions(formGrangerMain))
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
            if (BreedEvalutator == null) return null;
            else
            {
                return BreedEvalutator.GetHintColor(creature, minBreedValue, maxBreedValue);
            }
        }
    }
}
