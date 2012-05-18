using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Reflexil.JustDecompile
{
	internal class MenuItem 
    {
        public MenuItem()
        {
            this.Collection = new List<MenuItem>();

            this.AddMenuItems();
        }

        public virtual void AddMenuItems() { }

        public object Header { get; set; }

        public ICommand Command { get; set; }

        public List<MenuItem> Collection { get; set; }

        public object Icon { get; set; }
    }
}
