// RTM.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

using Mono.Addins;

using Do.Platform;
using Do.Universe;

using RtmNet;

namespace RememberTheMilk
{
	/// <summary>
	/// Contrller class to handle all RTM operations.
	/// </summary>
	public class RTM
	{
		#region [ Private Variable, Constant ]
		
		static Rtm rtm;
		static List<Item> tasks;
		static List<Item> lists;
		static List<Item> tags;
		static List<Item> locations;
		static List<Item> priorities;
		static List<Item> notes;
		static object list_lock;
		static object task_lock;
		static object location_lock;
		static object note_lock;
		static string timeline;
		static DateTime last_sync;
		static string username;
		static string filter;
		static uint overdue_timer;
		
		static string RTMIconPath = "rtm.png@" + typeof (RTMListItemSource).Assembly.FullName;
		
		const string ApiKey = "ee32c06f2d45baf935a2c046323457d8";
		const string SharedSecret = "1b835b123a903938";

		#endregion [ Private Properties, Constant ]
		
		RTM ()
		{
		}
		
		static RTM ()
		{
			rtm = new Rtm (ApiKey, SharedSecret);
			tasks = new List<Item> ();
			lists = new List<Item> ();
			tags = new List<Item> ();
			locations = new List<Item> ();
			priorities = new List<Item> ();
			notes = new List<Item> ();
			list_lock = new object ();
			task_lock = new object ();
			location_lock = new object ();
			note_lock = new object ();
			
			ResetLastSync ();
			ResetFilter ();
			
			RTMPreferences.AccountChanged += HandleAccountChanged;
			RTMPreferences.FilterChanged += HandleFilterChanged;
			RTMPreferences.OverdueIntervalChanged += HandleOverdueIntervalChanged;
			RTMPreferences.OverdueNotificationChanged += HandleOverdueNotificationChanged;
			
			Services.Core.UniverseInitialized += HandleInitialized;
			
			TryConnect ();
		}
		
		#region [ Authentication ]
		
		//// <value>
		/// If we are authorized to communicate with RTM server.
		/// </value>
		public static bool IsAuthenticated {
			get { return  (rtm.IsAuthenticated && !String.IsNullOrEmpty (rtm.AuthToken)); }
		}
		
		/// <summary>
		/// Initialize the authorization, open a URL where the user can agree 
		/// the operation this plugin will perform on his/her RTM account.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> indicating the frob value.
		/// </returns>
		public static string AuthInit ()
		{
			string frob;
			try {
				frob = rtm.AuthGetFrob ();
			} catch (RtmException e) {
				Log<RTM>.Error (AddinManager.CurrentLocalizer.GetString ("Failed to initialize authentication."), e.Message);
				return "";
			}
			Services.Environment.OpenUrl(rtm.AuthCalcUrl (frob, AuthLevel.Delete));
			return frob;
		}
		
		/// <summary>
		/// Complete the authorization, check the frob, retrieve and store the token as preference.
		/// </summary>
		/// <param name="frob">
		/// A <see cref="System.String"/> indicating the frab value.
		/// </param>
		/// <returns>
		/// A <see cref="Auth"/>
		/// </returns>
		public static Auth AuthComplete (string frob)
		{
			Auth auth;
			try {
				auth = rtm.AuthGetToken (frob);
			} catch (RtmException e) {
				Log<RTM>.Error (AddinManager.CurrentLocalizer.GetString ("Failed to complete authentication."), e.Message);
				return null;
			}
			rtm.AuthToken = auth.Token;
			timeline = rtm.TimelineCreate ();
			return auth;
		}
		
		#endregion [ Authentication ]
		
		#region [ Public Properties ]
		
		// If not the initial updating of the universe, we'd like the universe manager
		// to pick up the new items only after the update functions have done their jobs
		// so locks are used to ensure the synchronization between threads.

		/// <value>
		/// All list retrieved from the RTM account plus 4 meta list
		/// </value>
		public static List<Item> Lists {
			get {
				if (last_sync == DateTime.MinValue)
					return lists;
				else
					lock (list_lock) return lists;
			}
		}
		
		/// <value>
		/// All tasks retrieved from the RTM account
		/// </value>
		public static List<Item> Tasks {
			get {
				if (last_sync == DateTime.MinValue)
					return tasks;
				else
					lock (task_lock) return tasks;
			}
		}
		
