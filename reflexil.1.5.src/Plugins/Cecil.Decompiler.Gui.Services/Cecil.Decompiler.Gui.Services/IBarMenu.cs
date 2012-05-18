using System;

namespace Cecil.Decompiler.Gui.Services
{
    public interface IBarMenu : IBarItem, IBar, IActionNameContainer
    {
	    event EventHandler DropDownOpened;
    }
}