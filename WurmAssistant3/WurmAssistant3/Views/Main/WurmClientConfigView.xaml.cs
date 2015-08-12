using System.Windows;

namespace AldursLab.WurmAssistant3.Views.Main
{
    /// <summary>
    /// Interaction logic for WurmClientConfigView.xaml
    /// </summary>
    public partial class WurmClientConfigView : Window
    {
        public WurmClientConfigView()
        {
            InitializeComponent();
            ApplyAndRunWa.Click += (sender, args) => this.DialogResult = true;
            DoNothingAndRunWa.Click += (sender, args) => this.DialogResult = true;
            DoNothingAndShutdown.Click += (sender, args) => this.DialogResult = true;
        }
    }
}
