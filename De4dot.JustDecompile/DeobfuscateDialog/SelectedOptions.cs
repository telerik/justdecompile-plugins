namespace De4dot.JustDecompile.DeobfuscateDialog
{
	public class SelectedOptions
	{
		public bool EnableControlFlowDeobfuscation { get; set; }

		public bool KeepObfuscatorClasses { get; set; }

		public bool RenameClasses { get; set; }

		/// <summary>
		/// Sets the properties to their default values.
		/// </summary>
		public SelectedOptions()
		{
			this.EnableControlFlowDeobfuscation = true;
			this.KeepObfuscatorClasses = false;
			this.RenameClasses = true;
		}
	}
}