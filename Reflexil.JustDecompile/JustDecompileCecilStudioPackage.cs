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
using JustDecompile.API;
using Mono.Cecil;
using Reflexil.Plugins.CecilStudio;
using Reflexil.Utils;
using Reflexil.Handlers;
using Reflexil.Plugins;
using System.Windows.Forms;

namespace Reflexil.JustDecompile
{
	internal class JustDecompileCecilStudioPackage : CecilStudioPackage
	{
        public static event EventHandler HandleItemRequest = delegate { };

        public JustDecompileCecilStudioPackage() { }

		public override object ActiveItem
		{
			get
			{
				return this.SelectedTreeViewItem;
			}
		}
		public ITreeViewItem SelectedTreeViewItem { get; set; }
		public void Delete()
		{
            HandleItemRequest(this, EventArgs.Empty);

			this.DeleteMember(this, EventArgs.Empty);
		}

		public void Rename()
		{
            HandleItemRequest(this, EventArgs.Empty);

			this.RenameItem(this, EventArgs.Empty);
		}

		internal AssemblyDefinition GetCurrentAssemblyDefinition()
		{
            HandleItemRequest(this, EventArgs.Empty);

			return base.GetCurrentAssemblyDefinition();
		}

		internal string GetProductTitle()
		{
			return REFLEXIL_WINDOW_TEXT;
		}

		internal string GetProductVersion()
		{
			return this.REFLEXIL_BUTTON_TEXT;
		}

		internal void Inject(EInjectType injectType)
		{
            HandleItemRequest(this, EventArgs.Empty);

			base.Inject(injectType);
		}

		internal void ReloadAssembly()
		{
            HandleItemRequest(this, EventArgs.Empty);

			this.ReloadAssembly(this, EventArgs.Empty);

            ////string originallocation = GetCurrentModuleOriginalLocation();

            ////if (MessageBox.Show("Are you sure to reload assembly, discarding all changes?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            ////{
            ////    IAssemblyContext context = PluginFactory.GetInstance().ReloadAssemblyContext(originallocation);
            ////    if (context != null)
            ////    {
            ////        //return context.AssemblyDefinition;
            ////    }
            ////}
            //////return null;

            ////IHandler handler = PluginFactory.GetInstance().Package.ActiveHandler;
            ////if (handler != null && handler.IsItemHandled(ActiveItem))
            ////{
            ////    handler.HandleItem(ActiveItem);
            ////}
		}

		protected override void ActiveItemChanged(object sender, EventArgs e)
		{
		}
	}
}