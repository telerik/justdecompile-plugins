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
using JustDecompile.API.Core;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Mono.Cecil;
using Reflexil.Utils;

namespace Reflexil.JustDecompile.MenuItems
{
	internal class ModuleDefinitionContextMenu : MenuItemBase
	{
		public ModuleDefinitionContextMenu(IEventAggregator eventAggregator) : base(eventAggregator)
		{
		}

		public override void AddMenuItems()
		{
			base.AddMenuItems();

			this.MenuItems.Add(new MenuItem { Header = "Inject assembly reference", Command = new DelegateCommand(OnAssemblyReferenceClass) });
			this.MenuItems.Add(new MenuItem { Header = "Inject resource", Command = new DelegateCommand(OnResourceClass) });
			this.MenuItems.Add(new MenuSeparator());
			this.MenuItems.Add(new MenuItem { Header = "Save as...", Command = new DelegateCommand(OnSaveAs) });
			this.MenuItems.Add(new MenuItem { Header = "Reload", Command = new DelegateCommand(OnReaload) });
			this.MenuItems.Add(new MenuItem { Header = "Rename", Command = new DelegateCommand(OnRename) });
			this.MenuItems.Add(new MenuItem { Header = "Verify", Command = new DelegateCommand(OnVerify) });
		}

		private string GetFilePath()
		{
			if (this.StudioPackage.SelectedTreeViewItem == null)
			{
				return string.Empty;
			}
			switch (this.StudioPackage.SelectedTreeViewItem.TreeNodeType)
			{
				case TreeNodeType.AssemblyDefinition:
					return ((IAssemblyDefinitionTreeViewItem)this.StudioPackage.SelectedTreeViewItem).AssemblyDefinition.MainModule.FilePath;

				case TreeNodeType.AssemblyModuleDefinition:
					return ((IAssemblyModuleDefinitionTreeViewItem)this.StudioPackage.SelectedTreeViewItem).ModuleDefinition.FilePath;

				default:
					return string.Empty;
			}
		}

		private void OnAssemblyReferenceClass()
		{
			StudioPackage.Inject(EInjectType.AssemblyReference);
		}

		private void OnReaload()
		{
			StudioPackage.ReloadAssembly();
		}

		private void OnRename()
		{
			StudioPackage.Rename();
		}

		private void OnResourceClass()
		{
			StudioPackage.Inject(EInjectType.Resource);
		}

		private void OnSaveAs()
		{
			AssemblyDefinition assemblyDefinition = StudioPackage.GetCurrentAssemblyDefinition();

			string getOrginalFilePath = GetFilePath();

			if (!string.IsNullOrEmpty(getOrginalFilePath))
			{
				AssemblyHelper.SaveAssembly(assemblyDefinition, getOrginalFilePath);
			}
		}

		private void OnVerify()
		{
			AssemblyDefinition assemblyDefinition = StudioPackage.GetCurrentAssemblyDefinition();

			string getOrginalFilePath = GetFilePath();

			if (!string.IsNullOrEmpty(getOrginalFilePath))
			{
				AssemblyHelper.VerifyAssembly(assemblyDefinition, getOrginalFilePath);
			}
		}
	}
}