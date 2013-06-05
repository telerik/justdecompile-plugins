using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JustDecompile.Plugins.GoToEntryPoint
{
    public class ProgressWindow : Window
    {
        public ProgressBar ProgressBar;

        public ProgressWindow()
        {
            this.Title = "Searching...";

            this.ResizeMode = System.Windows.ResizeMode.NoResize;

            Grid grid = new Grid { Width = 250, Margin = new Thickness(15) };

            this.ProgressBar = new ProgressBar { HorizontalAlignment = HorizontalAlignment.Stretch, Height = 20 };

            this.ProgressBar.IsIndeterminate = true;

            grid.Children.Add(ProgressBar);

            this.Content = grid;

            this.WindowStyle = WindowStyle.ToolWindow;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            this.Owner = Application.Current.MainWindow;

            this.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
