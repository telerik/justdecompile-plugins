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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Reflexil.Forms;

namespace Reflexil.JustDecompile
{
	internal class ReflexilToolsMenuItem : MenuItem
	{
		private readonly ReflexilHost reflexilHost;
		private readonly ReflexilWindow reflexilWindow;
		private readonly IRegionManager regionManager;
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