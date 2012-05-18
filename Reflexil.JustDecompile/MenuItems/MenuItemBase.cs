using Microsoft.Practices.Prism.Commands;
using Reflexil.Utils;
using Microsoft.Practices.Prism.Events;
using JustDecompile.Core;

namespace Reflexil.JustDecompile
{
    internal abstract class MenuItemBase : MenuItem
    {
        public JustDecompileCecilStudioPackage StudioPackage;

        public MenuItemBase(IEventAggregator eventAggregator)
            : base()
        {
            this.StudioPackage = new JustDecompileCecilStudioPackage();
            
            this.Header = StudioPackage.GetProductVersion();

            eventAggregator.GetEvent<SelectedTreeViewItemChangedEvent>().Subscribe(SetReflexilHandler);
        }

        public void AddRenameDeleteNodes()
        {
            this.Collection.Add(new MenuItem { Header = "Rename...", Command = new DelegateCommand(OnRename) });
            this.Collection.Add(new MenuItem { Header = "Delete", Command = new DelegateCommand(OnDelete) });
        }

        public override void AddMenuItems()
        {
            this.Collection.Add(new MenuItem { Header = "Inject class", Command = new DelegateCommand(OnInjectClass) });
            this.Collection.Add(new MenuItem { Header = "Inject interface", Command = new DelegateCommand(OnInterfaceClass) });
            this.Collection.Add(new MenuItem { Header = "Inject struct", Command = new DelegateCommand(OnStructClass) });
            this.Collection.Add(new MenuItem { Header = "Inject enum", Command = new DelegateCommand(OnEnumClass) });
        }

        private void SetReflexilHandler(ITreeViewItem selectedTreeItem)
        {
            if (selectedTreeItem != null)
            {
                this.StudioPackage.SelectedTreeViewItem = selectedTreeItem;
            }
        }

        private void OnDelete()
        {
            StudioPackage.Delete();
        }

        private void OnRename()
        {
            StudioPackage.Rename();
        }

        private void OnInjectClass()
        {
            StudioPackage.Inject(EInjectType.Class);
        }

        private void OnInterfaceClass()
        {
            StudioPackage.Inject(EInjectType.Interface);
        }

        private void OnStructClass()
        {
            StudioPackage.Inject(EInjectType.Struct);
        }

        private void OnEnumClass()
        {
            StudioPackage.Inject(EInjectType.Enum);
        }
    }
}