		/// <value>
		/// All locations retrieved from the RTM account.
		/// </value>
		public static List<Item> Locations {
			get {
				if (last_sync == DateTime.MinValue)
					return locations;
				else
					lock (location_lock) return locations;
			}
		}
		
		/// <value>
		/// All tags retrieved from the RTM account.
		/// </value>
		public static List<Item> Tags {
			get { return tags; }
		}
		
		/// <value>
		/// A preset list of <see cref="RTMPriorityItem"/>
		/// </value>
		public static List<Item> Priorities
		{
			get { return priorities; }
		}
		
		#endregion [ Public Properties ]
		
		#region [ Relational Search ]
		
		/// <summary>
		/// Finds all tasks in a given list indicated by the list's Id.
		/// </summary>
		/// <param name="listId">
		/// A <see cref="System.String"/> indicating the Id of the list.
		/// </param>
		/// <returns>
		/// A <see cref="List"/> of <see cref="Item"/> containing the found tasks.
		/// </returns>
		public static List<Item> TasksForList (string listId)
		{
			if (listId == "all")
				return tasks;
			else if (listId == "overdue")
				return tasks.FindAll (i => IsOverdue (i as RTMTaskItem));
			else if (listId == "today")
				return tasks.FindAll (i => IsDueToday (i as RTMTaskItem));
			else if (listId == "tomorrow")
				return tasks.FindAll (i => IsDueTomorrow (i as RTMTaskItem));
			else if (listId == "week")
				return tasks.FindAll (i => IsDueInAWeek (i as RTMTaskItem));
			else
				return tasks.FindAll (i => (i as RTMTaskItem).ListId == listId);
		}
		
		/// <summary>
		/// Finds all tasks that have a given tag.
		/// </summary>
		/// <param name="tag">
		/// A <see cref="System.String"/> indicating the tag to be searched for.
		/// </param>
		/// <returns>
		/// A <see cref="List"/> of <see cref="Item"/> containing the found tasks.
		/// </returns>
		public static List<Item> TasksForTag (string tag)
		{
			return tasks.FindAll (i => (i as RTMTaskItem).Tags.Contains (tag));
		}
		
		/// <summary>
		/// Finds all tasks that are associated with a given location.
		/// </summary>
		/// <param name="locationId">
		/// A <see cref="System.String"/> indicating the Id of the location.
		/// </param>
		/// <returns>
		/// A <see cref="List"/> of <see cref="Item"/> containing the found tasks.
		/// </returns>
		public static List<Item> TasksForLocation (string locationId)
		{
			return tasks.FindAll (i => (i as RTMTaskItem).LocationId == locationId);
		}
		
		/// <summary>
		/// Generates all <see cref="RTMTaskAttributeItem"/>s based on the available properties of a task.
		/// </summary>
		/// <param name="task">
		/// A <see cref="RTMTaskItem"/> indicating the task.
		/// </param>
		/// <returns>
		/// A <see cref="List"/> of <see cref="Item"/> containing the generated attributes.
		/// </returns>
		public static List<Item> AttributesForTask (RTMTaskItem task)
		{
			List<Item> attribute_list = new List<Item> ();
			
			if (task.Due != DateTime.MinValue) {
				attribute_list.Add (new RTMTaskAttributeItem (task.Due.ToString ((task.HasDueTime != 0) ? "g" : "d"),
					"Due Date/Time", task.Url, "stock_calendar", task));
			}
			
			if (!String.IsNullOrEmpty (task.TaskUrl)) {
				attribute_list.Add (new RTMTaskAttributeItem (task.TaskUrl, "URL", task.TaskUrl, "text-html", task));
			}
			
			if (!String.IsNullOrEmpty (task.Estimate)) {
				attribute_list.Add (new RTMTaskAttributeItem (task.Estimate, "Time Estimate", 
					task.Url, "stock_appointment-reminder", task));
			}
			
			if (!String.IsNullOrEmpty (task.LocationId))
				attribute_list.Add (locations.Find (i => (i as RTMLocationItem).Id == task.LocationId));
			
			if (!String.IsNullOrEmpty (task.Tags)) {
				attribute_list.Add (new RTMTaskAttributeItem (task.Tags, "Tags", task.Url,
					"task-tag.png@" + typeof (RTMListItemSource).Assembly.FullName, task));
			}
			
			List<Item> note_list = notes.FindAll (i => (i as RTMNoteItem).TaskId == task.Id);
			if (note_list.Any ()) {
				lock (note_lock) {
					foreach (Item item in note_list)
						attribute_list.Add (item);
				}
			}
			return attribute_list;
		}
		
