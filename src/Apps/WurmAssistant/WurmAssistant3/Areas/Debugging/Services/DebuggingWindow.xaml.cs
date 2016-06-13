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

namespace AldursLab.WurmAssistant3.Areas.Debugging.Services
{
    /// <summary>
    /// Interaction logic for DebuggingWindow.xaml
    /// </summary>
    public partial class DebuggingWindow : Window
    {
        public DebuggingWindow()
        {
            InitializeComponent();
        }

        private void CrashMeBtn_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("I am evil and crashing your app!");
        }
    }
}
