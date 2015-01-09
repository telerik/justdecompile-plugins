using System;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IWindowManager
	{
		IWindowCollection Windows
		{
			get;
		}

		void ShowMessage(string message);
	}
}