		#endregion [ Relational Search ]
		
		#region [ Methods for Data Update ]
		
		/// <summary>
		/// Retrieves the list of task lists as <see cref="RTMListIem"/>s from RTM server.
		/// Also adds 4 meta lists for easy access to overdue tasks, 
		/// and tasks due today/tomorrow/in a week.
		/// </summary>
		public static void UpdateLists ()
		{
			lock (list_lock) {
				if (!IsAuthenticated && !TryConnect ())
					return;
				
				Lists rtmLists;
				try {
					rtmLists = rtm.ListsGetList ();
				} catch (RtmException e) {
					Log<RTM>.Debug (AddinManager.CurrentLocalizer.GetString ("An error occured when updating RTM lists: {0}"), 
						e.Message);
					rtmLists = null;
					return;
				}
				
				lists.Clear ();
				
				lists.Add (new RTMListItem ("today", "Today", 1, 1));
				lists.Add (new RTMListItem ("tomorrow", "Tomorrow", 1, 1));
				lists.Add (new RTMListItem ("week", "In a Week", 1, 1));
				lists.Add (new RTMListItem ("overdue", "Overdue", 1, 1));
				
				foreach (List rtmList in rtmLists.listCollection) {
					if (rtmList.Deleted == 0 && rtmList.Smart == 0)
						lists.Add (new RTMListItem (rtmList.ID, rtmList.Name, rtmList.Locked, rtmList.Smart));
				}
			}
			Log<RTM>.Debug ("Received {0} lists.", lists.ToArray ().Length);
		}
		
		/// <summary>
		/// Retrieves the list of locations as <see cref="RTMLocationItem"/>s from the RTM server.
		/// </summary>
		public static void UpdateLocations ()
		{
			lock (location_lock) {
				if (!IsAuthenticated && !TryConnect ())
					return;
			
				Locations rtmLocations;
				try {
					rtmLocations = rtm.LocationsGetList ();
				} catch (RtmException e) {
					Log<RTM>.Debug (AddinManager.CurrentLocalizer.GetString ("An error happend when updating RTM locations: {0}"), e.Message);
					rtmLocations = null;
					return;
				}
				
				locations.Clear ();
				if (rtmLocations.locationCollection.Length > 0) {
					foreach (Location rtmLocation in rtmLocations.locationCollection) {
						locations.Add (new RTMLocationItem (rtmLocation.ID, rtmLocation.Name,
							rtmLocation.Address, rtmLocation.Longitude, rtmLocation.Latitude));
					}
				}
			}
			Log<RTM>.Debug ("Received {0} locations.", locations.ToArray ().Length);
		}
		
