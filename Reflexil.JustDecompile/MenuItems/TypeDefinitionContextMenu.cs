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
using Microsoft.Practices.Prism.Events;
using Reflexil.Utils;

namespace Reflexil.JustDecompile.MenuItems
{
	internal class TypeDefinitionContextMenu : MenuItemBase
	{
		public TypeDefinitionContextMenu(IEventAggregator eventAggregator) : base(eventAggregator)
		{
		}

		public override void AddMenuItems()
		{
			base.AddMenuItems();

			this.MenuItems.Add(new MenuSeparator());
			this.MenuItems.Add(new MenuItem { Header = "Inject event", Command = new DelegateCommand(InjectEvent) });
			this.MenuItems.Add(new MenuItem { Header = "Inject field", Command = new DelegateCommand(InjectField) });
			this.MenuItems.Add(new MenuItem { Header = "Inject method", Command = new DelegateCommand(InjectMethod) });
			this.MenuItems.Add(new MenuItem { Header = "Inject constructor", Command = new DelegateCommand(InjectConstructor) });
			this.MenuItems.Add(new MenuItem { Header = "Inject property", Command = new DelegateCommand(InjectProperty) });
			this.MenuItems.Add(new MenuSeparator());

			this.AddRenameDeleteNodes();
		}

		private void InjectConstructor()
		{
			StudioPackage.Inject(EInjectType.Constructor);
		}

		private void InjectEvent()
		{
			StudioPackage.Inject(EInjectType.Event);
		}

		private void InjectField()
		{
			StudioPackage.Inject(EInjectType.Field);
		}

		private void InjectMethod()
		{
			StudioPackage.Inject(EInjectType.Method);
		}

		private void InjectProperty()
		{
			StudioPackage.Inject(EInjectType.Property);
		}
	}
}