/* IgnorePathNodeView.cs
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
using Mono.Unix;

using Gtk;

namespace Do.FilesAndFolders
{
	
	// TODO: update this class to use spin buttons,
	public class IgnorePathNodeView : PathNodeView
	{
		public enum Column {
			Path = 0,
			Index,
			NumColumns
		}
		
		public IgnorePathNodeView () : base ()
		{
			CellRenderer cell;
			RulesHint = true;
			HeadersVisible = true;
			
			Model = new ListStore (typeof (string), typeof (bool));

			cell = new CellRendererText ();
			(cell as CellRendererText).Width = 310;
			(cell as CellRendererText).Ellipsize = Pango.EllipsizeMode.Middle;
			AppendColumn (Catalog.GetString ("Folder"), cell, "text", Column.Path);
			
			Refresh ();
		}
		
		public void Refresh()
		{
			base.Refresh(false);
		}
	}
}