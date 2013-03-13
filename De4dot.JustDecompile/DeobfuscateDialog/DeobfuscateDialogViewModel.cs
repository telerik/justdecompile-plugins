using System.IO;
using System.Windows;
using System.Windows.Input;
using JustDecompile.API.Core.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using de4dot.code;
using JustDecompile.API.Core;
using dnlib.DotNet;

namespace De4dot.JustDecompile.DeobfuscateDialog
{
	public class DeobfuscateDialogViewModel : NotificationObject
	{
		private bool keepObfuscatorClasses;
		private bool enableControlFlowDeobfuscation;
		private bool renameClasses;

		private readonly IAssemblyManagerService assemblyManagerService;

		private string message;

		public DeobfuscateDialogViewModel(ITreeViewItem selectedItem, IAssemblyManagerService assemblyManagerService)
		{
			this.selectedItem = selectedItem;
			this.assemblyManagerService = assemblyManagerService;

			/// Set the default values for the settings
			this.KeepObfuscatorClasses = false;
			this.EnableControlFlowDeobfuscation = true;
			this.RenameClasses = true;

			this.DeobfuscateCommand = new DelegateCommand(ExecuteDeobfuscateCommand);

			GenerateMessage();
		}

		public bool KeepObfuscatorClasses
		{
			get
			{
				return this.keepObfuscatorClasses;
			}
			set
			{
				this.keepObfuscatorClasses = value;
				this.RaisePropertyChanged("KeepObfuscatorClasses");
			}
		}

		public bool EnableControlFlowDeobfuscation
		{
			get
			{
				return this.enableControlFlowDeobfuscation;
			}
			set
			{
				this.enableControlFlowDeobfuscation = value;
				this.RaisePropertyChanged("EnableControlFlowDeobfuscation");
			}
		}

		public bool RenameClasses
		{
			get
			{
				return this.renameClasses;
			}
			set
			{
				this.renameClasses = value;
				this.RaisePropertyChanged("RenameClasses");
			}
		}

		public ICommand DeobfuscateCommand
		{
			get;
			internal set;
		}

		public SelectedOptions Options { get; set; }

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
				this.RaisePropertyChanged("Message");
			}
		}

		private void ExecuteDeobfuscateCommand()
		{
			if (this.selectedItem == null)
			{
				return;
			}

			string location = GetFilePath();

			if (string.IsNullOrWhiteSpace(location))
			{
				return;
			}

			De4dotWrapper de4Dot = new De4dotWrapper();

			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Assembly files (*.exe, *.dll)|*.exe;*.dll";
			saveFileDialog.InitialDirectory = Path.GetDirectoryName(location);
			saveFileDialog.FileName = Path.GetFileNameWithoutExtension(location) + ".Cleaned" + Path.GetExtension(location);
			if (saveFileDialog.ShowDialog() == true)
			{
				IObfuscatedFile obfuscationfile = UpdateObfuscationFileWithOptions(de4Dot, location, saveFileDialog.FileName);
				DeobfuscationProgressWindow progressWindow = new DeobfuscationProgressWindow(obfuscationfile, this.assemblyManagerService)
				{
					Title = saveFileDialog.FileName,
					Width = 500,
					Height = 150,
					WindowStartupLocation = WindowStartupLocation.CenterOwner,
					Owner = Application.Current.MainWindow
				};
				progressWindow.Start(saveFileDialog.FileName);
			}
		}
  
		private void GenerateMessage()
		{
			string location = GetFilePath();

			if (string.IsNullOrWhiteSpace(location))
			{
				return;
			}

			De4dotWrapper de4Dot = new De4dotWrapper();

			IObfuscatedFile obfuscationfile = de4Dot.SearchDeobfuscator(location);
			string caption = "Assembly {0} is obfuscated with {1}. Clean the file ?";
			caption = string.Format(caption, Path.GetFileName(location), obfuscationfile.Deobfuscator.Name);
			this.Message = caption;
		}

		private readonly ITreeViewItem selectedItem;

		private string GetFilePath()
		{
			if (this.selectedItem == null)
			{
				return string.Empty;
			}
			switch (this.selectedItem.TreeNodeType)
			{
				case TreeNodeType.AssemblyDefinition:
					return ((IAssemblyDefinitionTreeViewItem)this.selectedItem).AssemblyDefinition.MainModule.FilePath;

				case TreeNodeType.AssemblyModuleDefinition:
					return ((IAssemblyModuleDefinitionTreeViewItem)this.selectedItem).ModuleDefinition.FilePath;

				default:
					return string.Empty;
			}
		}

		private IObfuscatedFile UpdateObfuscationFileWithOptions(De4dotWrapper de4Dot, string location, string newFile)
		{
			ObfuscatedFile.Options options = new ObfuscatedFile.Options();
			options.ControlFlowDeobfuscation = this.EnableControlFlowDeobfuscation;
			options.NewFilename = newFile;
			options.KeepObfuscatorTypes = this.KeepObfuscatorClasses;
			options.Filename = location;
			var context = new ModuleContext();
			ObfuscatedFile result = de4Dot.CreateObfuscationFile(options, context);
			return result;
		}
	}
}

