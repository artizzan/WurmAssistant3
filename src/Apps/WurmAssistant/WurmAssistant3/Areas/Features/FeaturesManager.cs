using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Logging;
using AldursLab.WurmAssistant3.Utils;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Areas.Features
{
    [KernelBind(BindingHint.Singleton), UsedImplicitly]
    public class FeaturesManager : IFeaturesManager
    {
        readonly IKernel kernel;
        readonly ISystemTrayContextMenu systemTrayContextMenu;
        readonly ILogger logger;
        readonly List<IFeature> features;
        bool asyncInitStarted = false;

        public FeaturesManager(
            [NotNull] IKernel kernel,
            [NotNull] ISystemTrayContextMenu systemTrayContextMenu,
            [NotNull] ILogger logger)
        {
            if (kernel == null) throw new ArgumentNullException(nameof(kernel));
            if (systemTrayContextMenu == null) throw new ArgumentNullException(nameof(systemTrayContextMenu));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.kernel = kernel;
            this.systemTrayContextMenu = systemTrayContextMenu;
            this.logger = logger;

            var allFeatureBindings = kernel.GetBindings(typeof(IFeature));
            List<IFeature> resolvedFeatures = new List<IFeature>();
            foreach (var allFeatureBinding in allFeatureBindings)
            {
                try
                {
                    var feature = kernel.Get<IFeature>(allFeatureBinding.Metadata.Name);
                    resolvedFeatures.Add(feature);
                }
                catch (Exception exception)
                {
                    logger.Error(exception, $"Unable to resolve IFeature named {allFeatureBinding.Metadata.Name}");
                }
            }
            features = resolvedFeatures;

            Init();
        }

        public IEnumerable<IFeature> Features => features;

        void Init()
        {
            // Introducing ConventionBindingManager has affected the order.
            // Ensure that core features are always ordered like so...
            ReorderCoreFeatures(new[]
            {
                "Sounds Manager",
                "Log Searcher",
                "Calendar",
                "Timers",
                "Triggers",
                "Granger",
                "Crafting Assistant",
                "Reveal Creatures Parser",
                "Skill Stats",
                "Combat Assistant",
                "Sermoner"
            });

            foreach (var f in Features)
            {
                var feature = f;
                systemTrayContextMenu.AddMenuItem(f.Name, () => feature.Show(), f.Icon);
            }
        }

        public async void InitFeaturesAsync()
        {
            if (asyncInitStarted)
            {
                throw new InvalidOperationException("AsyncInitFeatures already triggered");
            }

            asyncInitStarted = true;

            try
            {
                List<Tuple<Task, IFeature>> tasks = new List<Tuple<Task, IFeature>>();
                foreach (var feature in Features)
                {
                    tasks.Add(new Tuple<Task, IFeature>(feature.InitAsync(), feature));
                }

                bool irrklangDependencyMissingHandled = false;

                foreach (var tuple in tasks)
                {
                    try
                    {
                        await tuple.Item1;
                        logger.Info(string.Format("Feature initialized: {0}", tuple.Item2.Name));
                    }
                    catch (Exception exception)
                    {
                        if (!irrklangDependencyMissingHandled)
                        {
                            var validator = new IrrklangDependencyValidator();
                            irrklangDependencyMissingHandled =
                                validator.HandleWhenMissingIrrklangDependency(exception);
                        }

                        logger.Error(exception, string.Format("Error at feature initialization: {0}", tuple.Item2.Name));
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "");
            }
        }

        void ReorderCoreFeatures(string[] orderedFeatureNames)
        {
            var otherFeatures = Features.Where(feature => orderedFeatureNames.All(s => s != feature.Name)).ToArray();

            List<IFeature> orderedFeatures = new List<IFeature>();
            foreach (var orderedFeatureMame in orderedFeatureNames)
            {
                orderedFeatures.Add(Features.SingleOrDefault(feature => feature.Name == orderedFeatureMame));
            }

            orderedFeatures.AddRange(otherFeatures);

            features.Clear();
            features.AddRange(orderedFeatures.Where(feature => feature != null));
        }
    }
}
