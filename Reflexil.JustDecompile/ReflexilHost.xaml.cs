using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Reflexil.Forms;

namespace Reflexil.JustDecompile
{
    public partial class ReflexilHost
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ReflexilHost), null);

        private ReflexilWindow reflexilWindow;

        private readonly IRegionManager regionManager;

        public ReflexilHost()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;

            this.CloseCommand = new DelegateCommand(OnCloseExecuted);
        }

        public ReflexilHost(IRegionManager regionManager, ReflexilWindow reflexilWindow)
            : this()
        {
            this.regionManager = regionManager;

            this.reflexilWindow = reflexilWindow;

            var cecilStudioPackage = new JustDecompileCecilStudioPackage();

            this.Header = cecilStudioPackage.GetProductTitle();
        }

        public ICommand CloseCommand { get; private set; }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private void OnCloseExecuted()
        {
            regionManager.Regions["PluginRegion"].Remove(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hostPanel = new Panel { };
            hostPanel.Controls.Add(reflexilWindow);

            var host = new WindowsFormsHost { };
            host.Child = hostPanel;

            root.Children.Add(host);
        }

        private void RootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            reflexilWindow.Width = (int)this.ActualWidth;

            reflexilWindow.Height = (int)this.root.ActualHeight;
        }
    }
}
