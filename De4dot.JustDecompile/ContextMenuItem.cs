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
using System.Windows.Input;
using System.Collections.Generic;

namespace De4dot.JustDecompile
{
    internal class ContextMenuItem
    {
        public ContextMenuItem()
        {
            Collection = new List<ContextMenuItem>();
        }

        public IList<ContextMenuItem> Collection { get; private set; }

        public ICommand Command { get; set; }

        public object Header { get; set; }

        public object Icon { get; set; }
    }
}
