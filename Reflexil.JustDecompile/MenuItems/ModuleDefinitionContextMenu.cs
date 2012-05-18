using System;
using System.Linq;
using JustDecompile.Core;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Mono.Cecil;
using Reflexil.Utils;

namespace Reflexil.JustDecompile.MenuItems
{
    internal class ModuleDefinitionContextMenu : MenuItemBase
    {
        public ModuleDefinitionContextMenu(IEventAggregator eventAggregator)
            : base(eventAggregator) { }

        public override void AddMenuItems()
        {
            base.AddMenuItems();

            this.Collection.Add(new MenuItem { Header = "Inject assembly reference", Command = new DelegateCommand(OnAssemblyReferenceClass) });
            this.Collection.Add(new MenuItem { Header = "Inject resource", Command = new DelegateCommand(OnResourceClass) });
            this.Collection.Add(new MenuSeparator());
            this.Collection.Add(new MenuItem { Header = "Save as...", Command = new DelegateCommand(OnSaveAs) });
            this.Collection.Add(new MenuItem { Header = "Obfuscator search...", Command = new DelegateCommand(OnObfuscatorSearch) });
            this.Collection.Add(new MenuItem { Header = "Reload", Command = new DelegateCommand(OnReaload) });
            this.Collection.Add(new MenuItem { Header = "Rename", Command = new DelegateCommand(OnRename) });
            this.Collection.Add(new MenuItem { Header = "Verify", Command = new DelegateCommand(OnVerify) });
        }

        private void OnAssemblyReferenceClass()
        {
            StudioPackage.Inject(EInjectType.AssemblyReference);
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

        private void OnObfuscatorSearch()
        {
            string currentAssemblyPath = GetFilePath();

            if (!string.IsNullOrEmpty(currentAssemblyPath))
            {
                AssemblyHelper.SearchObfuscator(currentAssemblyPath);
            }
        }

        private void OnReaload()
        {
            StudioPackage.ReloadAssembly();
        }

        private void OnRename()
        {
            StudioPackage.Rename();
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

                default: return string.Empty;
            }
        }
    }
}
