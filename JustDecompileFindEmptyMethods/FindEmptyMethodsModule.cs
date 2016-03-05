using System.ComponentModel.Composition;
using FindEmptyMethods.Menus;
using FindEmptyMethods.Views;
using JustDecompile.API.CompositeEvents;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace FindEmptyMethods
{
    [ModuleExport(typeof(FindEmptyMethodsModule))]
    internal class FindEmptyMethodsModule
     : IModule, IPartImportsSatisfiedNotification
    {
        [Import]
#pragma warning disable CS0649 // Never assigned
        private IRegionManager regionManager;

        [Import]
        private IEventAggregator eventAggregator;
#pragma warning restore CS0649 // Never assigned

        private PluginAreaView pluginAreaView;

        public void Initialize()
        {
            this.pluginAreaView = new PluginAreaView(this.ClosePluginAreaView);
        }

        public void OnImportsSatisfied()
        {
            var menuItem = new FindEmptyMethodsMenuItem(
                    this.eventAggregator.GetEvent<SelectedTreeViewItemChangedEvent>(),
                    this.ShowResults);

            this.regionManager.AddToRegion("TypeTreeViewContextMenuRegion", menuItem);
            this.regionManager.AddToRegion("InheritTreeViewContextMenuRegion", menuItem);
            this.regionManager.AddToRegion("BaseTypeTreeViewContextMenuRegion", menuItem);

            this.regionManager.AddToRegion("NamespaceTreeViewContextMenuRegion", menuItem);
            this.regionManager.AddToRegion("ModuleDefinitionTreeViewContextMenuRegion", menuItem);
            this.regionManager.AddToRegion("AssemblyTreeViewContextMenuRegion", menuItem);
        }

        private void ShowResults(string text)
        {
            this.pluginAreaView.Text = text;

            this.ClosePluginAreaView();
            this.regionManager.AddToRegion("PluginRegion", this.pluginAreaView);
        }

        private void ClosePluginAreaView()
        {
            var pluginRegion = regionManager.Regions["PluginRegion"];

            if (pluginRegion.Views.Contains(this.pluginAreaView))
            {
                pluginRegion.Remove(this.pluginAreaView);
            }
        }
    }
}
