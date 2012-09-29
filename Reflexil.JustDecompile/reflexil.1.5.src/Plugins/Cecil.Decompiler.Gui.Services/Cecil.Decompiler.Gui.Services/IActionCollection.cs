using System.Collections;
using System;

namespace Cecil.Decompiler.Gui.Services
{
    public interface IActionCollection : ICollection, IEnumerable
    {
	    IAction this[string name] { get; }

	    IAction this[ActionNames name] { get; }

	    void Add(IAction action);

	    void Clear();

	    bool Contains(IAction action);

	    void Remove(IAction action);
    }
}