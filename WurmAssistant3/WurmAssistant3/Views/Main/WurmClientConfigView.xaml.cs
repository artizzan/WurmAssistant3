using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using AldurSoft.WurmAssistant3.ViewModels.Main;

namespace AldurSoft.WurmAssistant3.Views.Main
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
