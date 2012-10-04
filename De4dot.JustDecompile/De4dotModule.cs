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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using JustDecompile.API;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Win32;

namespace De4dot.JustDecompile
{
    [ModuleExport(typeof(De4dotModule))]
    public class De4dotModule : IModule, IPartImportsSatisfiedNotification
    {
        [Import]
        private IRegionManager regionManager;

        [Import]
        private IEventAggregator eventAggregator;

        private ITreeViewItem selectedItem;

        private ContextMenuItem assemblyNodeContextMenu;

        public De4dotModule() { }

        public void Initialize()
        {
            this.regionManager.AddToRegion("AssemblyTreeViewContextMenuRegion", assemblyNodeContextMenu);

            this.regionManager.AddToRegion("ModuleDefinitionTreeViewContextMenuRegion", assemblyNodeContextMenu);
        }

        public void OnImportsSatisfied()
        {
            this.assemblyNodeContextMenu = new ContextMenuItem { Header = "De4dot" };

            this.assemblyNodeContextMenu.Collection.Add(new ContextMenuItem { Header = "Obfuscator search ...", Command = new DelegateCommand(OnContextMenuClick) });

            this.eventAggregator.GetEvent<SelectedTreeViewItemChangedEvent>().Subscribe(OnSelectedTreeViewItemChanged);
        }

        private void OnSelectedTreeViewItemChanged(ITreeViewItem obj)
        {
            this.selectedItem = obj;
        }

        private void OnContextMenuClick()
        {
            if (this.selectedItem == null)
            {
                return;
            }
            string location = GetFilePath();

            if (string.IsNullOrWhiteSpace(location))
            {
                return;
            }
            var de4Dot = new De4dotWrapper();

            var obfuscationfile = de4Dot.SearchDeobfuscator(location);

            if (!de4Dot.IsUnknownDeobfuscator(obfuscationfile))
            {
                string caption = "Assembly {0} is obfuscated with {1}. Clean the file ?";

                caption = string.Format(caption, Path.GetFileName(location), obfuscationfile.Deobfuscator.Name);

                if (MessageBox.Show(caption, "De4dot", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Assembly files (*.exe, *.dll)|*.exe;*.dll";
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(location);
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(location) + ".Cleaned" + Path.GetExtension(location);
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var progressWindpw = new DeobfuscationProgressWindow(obfuscationfile, this.eventAggregator)
                        {
                            Title = saveFileDialog.FileName,
                            Width = 500,
                            Height = 150,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            Owner = Application.Current.MainWindow
                        };
                        progressWindpw.Start(saveFileDialog.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("No obfuscator found (or unknown)");
            }
        }

        private string GetFilePath()
        {
            if (this.selectedItem == null)
            {
                return string.Empty;
            }
            switch (this.selectedItem.TreeNodeType)
            {
                case TreeNodeType.AssemblyDefinition:
                    return ((IAssemblyDefinitionTreeViewItem)this.selectedItem).AssemblyDefinition.MainModule.FilePath;

                case TreeNodeType.AssemblyModuleDefinition:
                    return ((IAssemblyModuleDefinitionTreeViewItem)this.selectedItem).ModuleDefinition.FilePath;

                default:
                    return string.Empty;
            }
        }
    }
}