using JustDecompile.API.CompositeEvents;
using JustDecompile.API.Core;
using JustDecompile.API.Core.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace JustDecompile.Plugins.GoToEntryPoint
{
    [ModuleExport(typeof(GoToEntryPointModule))]
    public class GoToEntryPointModule : IModule
    {
#pragma warning disable 0649
        [Import]
        private IRegionManager regionManager;

        [Import]
        private IEventAggregator eventAggregator;

        [Import]
        private ITreeViewNavigatorService navigationService;
#pragma warning restore 0649

        const string ASSEMBLYTREEVIEWCONTEXTMENUREGION = "AssemblyTreeViewContextMenuRegion";
        const string MODULEDEFINITIONTREEVIEWCONTEXTMENUREGION = "ModuleDefinitionTreeViewContextMenuRegion";

        private MenuItem goToEntryPointAssemblyMenuItem = null;
        private MenuItem goToEntryPointModuleMenuItem = null;
        private IMethodDefinition selectedItem = null;

        public void Initialize()
        {
            goToEntryPointAssemblyMenuItem = new MenuItem("GoTo Entry Point", new DelegateCommand(goToEntryPointMenuItemCommand));
            goToEntryPointModuleMenuItem = new MenuItem("GoTo Entry Point", new DelegateCommand(goToEntryPointMenuItemCommand));
            //this.regionManager.AddToRegion(ASSEMBLYTREEVIEWCONTEXTMENUREGION, goToEntryPointAssemblyMenuItem);
            //this.regionManager.AddToRegion(MODULEDEFINITIONTREEVIEWCONTEXTMENUREGION, goToEntryPointModuleMenuItem);

            eventAggregator.GetEvent<SelectedTreeViewItemChangedEvent>().Subscribe(selectedTreeViewItemChanged);
        }

        private void goToEntryPointMenuItemCommand()
        {
            if (selectedItem == null)
                return;

            navigationService.NavigateToCodePath(selectedItem);
        }

        private void selectedTreeViewItemChanged(ITreeViewItem selectedTreeItem)
        {
            string region = null;
            MenuItem menuItem = null;

            if (selectedTreeItem is IAssemblyDefinitionTreeViewItem)
            {
                IAssemblyDefinitionTreeViewItem assDef = (IAssemblyDefinitionTreeViewItem)selectedTreeItem;
                selectedItem = assDef.AssemblyDefinition.EntryPoint;
                region = ASSEMBLYTREEVIEWCONTEXTMENUREGION;
                menuItem = goToEntryPointAssemblyMenuItem;
            }
            else if (selectedTreeItem is IAssemblyModuleDefinitionTreeViewItem)
            {
                IAssemblyModuleDefinitionTreeViewItem modDef = (IAssemblyModuleDefinitionTreeViewItem)selectedTreeItem;
                selectedItem = modDef.ModuleDefinition.EntryPoint;
                region = MODULEDEFINITIONTREEVIEWCONTEXTMENUREGION;
                menuItem = goToEntryPointModuleMenuItem;
            }
            else
                selectedItem = null; //any other node has no EntryPoint

            if (selectedItem == null)
            {
                if (region != null)
                {
                    //we remove the menuitem it there is no EntryPoint
                    if (regionManager.Regions[region].Views.Contains(menuItem))
                        regionManager.Regions[region].Remove(menuItem);
                }
            }
            else
            {
                //and add it when there is one
                if (!regionManager.Regions[region].Views.Contains(menuItem))
                    regionManager.AddToRegion(region, menuItem);
            }
        }
    }
}
