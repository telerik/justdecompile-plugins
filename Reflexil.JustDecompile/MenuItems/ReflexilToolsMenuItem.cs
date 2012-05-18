using System;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Reflexil.Forms;

namespace Reflexil.JustDecompile
{
    internal class ReflexilToolsMenuItem : MenuItem
    {
        private readonly IRegionManager regionManager;
        private readonly ReflexilWindow reflexilWindow;
        private readonly ReflexilHost reflexilHost;

        public ReflexilToolsMenuItem(IRegionManager regionManager, ReflexilWindow reflexilWindow)
        {
            this.Command = new DelegateCommand(OnClickExecuted);

            this.regionManager = regionManager;

            this.Header = "Reflexil";

            this.reflexilWindow = reflexilWindow;

            this.reflexilHost = new ReflexilHost(regionManager, this.reflexilWindow);
        }

        private void OnClickExecuted()
        {
            if (!regionManager.Regions["PluginRegion"].Views.Contains(reflexilHost))
            {
                regionManager.AddToRegion("PluginRegion", reflexilHost);
            }
        }
    }
}
