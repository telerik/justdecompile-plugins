using System;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IAction
	{
		bool Enabled
		{
			get;
			set;
		}

		string Name
		{
			get;
		}

		void Execute();

		event EventHandler EnabledChanged;
	}
}