using System;
using System.Drawing;
using System.Windows.Forms;
using AldursLab.WurmAssistant3.Core.Areas.Features.Contracts;
using AldursLab.WurmAssistant3.Core.Areas.Features.Modules;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Features.Views
{
    public partial class FeaturesView : UserControl
    {
        readonly IFeaturesManager featuresManager;

        public FeaturesView([NotNull] IFeaturesManager featuresManager)
        {
            if (featuresManager == null) throw new ArgumentNullException("featuresManager");
            this.featuresManager = featuresManager;
            InitializeComponent();

            flowLayoutPanel.Controls.Clear();

            var features = featuresManager.Features;

            foreach (var f in features)
            {
                var feature = f;
                Button btn = new Button
                {
                    Size = new Size(80, 80),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    BackgroundImage = feature.Icon
                };

                btn.Click += (o, args) => feature.Show();
                toolTips.SetToolTip(btn, feature.Name);
                flowLayoutPanel.Controls.Add(btn);
            }
        }
    }
}
