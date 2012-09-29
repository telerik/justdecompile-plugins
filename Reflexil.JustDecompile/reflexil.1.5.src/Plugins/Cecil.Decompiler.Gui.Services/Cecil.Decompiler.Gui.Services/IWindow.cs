using System;
using System.Windows.Forms;
using System.Drawing;

namespace Cecil.Decompiler.Gui.Services
{
    public interface IWindow
    {
	    string Caption { get; }

	    Control Content { get; }

	    Image Image { get; set; }

	    bool Visible { get; set; }

    }
}