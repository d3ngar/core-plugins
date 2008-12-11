/* TwitterPreferences.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *  
 * This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
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
using Do.Platform;

namespace Twitter
{
	
	public class TwitterPreferences
	{
		#region contants and default values
		
		const string MicroblogServiceKey = "Service";
		const string ShowNotificationsKey = "ShowFriendUpdates";
		
		const string MicroblogServiceDefault = "twitter";
		const bool ShowNotificationsDefault = true;
		
		#endregion
	
		IPreferences prefs;
		
		public TwitterPreferences()
		{
			prefs = Services.Preferences.Get <TwitterPreferences> ();
		}
		
		public bool ShowNotifications {
			get { return prefs.Get<bool> (ShowNotificationsKey, ShowNotificationsDefault); }
			set { prefs.Set<bool> (ShowNotificationsKey, value); }
		}
		
		public string MicroblogService {
			get { return prefs.Get<string> (MicroblogServiceKey, MicroblogServiceDefault); }
			set { prefs.Set<string> (MicroblogServiceKey, value); }
		}
	}
}
