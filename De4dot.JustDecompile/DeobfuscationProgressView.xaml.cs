// Copyright 2012 Telerik AD
// 
// This program is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System.Threading.Tasks;
using System.Windows;
using JustDecompile.API.Core.Services;
using de4dot.code;
using de4dot.code.renamer;

namespace De4dot.JustDecompile
{
	public partial class DeobfuscationProgressWindow : Window
	{
		private IObfuscatedFile obfuscationFile;

		private readonly IAssemblyManagerService assemblyManager;

		public DeobfuscationProgressWindow(IObfuscatedFile obfuscationFile, IAssemblyManagerService assemblyManager)
		{
			this.assemblyManager = assemblyManager;

			this.obfuscationFile = obfuscationFile;

			InitializeComponent();
		}

		private void ReportProgress(double progressValue, string message)
		{
			Dispatcher.BeginInvoke(new System.Action(() =>
				{
					txtProgressText.Text = message;
					progress.Value = progressValue;
				}));
		}

		internal void Start(string newFileName)
		{
			this.Show();

			Task.Factory.StartNew(() =>
				{
					try
					{
						ReportProgress(0, "Preparing deobfuscation");
						obfuscationFile.deobfuscateBegin();

						ReportProgress(20, "Deobfuscating");
						obfuscationFile.deobfuscate();

						ReportProgress(40, "Finishing deobfuscation");
						obfuscationFile.deobfuscateEnd();

						ReportProgress(60, "Renaming items");
						// turn all flags on
						RenamerFlags flags = RenamerFlags.DontCreateNewParamDefs | RenamerFlags.DontRenameDelegateFields | RenamerFlags.RenameEvents | 
											 RenamerFlags.RenameFields | RenamerFlags.RenameGenericParams | RenamerFlags.RenameMethodArgs | 
											 RenamerFlags.RenameMethods | RenamerFlags.RenameNamespaces | RenamerFlags.RenameProperties | 
											 RenamerFlags.RenameTypes | RenamerFlags.RestoreEvents | RenamerFlags.RestoreEventsFromNames | 
											 RenamerFlags.RestoreProperties  | RenamerFlags.RestorePropertiesFromNames;
						Renamer renamer = new Renamer(obfuscationFile.DeobfuscatorContext, new IObfuscatedFile[] { obfuscationFile }, flags);
						renamer.rename();

						ReportProgress(80, "Saving");
						obfuscationFile.save();
					}
					finally
					{
						obfuscationFile.deobfuscateCleanUp();
					}
				})
				.ContinueWith(t =>
				{
					ReportProgress(100, "Done");

					if (t.Status == TaskStatus.Faulted)
					{
						MessageBox.Show(t.Exception.InnerExceptions[0].Message);
					}
					else if (t.Status == TaskStatus.RanToCompletion)
					{
						ReportProgress(100, "Assembly cleaned");

						if (MessageBox.Show(Application.Current.MainWindow, "Would you like to load the cleaned assembly?", string.Empty, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							this.assemblyManager.LoadAssembly(newFileName);
						}
					}
					this.Close();
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
