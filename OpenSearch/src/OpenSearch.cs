//  OpenSearch.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
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

using System.Collections.Generic;

namespace Do.Plugins.OpenSearch
{	
	public static class OpenSearch
	{		
		public static List<OpenSearchItem> GetOpenSearchItems ()
		{
			List<IOpenSearchFile> openSearchFiles = OpenSearchFileManager.GetOpenSearchFiles ();
				
			List<OpenSearchItem> openSearchItems = new List<OpenSearchItem> ();	
			
			foreach(IOpenSearchFile file in openSearchFiles)
			{
				try {
					OpenSearchItem item = file.CreateOpenSearchItem ();
					if (item != null)
						openSearchItems.Add (item);
				} catch {
					continue;
				}
			}
			
			return openSearchItems;
		}
		
		
		/// <summary>
		/// Brain-dead hack to fix someone elses bad code that I don't have the time to re-do
		/// </summary>
		/// <returns>
		/// A <see cref="List`1"/>
		/// </returns>
		public static void Init()
		{
			OpenSearchFileManager.GetOpenSearchFiles ();
			return;
		}
	}
}
