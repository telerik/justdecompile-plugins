using System;

namespace Cecil.Decompiler.Gui.Services
{
    public interface IPlugin
    {
	    void Load(IServiceProvider serviceProvider);

	    void Unload();
    }
}