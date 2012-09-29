using System.Collections;
using System;
using System.Drawing;

namespace Cecil.Decompiler.Gui.Services
{
    public interface IBarItemCollection : ICollection, IEnumerable
    {
	    IBarItem this[int index] { get; }

	    void Add(IBarItem value);

	    IBarButton AddButton(string caption, EventHandler clickHandler);

	    IBarButton AddButton(string caption, Image image, EventHandler clickHandler);

	    IBarMenu AddMenu(string name, string caption);

	    IBarMenu AddMenu(string name, string caption, Image image);

	    IBarSeparator AddSeparator();

	    void Clear();

	    bool Contains(IBarItem item);

	    int IndexOf(IBarItem item);

	    void Insert(int index, IBarItem value);

	    IBarButton InsertButton(int index, string caption, EventHandler clickHandler);

	    IBarButton InsertButton(int index, string caption, Image image, EventHandler clickHandler);

	    IBarMenu InsertMenu(int index, string name, string caption);

	    IBarMenu InsertMenu(int index, string name, string caption, Image image);

	    IBarSeparator InsertSeparator(int index);

	    void Remove(IBarItem item);

	    void RemoveAt(int index);
    }
}