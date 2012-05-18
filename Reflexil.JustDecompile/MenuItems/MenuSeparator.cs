using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Reflexil.JustDecompile.MenuItems
{
    internal class MenuSeparator : MenuItem
    {
        public MenuSeparator()
        {
            Header = new Separator
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = 150,
                IsEnabled = false
            };
        }
    }
}
