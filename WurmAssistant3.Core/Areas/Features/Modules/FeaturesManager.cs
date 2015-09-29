using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using AldursLab.WurmAssistant3.Core.Root.Contracts;
using JetBrains.Annotations;
using Ninject;

namespace AldursLab.WurmAssistant3.Core.Areas.Features.Modules
{
    public class FeaturesManager : IFeaturesManager, IInitializable
    {
        readonly IFeatureViewFactory featureViewFactory;
        readonly ISystemTrayContextMenu systemTrayContextMenu;
        readonly IEnumerable<IFeature> features;
        readonly ILogger logger;

        bool asyncInitStarted = false;

        public FeaturesManager([NotNull] IEnumerable<IFeature> features,
            [NotNull] IFeatureViewFactory featureViewFactory, [NotNull] ISystemTrayContextMenu systemTrayContextMenu,
            [NotNull] ILogger logger)
        {
            if (featureViewFactory == null) throw new ArgumentNullException("featureViewFactory");
            if (systemTrayContextMenu == null) throw new ArgumentNullException("systemTrayContextMenu");
            if (features == null) throw new ArgumentNullException("features");
            if (logger == null) throw new ArgumentNullException("logger");
            this.featureViewFactory = featureViewFactory;
            this.systemTrayContextMenu = systemTrayContextMenu;
            this.features = features;
            this.logger = logger;
        }

        public void Initialize()
        {
            foreach (var f in features)
            {
                var feature = f;
                // this class controls context menu, because it is on MainForm, which isn't really resolved via DI and it's just a mess
                // consider: redesign
                systemTrayContextMenu.AddMenuItem(f.Name, () => feature.Show(), f.Icon);
            }
        }

        public IEnumerable<IFeature> Features { get { return features; } }
        public async void InitFeatures()
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
                foreach (var tuple in tasks)
                {
                    try
                    {
                        await tuple.Item1;
                        logger.Info(string.Format("Feature initialized: {0}", tuple.Item2.Name));
                    }
                    catch (Exception exception)
                    {
                        logger.Error(exception, string.Format("Error at feature initialization: {0}", tuple.Item2.Name));
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "");
            }
        }
    }
}
