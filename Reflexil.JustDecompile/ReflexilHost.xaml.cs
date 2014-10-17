// Copyright 2012 Telerik AD
// 
// This program is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Practices.Prism.Commands;
using Reflexil.Forms;

namespace Reflexil.JustDecompile
{
    /// <summary>
    ///     Interaction logic for ReflexilHost.xaml
    /// </summary>
    public partial class ReflexilHost
    {
        private ReflexilWindow reflexilWindow;

        public ReflexilHost()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ReflexilHost(Action onCloseExecuted, ReflexilWindow reflexilWindow)
            : this()
        {
            btnClose.Command = new DelegateCommand(onCloseExecuted);

            this.reflexilWindow = reflexilWindow;

            var cecilStudioPackage = new JustDecompileCecilStudioPackage();

            header.Text = cecilStudioPackage.GetProductTitle();
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