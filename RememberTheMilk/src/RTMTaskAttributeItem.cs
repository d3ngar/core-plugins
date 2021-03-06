// RTMTaskAttributeItem.cs
// 
// Copyright (C) 2009 GNOME Do
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
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

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	/// <summary>
	/// Item class for all the generic task attributes.
	/// </summary>
	public class RTMTaskAttributeItem : Item, IUrlItem
	{
		string name;
		string description;
		string icon;
		string url;
		RTMTaskItem parent;
		
		public RTMTaskAttributeItem (string name, string description, string url, RTMTaskItem parent) :
			this (name, description, url, "rtm.png@", parent)
		{
		}
		
		public RTMTaskAttributeItem (string name, string description, string url, string icon, RTMTaskItem parent)
		{
			this.name = name;
			this.description = description;
			this.url = url;
			this.icon = icon;
			this.parent = parent;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return description;}
		}
		
		public override string Icon {
			get {
				if (icon.EndsWith("@"))
					return icon + GetType ().Assembly.FullName;
				else
					return icon;
			}
		}
		
		public string Url {
			get { return url; }
		}

		public RTMTaskItem Parent {
			get { return parent; }
		}
	}
}
