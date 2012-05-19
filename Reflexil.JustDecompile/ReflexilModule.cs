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

        #region TreeViewContextMenu region
        private AssemblyNodeContextMenu assemblyNodeContextMenu;
        private TypeDefinitionContextMenu typeDefinitionNodeContextMenu;
        private EmbeddedResourceContextMenu resourceNodeContextMenu;
        private AssemblyReferenceNode assemblyReferenceContextMenu;
        private MemberDefinitionContextMenu memberDefinitionNodeContextMenu;
        private ModuleDefinitionContextMenu moduleDefinitionNodeContextMenu;
        #endregion

        public ReflexilModule() { }

        public IHandler ActiveHandler { get; set; }

        public ReflexilWindow ReflexilWindow { get; set; }

        public void Initialize()
        {
            this.justDecompileCecilPlugin = new JustDecompileCecilPlugin(this);

            PluginFactory.Register(justDecompileCecilPlugin);

            this.regionManager.AddToRegion("ToolMenuRegion", new ReflexilToolsMenuItem(regionManager, ReflexilWindow));
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

        private void SetReflexilHandler(ITreeViewItem selectedTreeItem)
        {
            if (selectedTreeItem != null)
            {
                ActiveHandler = ReflexilWindow.HandleItem(selectedTreeItem);
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
