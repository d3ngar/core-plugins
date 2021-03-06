/* VolumeItemSource.cs
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;
using Mono.Addins;
using Do.Universe;

namespace VolumeControl
{
	public class VolumeItemSource : ItemSource
	{
		List<Item> items;
		
		public VolumeItemSource ()
		{
			items = new List<Item> ();
		}
			
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Volume Actions"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Adjust your system volume"); }
		}
		
		public override string Icon {
			get { return "audio-card"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type [] {
				typeof (VolumeDownItem),
				typeof (VolumeMaximizeItem),
				typeof (VolumeMuteItem),
				typeof (VolumeUnmuteItem),
				typeof (VolumeUpItem), };
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return items; }
		}

		
		public override void UpdateItems ()
		{
			items.Clear ();
			foreach (Item vitem in VolumeItems)
				items.Add (vitem);
		}
		
		private Item [] VolumeItems {
			get { return new Item [] {
				new VolumeDownItem (),
				new VolumeMaximizeItem(),
				new VolumeMuteItem (),
				new VolumeUnmuteItem (),
				new VolumeUpItem (), };
			}
		}

		
		
	}
}