		/// <summary>
		/// Updates the list of tasks as <see cref="RTMTaskItem"/>s from the RTM server.
		/// Also collects all the notes and tags during the update.
		/// </summary>
		public static void UpdateTasks ()
		{
			lock (task_lock) {
				if (!IsAuthenticated && !TryConnect ())
					return;
				
				Tasks rtmTasks;
				Tasks rtmTasks_sync;
				
				if (last_sync == DateTime.MinValue) {
					tasks.Clear ();
					tags.Clear ();
					notes.Clear ();
				}
				
				try {
					// If first time sync, get full list of tasks restricted by filter
					// otherwise, only do incremental sync.
					if (last_sync == DateTime.MinValue) {
						rtmTasks = rtm.TasksGetList (null, null, filter);
						rtmTasks_sync = null;
					} else {
						rtmTasks_sync = rtm.TasksGetList (null, last_sync.ToUniversalTime ().ToString ("u"), null);
						rtmTasks = rtm.TasksGetList (null, last_sync.ToUniversalTime ().ToString ("u"), filter);
					}
				} catch (RtmException e) {
					rtmTasks = null;
					rtmTasks_sync = null;
					last_sync = DateTime.MinValue;
					Log<RTM>.Debug (AddinManager.CurrentLocalizer.GetString ("An error occured when updating RTM tasks: {0}"), 
						e.Message);
					return;
				}
				
				// if not first time sync, delete all changed tasks (using the list with nothing filtered)
				if (last_sync != DateTime.MinValue) {
					foreach (List rtmList in rtmTasks_sync.ListCollection) {
						if (rtmList.DeletedTaskSeries != null) {
							foreach (TaskSeries rtmTaskSeries in rtmList.DeletedTaskSeries.TaskSeriesCollection) {
								foreach (Task rtmTask in rtmTaskSeries.TaskCollection)
									TryRemoveTask (rtmTask.TaskID);
							}
						}
						
						if (rtmList.TaskSeriesCollection != null) {
							foreach (TaskSeries rtmTaskSeries in rtmList.TaskSeriesCollection) {
								foreach (Task rtmTask in rtmTaskSeries.TaskCollection)
									TryRemoveTask (rtmTask.TaskID);
							}
						}
					}
				}
				
				// add changed tasks from the list with filter used.
				foreach (List rtmList in rtmTasks.ListCollection) {
					if (rtmList.TaskSeriesCollection != null) {
						foreach (TaskSeries rtmTaskSeries in rtmList.TaskSeriesCollection) {
							foreach (Task rtmTask in rtmTaskSeries.TaskCollection) {
								// delete one recurrent task will cause other deleted instances
								// appear in the taskseries tag, so here we need to check again.
								if (rtmTask.Deleted == DateTime.MinValue) {
									// handle tags
									string temp_tags = "";
									if (rtmTaskSeries.Tags.TagCollection.Length > 0) {
										foreach (Tag rtmTag in rtmTaskSeries.Tags.TagCollection) {
											if (tags.FindIndex (i => (i as RTMTagItem).Name == rtmTag.Text) == -1)
												tags.Add (new RTMTagItem (rtmTag.Text));
											temp_tags += rtmTag.Text + ", ";
										}
										temp_tags = temp_tags.Remove (temp_tags.Length-2);
									}
									
									// handle notes
									if (rtmTaskSeries.Notes.NoteCollection.Length > 0) {
										foreach (Note rtmNote in rtmTaskSeries.Notes.NoteCollection) {
											notes.Add (new RTMNoteItem (rtmNote.Title, rtmNote.Text, rtmNote.ID,
												String.Format ("http://www.rememberthemilk.com/print/{0}/{1}/{2}/notes/",
													username, rtmList.ID, rtmTask.TaskID), rtmTask.TaskID));
										}
									}
									
									// add new task
									RTMTaskItem new_task = new RTMTaskItem (rtmList.ID, rtmTaskSeries.TaskSeriesID,
										rtmTask.TaskID, rtmTaskSeries.Name, rtmTask.Due, rtmTask.Completed,
										rtmTaskSeries.TaskURL, rtmTask.Priority, rtmTask.HasDueTime, 
										rtmTask.Estimate, rtmTaskSeries.LocationID, temp_tags);
									tasks.Add (new_task);
								}
							}
						}
					}
				}
				last_sync = DateTime.Now;
			}
			
			Log<RTM>.Debug ("Received {0} tasks.", tasks.ToArray ().Length);
			Log<RTM>.Debug ("Received {0} notes.", notes.ToArray ().Length);
			Log<RTM>.Debug ("Received {0} tags.", tags.ToArray ().Length);
		}
		
