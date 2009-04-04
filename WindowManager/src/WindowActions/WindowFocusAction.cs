//  
//  Copyright (C) 2009 GNOME Do
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.Linq;

using Do.Universe;
using Do.Interface.Wink;

using Wnck;
using Mono.Unix;

namespace WindowManager
{
	
	public class WindowFocusAction : WindowActionAction
	{
		public override string Name {
			get { return Catalog.GetString ("Focus"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Focus selected windows."); }
		}

		public override string Icon {
			get { return "go-jump"; }
		}

		public override void Action (IEnumerable<Window> windows)
		{
			WindowControl.FocusWindows (windows);
		}
	}
}
