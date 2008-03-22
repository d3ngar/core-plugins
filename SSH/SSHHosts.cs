/* SSHHosts.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Universe;

namespace GnomeDoSSH {

	public class HostItem : IOpenableItem {
		string name;

		public HostItem (string hostname)
		{
			name = hostname;
		}

		public string Name { get { return name; } }
		public string Description { get { return "SSH Host"; } }
		public string Icon { get { return "gnome-globe"; } }

		public string Text { get { return name; } }

		public void Open ()
		{
			string uri = "ssh://" + name;
			System.Diagnostics.Process.Start ("nautilus", uri);
		}
	}

	public class SSHHostItemSource : IItemSource {
		List<IItem> items;

		public SSHHostItemSource ()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}

		public string Name { get { return "SSH Hosts"; } }
		public string Description { get { return "Parses ssh-config"; } }
		public string Icon { get { return "network-server"; } }

		public Type[] SupportedItemTypes {
			get {
				return new Type[] { typeof (HostItem) };
			}
		}

		public ICollection<IItem> Items {
			get { return items; }
		}

		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;  
		}

		public void UpdateItems ()
		{
			items.Clear ();
			try {
				string hostsFile = Environment.GetEnvironmentVariable ("HOME") + "/.ssh/config";
				FileStream fs = new FileStream (hostsFile, FileMode.Open, FileAccess.Read);
				StreamReader reader = new StreamReader (fs);

				Regex r = new Regex ("^\\s*Host\\s+([^ ]+)\\s*$");

				string s;
				while ((s = reader.ReadLine ()) != null) {
					Match m = r.Match (s);
					if (m.Groups.Count == 2) {
						items.Add (new HostItem (m.Groups [1].ToString ()));
					}
				}
			} catch { }
		}
	}
}
