using System;
using System.Windows;

namespace AldursLab.WurmAssistant3.Areas.Debugging
{
    /// <summary>
    /// Interaction logic for DebuggingWindow.xaml
    /// </summary>
    [KernelBind]
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
