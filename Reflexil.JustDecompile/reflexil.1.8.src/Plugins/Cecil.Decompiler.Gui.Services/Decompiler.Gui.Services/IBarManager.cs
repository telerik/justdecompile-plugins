namespace Cecil.Decompiler.Gui.Services
{
	public interface IBarManager : IService
	{
		IBarCollection Bars
		{
			get;
		}
	}
}