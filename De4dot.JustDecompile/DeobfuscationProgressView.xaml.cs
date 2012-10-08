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

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using JustDecompile.API.Core.Services;
using de4dot.code;
using de4dot.code.renamer;
using Microsoft.Practices.Prism.Events;

namespace De4dot.JustDecompile
{
    public partial class DeobfuscationProgressWindow : Window
    {
        private IObfuscatedFile obfuscationfile;

        //private readonly IEventAggregator eventAggregator;
        private readonly IAssemblyManagerService assemblyManager;

        public DeobfuscationProgressWindow(IObfuscatedFile obfuscationfile, IAssemblyManagerService Assemblymanager)
        {
            this.assemblyManager = Assemblymanager;

            this.obfuscationfile = obfuscationfile;

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
            var ofiletype = obfuscationfile.GetType();
            var ofield = ofiletype.GetField("options", BindingFlags.NonPublic | BindingFlags.Instance);
            ObfuscatedFile.Options options = (ObfuscatedFile.Options)ofield.GetValue(obfuscationfile);
            options.NewFilename = newFileName;

            this.Show();

            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ReportProgress(0, "Preparing deobfuscation");
                        obfuscationfile.deobfuscateBegin();

                        ReportProgress(20, "Deobfuscating");
                        obfuscationfile.deobfuscate();

                        ReportProgress(40, "Finishing deobfuscation");
                        obfuscationfile.deobfuscateEnd();

                        ReportProgress(60, "Renaming items");
                        var renamer = new Renamer(obfuscationfile.DeobfuscatorContext, new IObfuscatedFile[] { obfuscationfile });
                        renamer.rename();

                        ReportProgress(80, "Saving");
                        obfuscationfile.save();
                    }
                    finally
                    {
                        obfuscationfile.deobfuscateCleanUp();
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
                        //this.eventAggregator.GetEvent<LoadAssemblyEvent>().Publish(newFileName);
                        this.assemblyManager.LoadAssembly(newFileName);
                        }
                    }
                    this.Close();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
