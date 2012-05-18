using System;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Reflexil.Utils;
using Microsoft.Practices.Prism.Events;

namespace Reflexil.JustDecompile.MenuItems
{
    internal class TypeDefinitionContextMenu : MenuItemBase
    {
        public TypeDefinitionContextMenu(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }

        public override void AddMenuItems()
        {
            base.AddMenuItems();

            this.Collection.Add(new MenuSeparator());
            this.Collection.Add(new MenuItem { Header = "Inject event", Command = new DelegateCommand(InjectEvent) });
            this.Collection.Add(new MenuItem { Header = "Inject field", Command = new DelegateCommand(InjectField) });
            this.Collection.Add(new MenuItem { Header = "Inject method", Command = new DelegateCommand(InjectMethod) });
            this.Collection.Add(new MenuItem { Header = "Inject constructor", Command = new DelegateCommand(InjectConstructor) });
            this.Collection.Add(new MenuItem { Header = "Inject property", Command = new DelegateCommand(InjectProperty) });
            this.Collection.Add(new MenuSeparator());

            this.AddRenameDeleteNodes();
        }

        private void InjectProperty()
        {
            StudioPackage.Inject(EInjectType.Property);
        }

        private void InjectConstructor()
        {
            StudioPackage.Inject(EInjectType.Constructor);
        }

        private void InjectMethod()
        {
            StudioPackage.Inject(EInjectType.Method);
        }

        private void InjectField()
        {
            StudioPackage.Inject(EInjectType.Field);
        }

        private void InjectEvent()
        {
            StudioPackage.Inject(EInjectType.Event);
        }
    }
}
