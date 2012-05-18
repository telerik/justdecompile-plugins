using System;
using Mono.Cecil;

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