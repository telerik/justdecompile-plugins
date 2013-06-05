using JustDecompile.API.Core;
using System.Collections.Generic;
using System.Windows.Input;

namespace JustDecompile.Plugins.GoToEntryPoint
{
    public class MenuItem : IMenuItem
    {
        List<IMenuItem> menuItems = new List<IMenuItem>();

        public MenuItem(string header)
            : this(header, null)
        {
        }

        public MenuItem(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public ICommand Command
        {
            get;
            set;
        }

        public object Header
        {
            get;
            set;
        }

        public object Icon
        {
            get;
            set;
        }

        public IList<IMenuItem> MenuItems
        {
            get { return menuItems; }
        }
    }
}
