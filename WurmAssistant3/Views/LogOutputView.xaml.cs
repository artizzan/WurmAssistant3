using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AldursLab.Wpf.Extensions.DotNet.Windows.Media;
using AldursLab.Wpf.ScrollViewers;

namespace AldursLab.WurmAssistant3.Views
{
    /// <summary>
    /// Interaction logic for LogOutputViewModel.xaml
    /// </summary>
    public partial class LogOutputView : UserControl
    {
        public LogOutputView()
        {
            InitializeComponent();

            var scrollViewer = MessagesListBox.GetDescendantByType<ScrollViewer>();
            scrollViewer.SetValue(ScrollViewerEx.AutoScrollProperty, true);
        }
    }
}
