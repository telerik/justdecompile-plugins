using System;
using System.Drawing;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IBarItem
	{
		bool Enabled
		{
			get;
			set;
		}

		System.Drawing.Image Image
		{
			get;
			set;
		}

		string Text
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