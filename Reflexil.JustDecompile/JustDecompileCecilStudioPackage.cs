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
using JustDecompile.Core;
using Mono.Cecil;
using Reflexil.Plugins.CecilStudio;
using Reflexil.Utils;

namespace Reflexil.JustDecompile
{
	internal class JustDecompileCecilStudioPackage : CecilStudioPackage
	{
		public JustDecompileCecilStudioPackage()
		{
		}

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
			this.DeleteMember(this, EventArgs.Empty);
		}

		public void Rename()
		{
			this.RenameItem(this, EventArgs.Empty);
		}

		internal AssemblyDefinition GetCurrentAssemblyDefinition()
		{
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
			base.Inject(injectType);
		}

		internal void ReloadAssembly()
		{
			this.ReloadAssembly(this, EventArgs.Empty);
		}

		protected override void ActiveItemChanged(object sender, EventArgs e)
		{
		}
	}
}