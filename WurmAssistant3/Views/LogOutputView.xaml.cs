using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

    public static class ScrollViewerEx
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEnd",
                typeof (bool),
                typeof (ScrollViewerEx),
                new PropertyMetadata(false, HookupAutoScrollToEnd));

        public static readonly DependencyProperty AutoScrollHandlerProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEndHandler",
                typeof (ScrollViewerAutoScrollToEndHandler),
                typeof (ScrollViewerEx));

        static void HookupAutoScrollToEnd(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null) return;

            SetAutoScrollToEnd(scrollViewer, (bool) e.NewValue);
        }

        public static bool GetAutoScrollToEnd(ScrollViewer instance)
        {
            return (bool) instance.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScrollToEnd(ScrollViewer instance, bool value)
        {
            var oldHandler = (ScrollViewerAutoScrollToEndHandler) instance.GetValue(AutoScrollHandlerProperty);
            if (oldHandler != null)
            {
                oldHandler.Dispose();
                instance.SetValue(AutoScrollHandlerProperty, null);
            }
            instance.SetValue(AutoScrollProperty, value);
            if (value)
                instance.SetValue(AutoScrollHandlerProperty, new ScrollViewerAutoScrollToEndHandler(instance));
        }
    }

    public class ScrollViewerAutoScrollToEndHandler : DependencyObject, IDisposable
    {
        readonly ScrollViewer scrollViewer;
        bool doScroll = true;

        public ScrollViewerAutoScrollToEndHandler(ScrollViewer scrollViewer)
        {
            if (scrollViewer == null)
            {
                throw new ArgumentNullException("scrollViewer");
            }

            this.scrollViewer = scrollViewer;
            this.scrollViewer.ScrollToEnd();
            this.scrollViewer.ScrollChanged += ScrollChanged;
        }

        void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {
                doScroll = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;
            }

            // Content scroll event : autoscroll eventually
            if (doScroll && e.ExtentHeightChange != 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }

        public void Dispose()
        {
            scrollViewer.ScrollChanged -= ScrollChanged;
        }
    }

    public static class Extensions
    {
        public static T GetDescendantByType<T>(this Visual element) where T : class
        {
            if (element == null)
            {
                return default(T);
            }
            if (element.GetType() == typeof(T))
            {
                return element as T;
            }
            T foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = visual.GetDescendantByType<T>();
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

    }
}
