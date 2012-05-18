using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;

namespace Reflexil.JustDecompile
{
    internal class MemberDefinitionContextMenu : MenuItemBase
    {
        public MemberDefinitionContextMenu(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }

        public override void AddMenuItems()
        {
            this.AddRenameDeleteNodes();
        }
    }
}
