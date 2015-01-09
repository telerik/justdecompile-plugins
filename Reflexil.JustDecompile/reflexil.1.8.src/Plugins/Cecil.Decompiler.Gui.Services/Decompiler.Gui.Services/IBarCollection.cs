using System;
using System.Collections;
using System.Reflection;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IBarCollection : ICollection, IEnumerable
	{
		IBar this[string name]
		{
			get;
		}

		IBar this[BarNames name]
		{
			get;
		}

		void Add(IBar bar);

		void Clear();

		bool Contains(IBar bar);

		void Remove(IBar bar);
	}
}