/* TakeScreenshotAction.cs
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;

using Do.Universe;

namespace GNOME {

	public class TakeScreenshotAction : Act {

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Take screenshot"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Takes a screenshot with optional delay."); }
		}

		public override string Icon {
			get { return "camera"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ScreenshotItem),
				};
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (ScreenshotDelayItem),
				};
			}
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			Item [] items = new Item [100];
			for (int i = 0; i < items.Length; ++i)
				items [i] = new ScreenshotDelayItem (i+1);
			return items;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			int seconds;
			string window;

			window = "";
			seconds = 0;

			if (items.First () is CurrentWindowScreenshotItem)
				window = "--window";

			if (modItems.Any ())
				seconds = (modItems.First () as ScreenshotDelayItem).Seconds;
				
			Process.Start ("gnome-screenshot", string.Format ("{0} --delay={1}", window, seconds));
			return null;
		}
	}
}
