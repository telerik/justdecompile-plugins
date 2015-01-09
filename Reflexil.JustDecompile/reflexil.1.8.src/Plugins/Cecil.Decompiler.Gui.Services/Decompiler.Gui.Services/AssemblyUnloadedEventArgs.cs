using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Cecil.Decompiler.Gui.Services
{
	public class AssemblyUnloadedEventArgs : EventArgs
	{
		public AssemblyDefinition Assembly
		{
			get;
			set;
		}

		public AssemblyUnloadedEventArgs(AssemblyDefinition assembly)
		{
			this.Assembly = assembly;
		}
	}
}