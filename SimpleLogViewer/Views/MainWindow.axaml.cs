using Avalonia.Controls;

namespace SimpleLogViewer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ScrollViewerLog.ScrollChanged += ScrollViewer_ScrollChangedEvent;
        }
        private void ScrollViewer_ScrollChangedEvent(object? sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (e.OffsetDelta.Y==0 && e.OffsetDelta.X ==0)
            {
                scrollViewer?.ScrollToEnd();
            }
        }
    }
}