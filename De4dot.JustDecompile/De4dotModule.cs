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

using System.ComponentModel.Composition;
using System.Windows;
using JustDecompile.API.Core;
using JustDecompile.API.CompositeEvents;
using JustDecompile.API.Core.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using de4dot.code;

namespace De4dot.JustDecompile
{
	[ModuleExport(typeof(De4dotModule))]
	public class De4dotModule : IModule, IPartImportsSatisfiedNotification
	{
		[Import]
		private IRegionManager regionManager;

		[Import]
		private IEventAggregator eventAggregator;

		[Import]
		private IAssemblyManagerService assemblyManagerService;

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

			this.assemblyNodeContextMenu.MenuItems.Add(new ContextMenuItem { Header = "Obfuscator search ...", Command = new DelegateCommand(OnContextMenuClick) });

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
			De4dotWrapper de4Dot = new De4dotWrapper();

			IObfuscatedFile obfuscationfile = de4Dot.SearchDeobfuscator(location);

			if (!de4Dot.IsUnknownDeobfuscator(obfuscationfile))
			{
				DeobfuscateDialog.DeobfuscateDialog dialog = new DeobfuscateDialog.DeobfuscateDialog(selectedItem, assemblyManagerService);
				dialog.ShowDialog();
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