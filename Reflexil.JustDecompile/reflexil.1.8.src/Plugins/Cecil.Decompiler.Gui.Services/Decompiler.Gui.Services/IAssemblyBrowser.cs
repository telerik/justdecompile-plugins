using System;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IAssemblyBrowser : IService
	{
		object ActiveItem
		{
			get;
			set;
		}

		object[] ActiveItemObjectHierarchy
		{
			get;
		}

		void GoBack();

		void GoForward();

		event EventHandler ActiveItemChanged;
	}
}