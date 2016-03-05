using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IWindowCollection : ICollection, IEnumerable
	{
		IWindow this[string identifier]
		{
			get;
		}

		IWindow Add(string identifier, Control content, string caption);

		void Remove(string identifier);
	}
}