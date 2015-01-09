using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Cecil.Decompiler.Gui.Services
{
	public class AssemblyLoadedEventArgs : EventArgs
	{
		public AssemblyDefinition Assembly
		{
			get;
			set;
		}

		public AssemblyLoadedEventArgs(AssemblyDefinition assembly)
		{
			this.Assembly = assembly;
		}
	}
}