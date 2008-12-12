/* MicroblogClient.cs 
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
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;

using Mono.Unix;

using Twitterizer.Framework;

using Do.Platform;
using Do.Universe;
	
namespace Microblogging
{

	public class MicroblogClient
	{
		#region Class constants, error messages
		
		readonly string StatusUpdatedMsg = Catalog.GetString ("'{0}' successfully posted.");
		readonly string DownloadFailedMsg = Catalog.GetString ("Failed to fetch file from {0}");
		readonly string NoUpdatesMsg = Catalog.GetString ("No new microblog status updates found.");
		readonly string GenericErrorMsg = Catalog.GetString ("Twitter encountered an error in {0}");
		readonly string MissingCredentialsMsg = Catalog.GetString ("Missing login credentials. Please set login "
			+ "information in plugin configuration.");
		
		readonly string FailedPostMsg = Catalog.GetString ("Unable to post tweet. Check your login settings. If you "
			+ "are behind a proxy make sure that the settings in /system/http_proxy are correct.");

		const string ErrorIcon = "dialog-warning";
		const int UpdateTimelineTimeout = 30 * 1000;
		const int UpdateContactsTimeout = 10 * 60 * 1000;

		#endregion

		Twitter blog;
		string username;
		DateTime last_updated;
		readonly string PhotoDirectory;

		public IEnumerable<IItem> Contacts { get; private set; }
		
		public MicroblogClient (string username, string password, Service service)
		{
			last_updated = DateTime.UtcNow;
			Contacts = Enumerable.Empty<IItem> ();
			blog = new Twitter (username, password, service);
			PhotoDirectory = new [] {Paths.UserData, "Microblogging", "photos"}.Aggregate (Path.Combine);

			GLib.Timeout.Add (UpdateContactsTimeout, UpdateContacts);
			GLib.Timeout.Add (UpdateTimelineTimeout, UpdateTimeline);
		}

		/// <value>
		/// The ContactItem key that contains the user's microblogging screen name
		/// </value>
		public static string ContactKeyName {
			get { return "microblog.screenname"; }
		}

		/// <summary>
		/// Update your miroblogging status
		/// </summary>
		/// <param name="status">
		/// A <see cref="System.String"/> status message
		/// </param>
		public void UpdateStatus (string status)
		{
			string errorMessage = "";
			try {
				blog.Status.Update (status);
				
			} catch (TwitterizerException e) {
				errorMessage = FailedPostMsg;
					
				Log.Error (string.Format (GenericErrorMsg, "Post"), e.Message);
				Log.Debug (e.StackTrace);
			}

			OnStatusUpdated (status, errorMessage);
		}

		bool UpdateContacts ()
		{
			ContactItem newContact;
			List<IItem> newContacts;
			TwitterUserCollection friends;
			
			try {
				newContacts = new List<IItem> ();
				friends = blog.User.Friends ();
			} catch (TwitterizerException e) {
				Log.Error (string.Format (GenericErrorMsg, "UpdateFriends"), e.Message);
				Log.Debug (e.StackTrace);
				return true;
			}
				
			foreach (TwitterUser friend in friends) {
				newContact = ContactItem.Create (friend.ScreenName);
				newContact[ContactKeyName] = friend.ScreenName;
				
				if (File.Exists (Paths.Combine (PhotoDirectory, "" + friend.ID)))
					newContact["photo"] = Paths.Combine (PhotoDirectory,  "" + friend.ID);
				else
					DownloadBuddyIcon (friend.ProfileImageUri, friend.ID);
				
				newContacts.Add (newContact);
			}

			Contacts = newContacts;
			return true;
		}

		bool UpdateTimeline ()
		{
			string icon = "";
			TwitterStatus tweet;
			
			try {
				// get the most recent update
				tweet = blog.Status.FriendsTimeline () [0];
			} catch (TwitterizerException e) {
				Log.Error (string.Format (GenericErrorMsg, "UpdateTimeline"), e.Message);
				Log.Debug (e.StackTrace);
				return true;
			} catch (IndexOutOfRangeException) {
				Log.Debug (NoUpdatesMsg);
				return true;
			}
				
			if (tweet.TwitterUser.ScreenName.Equals (username) || tweet.Created <= last_updated) return true;
			
			if (File.Exists (Path.Combine (PhotoDirectory, "" + tweet.TwitterUser.ID)))
				icon = Path.Combine (PhotoDirectory, "" + tweet.TwitterUser.ID);
			else
				DownloadBuddyIcon (tweet.TwitterUser.ProfileImageUri, tweet.TwitterUser.ID);
			
			last_updated = tweet.Created;
			
			OnTimelineUpdated (tweet.TwitterUser.ScreenName, tweet.Text, icon);
			return true;
		}

		void DownloadBuddyIcon (Uri imageUri, int userId)
		{
			string imageDestination;
			imageDestination = Path.Combine (PhotoDirectory, "" + userId);
			if (!Directory.Exists (PhotoDirectory)) Directory.CreateDirectory (PhotoDirectory);
			
			using (WebClient client = new WebClient ()) 
			{
				try {
					client.DownloadFile (imageUri.AbsoluteUri, imageDestination);
				} catch (Exception e) {
					Log.Error (string.Format (DownloadFailedMsg, imageUri.AbsoluteUri), e.Message);
					Log.Debug (e.StackTrace);
				}
			}
		}

		protected virtual void OnStatusUpdated (string status, string errorMessage)
		{
			if (StatusUpdated != null)
				StatusUpdated (this, new StatusUpdatedEventArgs (status, errorMessage));
		}

		protected virtual void OnTimelineUpdated (string screenname, string status, string icon)
		{
			if (TimelineUpdated != null)
				TimelineUpdated (this, new TimelineUpdatedEventArgs (screenname, status, icon));
		}

		public event StatusUpdatedDelegate StatusUpdated;
		public event TimelineUpdatedDelegate TimelineUpdated;
		
		public delegate void StatusUpdatedDelegate (object sender, StatusUpdatedEventArgs args);
		public delegate void TimelineUpdatedDelegate (object sender, TimelineUpdatedEventArgs args);
	}
}