namespace Cecil.Decompiler.Gui.Services
{
	public interface IActionManager : IService
	{
		IActionCollection Actions
		{
			get;
		}
	}
}