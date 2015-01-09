using System;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IActionNameContainer
	{
		ActionNames ActionName
		{
			get;
			set;
		}
	}
}