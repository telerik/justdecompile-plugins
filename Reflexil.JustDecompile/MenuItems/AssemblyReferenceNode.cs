using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;

namespace Reflexil.JustDecompile.MenuItems
{
    internal class AssemblyReferenceNode : MenuItemBase
    {
        public AssemblyReferenceNode(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        public override void AddMenuItems()
        {
            this.AddRenameDeleteNodes();
        }
    }
}