		/// <summary>
		/// Manully generates a list containing all the <see cref="RTMPrioirtyItem"/>s.
		/// </summary>
		static void UpdatePriorities ()
		{
			priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("High"),
				AddinManager.CurrentLocalizer.GetString ("High Priority")));
			priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Medium"),
				AddinManager.CurrentLocalizer.GetString ("Medium Priority")));
			priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Low"),
				AddinManager.CurrentLocalizer.GetString ("Low Priority")));
			priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("None"),
				AddinManager.CurrentLocalizer.GetString ("No Priority")));
			priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Up"),
				AddinManager.CurrentLocalizer.GetString ("Increase the priority")));
			priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Down"),
				AddinManager.CurrentLocalizer.GetString ("Decrease the priority")));
		}
		
		#endregion [ Methods for Data Update ]
		
		#region [ Task Actions ]
		
		public static RTMTaskItem NewTask (string listId, string taskData)
		{
			List rtmList;
			
			try {
				rtmList = rtm.TasksAdd (timeline, taskData, listId, true);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return null;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("New Task Created"),
				AddinManager.CurrentLocalizer.GetString ("The task has been successully added to your Remember The milk task list."));
			
			return new RTMTaskItem (rtmList.ID, rtmList.TaskSeriesCollection[0].TaskSeriesID,
				rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID,
				rtmList.TaskSeriesCollection[0].Name,
				rtmList.TaskSeriesCollection[0].TaskCollection[0].Due,
				rtmList.TaskSeriesCollection[0].TaskCollection[0].Completed,
				rtmList.TaskSeriesCollection[0].TaskURL,
				rtmList.TaskSeriesCollection[0].TaskCollection[0].Priority,
				rtmList.TaskSeriesCollection[0].TaskCollection[0].HasDueTime,
				rtmList.TaskSeriesCollection[0].TaskCollection[0].Estimate,
				rtmList.TaskSeriesCollection[0].LocationID, "");
		}
		
		public static void DeleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksDelete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Deleted"),
				AddinManager.CurrentLocalizer.GetString ("The selected task has been successfully deleted from your Remember The Milk task list"));
		}

		public static void CompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksComplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Completed"),
				AddinManager.CurrentLocalizer.GetString ("The selected task in your Remember The Milk task list has been marked as completed."));
		}
		
		public static void SetTaskPriority (string listId, string taskSeriesId, string taskId, string priority)
		{
			try {
				if (priority == "up" || priority == "down")
					rtm.TasksMovePriority (timeline, listId, taskSeriesId, taskId, priority);
				else
					rtm.TasksSetPriority (timeline, listId, taskSeriesId, taskId, priority);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Priority Changed"),
				AddinManager.CurrentLocalizer.GetString ("The priority of the selected task in your Remember The Milk task list has been changed."));
		}
		
		public static void SetDueDateTime (string listId, string taskSeriesId, string taskId, string due)
		{
			try {
				if (String.IsNullOrEmpty (due))
					rtm.TasksSetDueDate (timeline, listId, taskSeriesId, taskId);
				else
					rtm.TasksSetDueDateParse (timeline, listId, taskSeriesId, taskId, due);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			if (String.IsNullOrEmpty (due)) 
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Due Date/Time Unset"),
					AddinManager.CurrentLocalizer.GetString ("The due date/time of the selected task in your Remember The Milk task list has been unset."));
			else 
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Due Date/Time Changed"),
					AddinManager.CurrentLocalizer.GetString ("The due date/time of the selected task in your Remember The Milk task list has been changed."));
		}
		
		public static void MoveTask (string fromListId, string toListId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksMoveTo (timeline, fromListId, toListId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Moved"),
				AddinManager.CurrentLocalizer.GetString (String.Format ("The selected task has been moved from Remember The Milk list \"{0}\" to list \"{1}\".",
					lists.Find (i => (i as RTMListItem).Id == fromListId).Name, 
					lists.Find (i => (i as RTMListItem).Id == toListId).Name)));
		}
		
		public static void RenameTask (string listId, string taskSeriesId, string taskId, string newName)
		{
			try {
				rtm.TasksSetName (timeline, listId, taskSeriesId, taskId, newName);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Renamed"),
				String.Format (AddinManager.CurrentLocalizer.GetString ("The selected task has been renamed to \"{0}\"."), 
					newName));
		}
		
		public static void PostponeTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksPostpone (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Postponed"),
				AddinManager.CurrentLocalizer.GetString ("The selected task in your Remember The Milk task list has been postponed"));
		}
		
		public static void SetRecurrence (string listId, string taskSeriesId, string taskId, string repeat)
		{
			try {
				rtm.TasksSetRecurrence (timeline, listId, taskSeriesId, taskId, repeat);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Recurrence Pattern Changed"),
				AddinManager.CurrentLocalizer.GetString ("The recurrence pattern of the selected task in your Remember The Milk task list has been changed."));
		}
		
		public static void SetURL(string listId, string taskSeriesId, string taskId, string url)
		{
			try {
				rtm.TasksSetUrl(timeline, listId, taskSeriesId, taskId, url);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			if (!string.IsNullOrEmpty(url)) {
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task URL Set"),
					AddinManager.CurrentLocalizer.GetString ("The selected task has been assigned a URL."));
			} else {
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task URL Reset"),
					AddinManager.CurrentLocalizer.GetString ("The URL for the selected task has been reset."));
			}
		}
		
		public static void SetEstimateTime (string listId, string taskSeriesId, string taskId, string estimateTime)
		{
			try {
				rtm.TasksSetEstimateTime(timeline, listId, taskSeriesId,
				                         taskId, estimateTime);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			if (String.IsNullOrEmpty (estimateTime))
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Estimated Time Unset"),
					AddinManager.CurrentLocalizer.GetString ("The estimated time for the selected task has been unset."));
			else
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Estimated Time Set"),
					AddinManager.CurrentLocalizer.GetString ("The selected task has been assigned an estimated time."));
		}
		
		public static void SetLocation (string listId, string taskSeriesId, string taskId, string locationId)
		{
			try {
				rtm.TasksSetLocation (timeline, listId, taskSeriesId, taskId, locationId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			if (string.IsNullOrEmpty (locationId)) {
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Location Reset"),
					AddinManager.CurrentLocalizer.GetString ("The location of the selected task has been cleared."));
			} else {
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Location changed"),
					AddinManager.CurrentLocalizer.GetString ("The location of the selected task has been successfully changed."));
			}
			
		}
		
		public static void UncompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksUncomplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task Uncompleted"),
				AddinManager.CurrentLocalizer.GetString ("The selected task has been marked as \"incomplete\"."));
		}
		
		#endregion [ Task Actions ]
		
		#region [ List Actions ]
		
		public static void NewList (string newListName)
		{
			if (IsProtectedList (newListName)) {
				Services.Notifications.Notify ("Invalid List Name", "The provided new list name is reserved.", RTMIconPath);
				return;
			}
			
			try {
				rtm.ListsNew (timeline, newListName);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("New List Created"),
				String.Format (AddinManager.CurrentLocalizer.GetString ("A new task list named \"{0}\" has been created."), 
					newListName));
		}
		
		public static void DeleteList(string listId)
		{
			try {
				rtm.ListsDelete(timeline, listId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("List Deleted"),
				AddinManager.CurrentLocalizer.GetString ("The selected task list has been deleted."));
		}      
		
		public static void RenameList (string listId, string newListName)
		{
			if (IsProtectedList (newListName)) {
				Services.Notifications.Notify ("Invalid List Name", "The provided new list name is reserved.", RTMIconPath);
				return;
			}
			
			try {
				rtm.ListsRename (timeline, listId, newListName);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Task List Renamed"),
				String.Format (AddinManager.CurrentLocalizer.GetString ("The selected task list has been renamed to \"{0}\"."), 
					newListName));
		}
		
		#endregion [ List Actions ]
		
		#region [ Note Actions ]
		
		public static void NewNote (string listId, string taskSeriesId, string taskId, string note)
		{
			string[] parts = null;
			string note_title;
			string note_body;
			bool has_separator = (0 < note.IndexOf ("|") && note.IndexOf ("|") < note.Length);
			bool has_newline = (0 < note.IndexOf ("\n") && note.IndexOf ("\n") < note.Length);
			bool newline_first = note.IndexOf ("\n") < note.IndexOf ("|");
			
			if ((has_newline && has_separator && newline_first) || (has_newline && !has_separator)) {
				parts = note.Split(new char[] {'\n'}, 2);
			} else if (has_separator) {
				parts = note.Split(new char[] {'|'}, 2);
			} 
			
			if (string.IsNullOrEmpty (note) || ((has_separator || has_newline) && parts != null && parts.Length < 2)) {
				Log<RTM>.Debug ("Entered text cannot be used as a note.");
				return;
			} else {
				note_title = (has_separator || has_newline) ? parts[0].Trim () : "Untitled Note";
				note_body = (has_separator || has_newline) ? parts[1].Trim () : note;
			}
			
			try {
				rtm.NotesAdd (timeline, listId, taskSeriesId, taskId, note_title, note_body);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Note Added"),
				AddinManager.CurrentLocalizer.GetString ("A note has been added to the selected task"));
		}
		
		public static void DeleteNote (string noteId)
		{
			try {
				rtm.NotesDelete (timeline, noteId);
			} catch (RtmException e) {
				Log<RTM>.Debug (e.Message);
				return;
			}

			lock (note_lock)
				notes.Remove (notes.Find (i => (i as RTMNoteItem).Id == noteId));
			
			FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Note Deleted"),
				AddinManager.CurrentLocalizer.GetString ("The selected note has been deleted from the selected task"));
		}
		
		#endregion [ Note Actions ]
		
		#region [ Tag Actions ]
		
		public static void AddTags (string listId, string taskSeriesId, string taskId, string tags)
		{
			if (String.IsNullOrEmpty (tags)) {
				Log<RTM>.Debug ("Tags to add is empty or null string.");
			} else {
				try {
					rtm.TasksAddTags (timeline, listId, taskSeriesId, taskId, tags);
				} catch (RtmException e) {
					Log<RTM>.Debug (e.Message);
					return;
				}
				
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Tags Added"),
					AddinManager.CurrentLocalizer.GetString ("New tags have been successfully added to the selected task."));
			}
		}

		public static void DeleteTags (string listId, string taskSeriesId, string taskId, string tags)
		{
			if (String.IsNullOrEmpty (tags)) {
				Log<RTM>.Debug ("[RememberTheMilk] Tags to delete is empty or null string.");
			} else {
				try {
					rtm.TasksRemoveTags (timeline, listId, taskSeriesId, taskId, tags);
				} catch (RtmException e) {
					Log<RTM>.Debug (e.Message);
					return;
				}
				
				FinalizeAction (AddinManager.CurrentLocalizer.GetString ("Tags Deleted"),
					AddinManager.CurrentLocalizer.GetString ("Selected tags have been successfully removed from the selected task."));
			}
		}
		
		#endregion [ Tag Actions ]
		
		#region [ Utilities ]
		
		/// <summary>
		/// Initializes the connection to RTM server.
		/// Verify the stored token with RTM server and if valid also creates the timeline.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, true if the connection is successfully established,
		/// false if there is a problem during verfication or timeline creation.
		/// </returns>
		static bool TryConnect ()
		{
			if (!Services.Network.IsConnected)
				return false;
			
			if (!String.IsNullOrEmpty (RTMPreferences.Token)) {
				Auth auth;
				try {
					auth = rtm.AuthCheckToken (RTMPreferences.Token);
				} catch (RtmException e) {
					Log<RTM>.Error (AddinManager.CurrentLocalizer.GetString ("Token verification failed."), e.Message);
					return false;
				}
				
				rtm.AuthToken = auth.Token;
				username = auth.User.Username;
				
				try {
					timeline = rtm.TimelineCreate ();
				} catch (RtmException e) {
					Log<RTM>.Error (AddinManager.CurrentLocalizer.GetString ("Remember The Milk timeline creation failed."), e.Message);
					return false;
				}
				
				return true;
			} else {
				Log<RTM>.Error (AddinManager.CurrentLocalizer.GetString ("Not authorized to use a Remember The Milk account."));
				return false;
			}
		}
		
		/// <summary>
		/// Try to remove a <see cref="RTMTaskItem"/> from the task list. 
		/// All <see cref="RTMNoteItem"/>s related to the task are also removed from the note list.
		/// </summary>
		/// <param name="taskId">
		/// A <see cref="System.String"/> indicating the Id of the task to be removed.
		/// </param>
		static void TryRemoveTask (string taskId)
		{
			lock (task_lock)
				tasks.RemoveAll (i => (i as RTMTaskItem).Id == taskId);
			lock (note_lock)
				notes.RemoveAll (i => (i as RTMNoteItem).TaskId == taskId);
		}
		
		/// <summary>
		/// Show a notification if there is any overdue task.
		/// </summary>
		static void NotifyOverdueTasks ()
		{
			List<Item> overdue_tasks;
			object list_lock = new object ();
			overdue_tasks = new List<Item> ();
			lock (task_lock)
				overdue_tasks = tasks.FindAll (i => IsOverdue (i as RTMTaskItem));      
			
			int len = overdue_tasks.ToArray ().Length;
			if (len > 0) {
				string title;
				title = String.Format (AddinManager.CurrentLocalizer.GetPluralString ("{0} Task Overdue", 
							"{0} Tasks Overdue", len), len);
				
				string body = "";
				lock (list_lock) {
					foreach (Item item in overdue_tasks)
						body += ("- " + (item as RTMTaskItem).Name +"\n");
				}
				Services.Notifications.Notify (new Notification (title, body, 
					"task-overdue.png@" + typeof (RTM).Assembly.FullName));
			}
		}
		
		/// <summary>
		/// Check if a task is overdue.
		/// </summary>
		/// <param name="item">
		/// A <see cref="RTMTaskItem"/> indicating the task to be checked.
		/// </param>
		/// <returns>
		/// Ignored
		/// </returns>
		static bool IsOverdue (RTMTaskItem item)
		{
			return (item.Completed == DateTime.MinValue && item.Due > DateTime.MinValue
				 && ((item.HasDueTime == 1 && item.Due < DateTime.Now) ||  item.Due.Date < DateTime.Today));
		}
		
		/// <summary>
		/// Check if a task is due today
		/// </summary>
		/// <param name="item">
		/// A <see cref="RTMTaskItem"/> indicating the task to be checked
		/// </param>
		/// <returns>
		/// Ignored
		/// </returns>
		static bool IsDueToday (RTMTaskItem item)
		{
			return (item.Completed == DateTime.MinValue && item.Due.Date == DateTime.Today);
		}
		
		/// <summary>
		/// Check if a task is due tomorrow
		/// </summary>
		/// <param name="item">
		/// A <see cref="RTMTaskItem"/> indicating the task to be checked
		/// </param>
		/// <returns>
		/// Ignored
		/// </returns>
		static bool IsDueTomorrow (RTMTaskItem item)
		{
			return (item.Completed == DateTime.MinValue && item.Due.Date == DateTime.Today.AddDays (1.0));
		}
		
		/// <summary>
		/// Check if a task is due in next 7 days.
		/// </summary>
		/// <param name="item">
		/// A <see cref="RTMTaskItem"/> indicating the task to be checked.
		/// </param>
		/// <returns>
		/// Ignored
		/// </returns>
		static bool IsDueInAWeek (RTMTaskItem item)
		{
			return (item.Completed == DateTime.MinValue && item.Due != DateTime.MinValue
				&& item.Due.Date <= DateTime.Today.AddDays (6.0));
		}
		
		/// <summary>
		/// Check if the give list name is protected from assigning to other list.
		/// </summary>
		/// <param name="listName">
		/// A <see cref="System.String"/> indicating the list name to be checked .
		/// </param>
		/// <returns>
		/// Ignored
		/// </returns>
		static bool IsProtectedList (string listName)
		{
			Item item = lists.Find (i => (i as RTMListItem).Name == listName);
			if (item != null)
				return (item as RTMListItem).Locked;
			return false;
		}
		
		static void FinalizeAction (string title, string body,  bool updateTasks,
		                            bool updateLists, bool updateLocations)
		{
			if (RTMPreferences.ActionNotification)
				Services.Notifications.Notify (new Notification (title, body, RTMIconPath));
			if (updateLists)
				UpdateLists ();
			if (updateTasks)
				UpdateTasks ();
			if (updateLocations)
				UpdateLocations ();
		}
		
		static void FinalizeAction (string title, string body)
		{
			FinalizeAction (title, body, false, false, false);
		}
		
		/// <summary>
		/// Reset the timer for notifying overdue task.
		/// </summary>
		static void ResetOverdueTimer ()
		{
			if (overdue_timer != 0)
				GLib.Source.Remove (overdue_timer);
			if (RTMPreferences.OverdueNotification)
				overdue_timer = GLib.Timeout.Add ((uint) RTMPreferences.OverdueInterval * 60 * 1000,
					() => { NotifyOverdueTasks (); return true; });
		}
		
		/// <summary>
		/// Reset the <see cref="filter"/> to the current preference setting,
		/// make sure we have 'status' defined in the string.
		/// </summary>
		static void ResetFilter ()
		{
			filter = RTMPreferences.Filter;
			
			if (String.IsNullOrEmpty (filter))
				filter = "status:incomplete";
			else if (!filter.Contains ("status:"))
				filter = "status:incomplete OR (" + filter + ")";
		}
		
		/// <summary>
		/// Reset the last synchronization timestamp.
		/// </summary>
		static void ResetLastSync ()
		{
			last_sync = DateTime.MinValue;
		}
		
		#endregion [ Utilities ]
		
		#region [ Event Handlers ]

		/// <summary>
		/// Handles when RTM account is changed
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		static void HandleAccountChanged (object sender, EventArgs e)
		{
			ResetLastSync ();
		}

		/// <summary>
		/// Handles when Filter preference is changed.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		static void HandleFilterChanged (object sender, EventArgs e)
		{
			ResetLastSync ();
			ResetFilter ();
		}
		
		/// <summary>
		/// Handles when overdue notification interval preference is changed.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		static void HandleOverdueIntervalChanged (object sender, EventArgs e)
		{
			ResetOverdueTimer ();
		}
		
		/// <summary>
		/// Handles when show overdue notification preference is changed.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		static void HandleOverdueNotificationChanged (object sender, EventArgs e)
		{
			ResetOverdueTimer ();
		}
		
		/// <summary>
		/// Handles when the <see cref="UniverseManger"/> is initialized.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		static void HandleInitialized (object sender, EventArgs e)
		{
			Services.Core.UniverseInitialized -= HandleInitialized;
			UpdatePriorities ();
			ResetOverdueTimer ();
		}
		
		#endregion
	}
}
