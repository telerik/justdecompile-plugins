using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Cecil.Decompiler.Gui.Services
{
	public interface IAssemblyManager : IService
	{
		ICollection<AssemblyDefinition> Assemblies
		{
			get;
		}

		AssemblyDefinition LoadFile(string location);

		void SaveFile(AssemblyDefinition value, string location);

		void Unload(AssemblyDefinition value);

		event AssemblyLoadedEventHandler AssemblyLoaded;

		event AssemblyUnloadedEventHandler AssemblyUnloaded;
	}
}