using System;
using System.Drawing;
using System.Windows.Forms;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IWindow
	{
		string Caption
		{
			get;
		}

		Control Content
		{
			get;
		}

		System.Drawing.Image Image
		{
			get;
			set;
		}

		bool Visible
		{
			get;
			set;
		}
	}
}