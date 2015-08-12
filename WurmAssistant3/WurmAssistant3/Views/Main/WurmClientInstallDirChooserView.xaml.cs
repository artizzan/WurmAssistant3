using System.Windows;
using System.Windows.Forms;

namespace AldursLab.WurmAssistant3.Views.Main
{
    /// <summary>
    /// Interaction logic for WurmClientInstallDirChooserView.xaml
    /// </summary>
    public partial class WurmClientInstallDirChooserView : Window
    {
        public WurmClientInstallDirChooserView()
        {
            InitializeComponent();
            Apply.Click += (sender, args) => this.DialogResult = true;
            Cancel.Click += (sender, args) => this.DialogResult = false;
        }

        private void ChangePath_OnClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InstallDirFullPath.Text = dialog.SelectedPath;
            }
        }
    }
}
