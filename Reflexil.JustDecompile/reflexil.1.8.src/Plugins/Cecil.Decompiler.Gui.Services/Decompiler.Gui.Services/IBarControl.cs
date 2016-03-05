using System;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IBarControl
	{
		void PerformClick();

		event EventHandler Click;
	}
}