using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using JustDecompile.API.CompositeEvents;
using JustDecompile.API.Core;
using JustDecompile.API.Core.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Controls;
using System.Threading.Tasks;

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

        [Import]
        private ITabService tabService;

        private ITypeDefinition selectedTypeDefinition;

        private IEnumerable<ITreeViewItem> treeViewItems;

#pragma warning restore 0649

        const string ASSEMBLYTREEVIEWCONTEXTMENUREGION = "AssemblyTreeViewContextMenuRegion";
        const string MODULEDEFINITIONTREEVIEWCONTEXTMENUREGION = "ModuleDefinitionTreeViewContextMenuRegion";
        const string TYPEDEFINITIONTREEVIEWCONTEXTMENUREGION = "TypeTreeViewContextMenuRegion";

        private MenuItem goToEntryPointAssemblyMenuItem = null;
        private MenuItem goToEntryPointModuleMenuItem = null;
        private MenuItem findInheritedTypesMenuItem = null;
        private IMethodDefinition selectedItem = null;

        private ProgressWindow progressWindow;

        public void Initialize()
        {
            goToEntryPointAssemblyMenuItem = new MenuItem("GoTo Entry Point", new DelegateCommand(goToEntryPointMenuItemCommand));

            goToEntryPointModuleMenuItem = new MenuItem("GoTo Entry Point", new DelegateCommand(goToEntryPointMenuItemCommand));

            findInheritedTypesMenuItem = new MenuItem("Load all inherited types in new tabs", new DelegateCommand(LoadInheritedTypes));

            this.eventAggregator.GetEvent<SelectedTreeViewItemChangedEvent>().Subscribe(selectedTreeViewItemChanged);

            this.eventAggregator.GetEvent<TreeViewItemCollectionChangedEvent>().Subscribe(OnTreeViewCollectionChanged);
        }

        private void OnTreeViewCollectionChanged(IEnumerable<ITreeViewItem> treeViewItems)
        {
            this.treeViewItems = treeViewItems;
        }

        private void LoadInheritedTypes()
        {
            Task.Factory.StartNew(() =>
                {
                    var searchAssemblies = new Dictionary<IAssemblyDefinitionTreeViewItem, List<string>>();

                    int typesCount = 0;

                    foreach (IAssemblyDefinitionTreeViewItem assemblyDefinitionTreeViewItem in this.treeViewItems.OfType<IAssemblyDefinitionTreeViewItem>())
                    {
                        IModuleDefinition moduleDefinition = assemblyDefinitionTreeViewItem.AssemblyDefinition.MainModule;

                        if (moduleDefinition.AssemblyReferences.Any(a => a.FullName == selectedTypeDefinition.Module.Assembly.FullName) ||
                            moduleDefinition.FilePath == selectedTypeDefinition.Module.FilePath)
                        {
                            foreach (ITypeDefinition typeDefinition in moduleDefinition.Types)
                            {
                                if (HasMatchingType(typeDefinition))
                                {
                                    typesCount++;

                                    if (searchAssemblies.ContainsKey(assemblyDefinitionTreeViewItem))
                                    {
                                        searchAssemblies[assemblyDefinitionTreeViewItem].Add(typeDefinition.Namespace);
                                    }
                                    else
                                    {
                                        searchAssemblies.Add(assemblyDefinitionTreeViewItem, new List<string> { typeDefinition.Namespace });
                                    }
                                }
                            }
                        }
                    }
                    var treeViewItemsBag = new TreeViewItemsBag(OnItemsLoaded, typesCount);

                    if (typesCount == 0)
                    {
                        treeViewItemsBag.Completed();
                    }
                    foreach (KeyValuePair<IAssemblyDefinitionTreeViewItem, List<string>> item in searchAssemblies)
                    {
                        IList<string> namespaces = item.Value;

                        item.Key.Expand(t => OnItemsLoaded(namespaces, t, treeViewItemsBag));
                    }
                });
            progressWindow = new ProgressWindow();

            progressWindow.ShowDialog();
        }

        private void OnItemsLoaded(IList<string> namespaces, IEnumerable<ITreeViewItem> items, TreeViewItemsBag itemsBag)
        {
            foreach (ITreeViewItem treeViewItem in items)
            {
                if (treeViewItem.TreeNodeType == TreeNodeType.AssemblyModuleDefinition)
                {
                    treeViewItem.Expand(r => OnItemsLoaded(namespaces, r, itemsBag));
                }
                else if (treeViewItem.TreeNodeType == TreeNodeType.AssemblyNamespace &&
                         namespaces.Any(n => n.Equals(((INamespaceTreeViewItem)treeViewItem).Namespace, StringComparison.OrdinalIgnoreCase)))
                {
                    treeViewItem.Expand(r => OnItemsLoaded(r.OfType<ITypeDefinitionTreeViewItem>(), itemsBag));
                }
            }
        }

        private void OnItemsLoaded(IEnumerable<ITypeDefinitionTreeViewItem> items, TreeViewItemsBag itemsBag)
        {
            foreach (ITypeDefinitionTreeViewItem treeViewItem in items)
            {
                ITypeDefinition typeDefinition = treeViewItem.TypeDefinition;

                if (HasMatchingType(typeDefinition))
                {
                    itemsBag.AddNewEntry(treeViewItem);
                }
            }
        }

        private bool HasMatchingType(ITypeDefinition typeDefinition)
        {
            if ((typeDefinition.BaseType != null && typeDefinition.BaseType.Resolve().FullName == selectedTypeDefinition.FullName)
                  || (typeDefinition.HasInterfaces && typeDefinition.Interfaces.Any(a => a.Resolve().FullName == selectedTypeDefinition.FullName)))
            {
                return true;
            }
            return false;
        }

        private void OnItemsLoaded(List<ITreeViewItem> items)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    progressWindow.Close();

                    if (items.Count == 0)
                    {
                        MessageBox.Show("No inherited types were found!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        bool loadTabs = true;

                        if (items.Count > 30)
                        {
                            loadTabs = MessageBox.Show("You are attempting to open too many types. Do you want to proceed?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                        }
                        if (loadTabs)
                        {
                            this.tabService.OpenInNewTab(items);
                        }
                    }
                }));
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
            else if (selectedTreeItem.TreeNodeType == TreeNodeType.AssemblyTypeDefinition)
            {
                ITypeDefinitionTreeViewItem typeDef = (ITypeDefinitionTreeViewItem)selectedTreeItem;
                selectedTypeDefinition = typeDef.TypeDefinition;
                region = TYPEDEFINITIONTREEVIEWCONTEXTMENUREGION;

                if (!regionManager.Regions[region].Views.Contains(findInheritedTypesMenuItem))
                {
                    regionManager.AddToRegion(region, findInheritedTypesMenuItem);
                }
            }
            else
            {
                selectedItem = null; //any other node has no EntryPoint
            }
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

        private class TreeViewItemsBag : List<ITreeViewItem>
        {
            private readonly Action<List<ITreeViewItem>> readyAction = delegate { };

            private readonly int typesCount;

            public TreeViewItemsBag(Action<List<ITreeViewItem>> readyAction, int typesCount)
            {
                this.typesCount = typesCount;

                this.readyAction = readyAction;
            }

            public void AddNewEntry(ITreeViewItem item)
            {
                this.Add(item);

                if (this.Count == typesCount)
                {
                    Completed();
                }
            }

            public void Completed()
            {
                readyAction(this);
            }
        }
    }
}
