using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;

namespace Reflexil.JustDecompile.MenuItems
{
    internal class AssemblyNodeContextMenu : ModuleDefinitionContextMenu
    {
        public AssemblyNodeContextMenu(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }
    }
}
