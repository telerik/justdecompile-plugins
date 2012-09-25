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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JustDecompile.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Reflexil.Forms;
using Reflexil.Handlers;
using Reflexil.JustDecompile.MenuItems;
using Reflexil.Plugins;

namespace Reflexil.JustDecompile
{
	[ModuleExport(typeof(ReflexilModule))]
	public class ReflexilModule : IModule, IPartImportsSatisfiedNotification, IPackage
	{
		[Import]
		private IRegionManager regionManager;

		[Import]
		private IEventAggregator eventAggregator;

		private JustDecompileCecilPlugin justDecompileCecilPlugin;
        private ReflexilHost reflexilHost;
        private ITreeViewItem selectedItem;

		#region TreeViewContextMenu region
		private AssemblyNodeContextMenu assemblyNodeContextMenu;
		private TypeDefinitionContextMenu typeDefinitionNodeContextMenu;
		private EmbeddedResourceContextMenu resourceNodeContextMenu;
		private AssemblyReferenceNode assemblyReferenceContextMenu;
		private MemberDefinitionContextMenu memberDefinitionNodeContextMenu;
		private ModuleDefinitionContextMenu moduleDefinitionNodeContextMenu;
		#endregion

        public ReflexilModule()
        {
            JustDecompileCecilStudioPackage.HandleItemRequest += JustDecompileCecilStudioPackageHandleItemRequest;
        }

		public IHandler ActiveHandler { get; set; }

		public ReflexilWindow ReflexilWindow { get; set; }

        private bool IsReflexilHostLoaded
        {
            get { return regionManager.Regions["PluginRegion"].Views.Contains(reflexilHost); }
        }

		public void Initialize()
		{
			this.justDecompileCecilPlugin = new JustDecompileCecilPlugin(this);

			PluginFactory.Register(justDecompileCecilPlugin);

            this.regionManager.AddToRegion("ToolMenuRegion", new ReflexilToolsMenuItem(OnClickExecuted));
			this.regionManager.AddToRegion("AssemblyTreeViewContextMenuRegion", assemblyNodeContextMenu);
			this.regionManager.AddToRegion("TypeTreeViewContextMenuRegion", typeDefinitionNodeContextMenu);
			this.regionManager.AddToRegion("EmbeddedResourceTreeViewContextMenuRegion", resourceNodeContextMenu);
			this.regionManager.AddToRegion("ResourceTreeViewContextMenuRegion", resourceNodeContextMenu);
			this.regionManager.AddToRegion("AssemblyReferenceTreeViewContextMenuRegion", assemblyReferenceContextMenu);
			this.regionManager.AddToRegion("MemberTreeViewContextMenuRegion", memberDefinitionNodeContextMenu);
			this.regionManager.AddToRegion("ModuleDefinitionTreeViewContextMenuRegion", moduleDefinitionNodeContextMenu);
		}

		public void OnImportsSatisfied()
		{
			this.ReflexilWindow = new ReflexilWindow();

			this.assemblyNodeContextMenu = new AssemblyNodeContextMenu(eventAggregator);
			this.typeDefinitionNodeContextMenu = new TypeDefinitionContextMenu(eventAggregator);
			this.resourceNodeContextMenu = new EmbeddedResourceContextMenu(eventAggregator);
			this.assemblyReferenceContextMenu = new AssemblyReferenceNode(eventAggregator);
			this.memberDefinitionNodeContextMenu = new MemberDefinitionContextMenu(eventAggregator);
			this.moduleDefinitionNodeContextMenu = new ModuleDefinitionContextMenu(eventAggregator);

			this.eventAggregator.GetEvent<SelectedTreeViewItemChangedEvent>().Subscribe(SetReflexilHandler);
			this.eventAggregator.GetEvent<TreeViewItemCollectionChangedEvent>().Subscribe(LoadAssembliesIntoPlugin);
		}

        private void JustDecompileCecilStudioPackageHandleItemRequest(object sender, EventArgs e)
        {
            if (this.selectedItem != null)
            {
                ActiveHandler = ReflexilWindow.HandleItem(this.selectedItem);
            }
        }

        private void OnClickExecuted()
        {
            if (!IsReflexilHostLoaded)
            {
                if (this.reflexilHost == null)
                {
                    this.reflexilHost = new ReflexilHost(OnCloseReflexilHostExecuted, ReflexilWindow);
                }
                regionManager.AddToRegion("PluginRegion", reflexilHost);

                SetReflexilHandler(this.selectedItem);
            }
        }

        private void OnCloseReflexilHostExecuted()
        {
            IRegion pluginRegion = regionManager.Regions["PluginRegion"];

            if (pluginRegion.Views.Contains(reflexilHost))
            {
                pluginRegion.Remove(reflexilHost);
            }
        }

		private void SetReflexilHandler(ITreeViewItem selectedTreeItem)
		{
            this.selectedItem = selectedTreeItem;

            if (!this.IsReflexilHostLoaded)
            {
                return;
            }
            if (this.selectedItem != null)
            {
                if (!ReflexilWindow.Visible)
                {
                    ReflexilWindow.Visible = true;
                }
                ActiveHandler = ReflexilWindow.HandleItem(this.selectedItem);
            }
            else
            {
                ReflexilWindow.Visible = false;
            }
		}

		private void LoadAssembliesIntoPlugin(IEnumerable<ITreeViewItem> assemblies)
		{
			justDecompileCecilPlugin.LoadAssemblies(assemblies.Where(i => i.TreeNodeType == TreeNodeType.AssemblyDefinition)
															  .Cast<IAssemblyDefinitionTreeViewItem>()
															  .Select(i => i.AssemblyDefinition));
		}
	}
}