using System.Windows;
using JustDecompile.API.Core;
using JustDecompile.API.Core.Services;

namespace De4dot.JustDecompile.DeobfuscateDialog
{
	public partial class DeobfuscateDialog : Window
	{
		public DeobfuscateDialog(ITreeViewItem selectedItem, IAssemblyManagerService assemblyManagerService)
		{
			InitializeComponent();
			this.DataContext = new DeobfuscateDialogViewModel(selectedItem, assemblyManagerService);
		}

		private void OnCloseHandler(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
