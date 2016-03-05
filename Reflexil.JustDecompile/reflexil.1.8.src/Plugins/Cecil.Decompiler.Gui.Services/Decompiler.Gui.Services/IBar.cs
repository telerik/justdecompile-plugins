using System;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IBar
	{
		IBarItemCollection Items
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}