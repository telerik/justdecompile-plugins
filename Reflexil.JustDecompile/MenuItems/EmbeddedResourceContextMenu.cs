using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;

namespace Reflexil.JustDecompile.MenuItems
{
    internal class EmbeddedResourceContextMenu : MenuItemBase
    {
        public EmbeddedResourceContextMenu(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }

        public override void AddMenuItems()
        {
            this.AddRenameDeleteNodes();
        }
    }
}
