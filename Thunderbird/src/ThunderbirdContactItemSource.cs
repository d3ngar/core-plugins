//  ThunderbirdContactItemSource.cs
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
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

// 

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Do.Universe;

using Beagle.Util;

namespace Do.Addins.Thunderbird
{

	public class ThunderbirdContactItemSource : ItemSource
	{

	  public class EmailContactDetail: Item, IContactDetailItem
	  {
	  	readonly string detail;
		readonly ContactItem owner;

	  	public  EmailContactDetail(ContactItem owner, string detail)
	  	{
	  		this.owner  = owner;
	  		this.detail = detail;
	  	}

	  	public override string Name 
	  	{
		  get
		  {
			return owner[detail];
			// return AddinManager.CurrentLocalizer.GetString ("Email");
		  }
	  	}

	  	public override string Description
	  	{
	  	  get { return Value; }
	  	}

	  	public override string Icon
	  	{
		  get { return "thunderbird"; }
	  	  // get { return "stock_person"; }
	  	  // get { return "stock_mail-compose"; }
	  	}

	  	public string Key {
	  	  get { return detail; }
	  	}

	  	public string Value {
	  	  get { return owner[detail]; }
	  	}
	  }
		
		const string BeginProfileName    = "Path=";
		const string BeginDefaultProfile = "Name=default";
		const string EMAIL_COUNTER       = "thunderbird.counter";
		const string THUNDERBIRD_EMAIL   = "email.thunderbird";
		
		Dictionary<string, Item> contacts; // name => ContactItem
		
		public ThunderbirdContactItemSource ()
		{
		    contacts = new Dictionary<string, Item> ();
			// UpdateItems ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}
		
		public override string Name { get { return "Thunderbird Contacts"; } }
		public override string Description { get { return "Thunderbird Contacts"; } }
		public override string Icon { get { return "thunderbird"; } }
		
		public override void UpdateItems ()
		{
			try {
				_UpdateItems ();
			} catch (Exception e) {
				Console.Error.WriteLine ("Cannot index Thunderbird contacts because a {0} was thrown: {1}", e.GetType (), e.Message);
				return;
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return contacts.Values; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
		  ContactItem contact = item as ContactItem;
		  Console.Error.WriteLine("ParentItem: {0}[\"email\"]/{1}", contact["name"], contact["email"]);
		  foreach (string detail in contact.Details)
			{
			  Console.Error.WriteLine("{0} = {1}", detail, contact[detail]);
			}

		  foreach (string detail in contact.Details)
		  {
		  	if (detail.StartsWith(THUNDERBIRD_EMAIL))// && detail != "email")
		  	  {
		  		// ContactItem nextEmail = ContactItem.CreateWithName(contact["name"]);
		  		// nextEmail["email"] = contact[detail];
		  		Console.Error.WriteLine("ChildItem: {0}[{1}]={2}", contact["name"], detail, contact[detail]);
		  		yield return new EmailContactDetail(contact, detail);
		  	  }
		  }
		  yield break;
		}
		
		void _UpdateItems ()
		{
 		    Console.Error.WriteLine("_UpdateItems");
		    MorkDatabase database, history;

			contacts.Clear ();

			database = new MorkDatabase (GetThunderbirdAddressBookFilePath ());
			database.Read ();
			database.EnumNamespace = "ns:addrbk:db:row:scope:card:all";

			history = new MorkDatabase (GetThunderbirdHistoryFilePath ());
			history.Read ();
			history.EnumNamespace = "ns:addrbk:db:row:scope:card:all";

			addContacts(history);
			addContacts(database);
			foreach (ContactItem item in contacts.Values)
			  {
				foreach (string detail in item.Details)
				  {
					Console.Error.WriteLine("{0} = {1}", detail, item[detail]);
				  }
			  }
		}

		void addContacts(MorkDatabase database)
		{
			foreach (string id in database) {
				Hashtable contact_row;
				ContactItem contact;
				
				contact_row = database.Compile (id, database.EnumNamespace);
				contact = CreateThunderbirdContactItem (contact_row);
				if (contact == null)
				  continue;
				
				string name  = contact["name"];
				string email = contact["email"];
				// int i = 0;
				// if (contacts.ContainsKey(name) && 
				// 	email != null && email != string.Empty)
				// {
				//     contact = contacts[name] as ContactItem;
				// 	i = Convert.ToUInt16(contact[EMAIL_COUNTER]) + 1;
				// 	contact[EMAIL_COUNTER] = i.ToString();
				// 	name = name + " " + i;
				// 	contact = ContactItem.CreateWithName(name);
				// 	contact["email"] = email;
				// 	Console.Error.WriteLine("About to add {0}[{1}]/{2}, EMAIL_COUNTER={3}",
				// 							name, i, email, contact[EMAIL_COUNTER]);
				// 	// int i = 0;
				// 	// foreach (string detail in contact.Details) {
				// 	//   if (detail.StartsWith("email"))
				// 	// 	i++;
				// 	// }
				// 	// string detailN = "email." + i;
				// 	// contact[detailN] = email;
				// }
				if (!contacts.ContainsKey(name.ToLower()))
				  {
					contacts.Add(name.ToLower(), contact);
				  }
				Console.Error.WriteLine("Added: {0}[{1}]/{2}", name, contact[EMAIL_COUNTER], email);
				// if (contacts.ContainsKey(name))
				// {
				//     contact = contacts[name] as ContactItem;
				// 	int i = 0;
				// 	foreach (string detail in contact.Details) {
				// 	  if (detail.StartsWith("email"))
				// 		i++;
				// 	}
				// 	string detailN = "email." + i;
				// 	contact[detailN] = email;
				// 	Console.Error.WriteLine("Added: {0}[{1}]/{2}", name, i, contact[detailN]);
				// }
				// else
				// {
				//   contacts.Add(name, contact);
				//   // Console.Error.WriteLine("First contact: {0}", name ?? "unknown");
				//   Console.Error.WriteLine("Added: {0}[0]/{1}", name, contact["email"]);
				// }
			}
		}
	
		ContactItem CreateThunderbirdContactItem (Hashtable row) {
		  ContactItem contact;
			string name, email;
			
//			foreach (object o in row.Keys)
//				Console.WriteLine ("\t{0} --> {1}", o, row[o]);
			
			// I think this will detect deleted contacts... Hmm...
			if (row["table"] == null || row["table"] as string == "C6")
				return null;
			
			// Name
			name = row["DisplayName"] as string;
			if (name == null || name == string.Empty)
				name = string.Format ("{0} {1}", row["FirstName"], row["LastName"]);
			
			// Email
			email = row["PrimaryEmail"] as string;
			
			if (name == null || name.Trim() == string.Empty)
			  name = email;

			int i = 0;
			// Item sameContact;
			// contacts.TryGetValue(name.ToLower(), out sameContact);
			// contact = sameContact as ContactItem;
			contact = ContactItem.Create (name);
			// string detail = "email";
			// if (null != contact)//  && 
			// 	// contact["email"] != null && contact["email"] != string.Empty)
			//   {

			Console.Error.WriteLine("Found {0}, num_email={1}", name, contact[EMAIL_COUNTER]);
			i = Convert.ToUInt16(contact[EMAIL_COUNTER]) + 1;
			contact[EMAIL_COUNTER] = i.ToString();

				// name = name + " " + i;
			  // }
			string detail = THUNDERBIRD_EMAIL + "." + i;

			Console.Error.WriteLine("Creating: {0}[{1}]/{2}", name, detail, email);
			if (email != null && email != string.Empty)
			{
			  contact[detail] = email;
			  // if (null == contact["email"] || string.Empty == contact["email"])
			  // 	contact["email"] = email;
			}
			// contact[EMAIL_COUNTER] = i.ToString();
			
			return contact;
		}

		string GetThuderbirdDefaultProfilePath()
		{
			string home, path, profile;
			StreamReader reader;

			profile = null;
			home = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			path = System.IO.Path.Combine (home, ".thunderbird/profiles.ini");
			try {
				reader = System.IO.File.OpenText (path);
			} catch {
				return null;
			}
			
			bool got_default = false;
			for (string line = reader.ReadLine (); line != null; line = reader.ReadLine ()) {
				if (got_default && line.StartsWith (BeginProfileName)) {
					line = line.Trim ();
					line = line.Substring (BeginProfileName.Length);
					profile = line;
					break;
				}
				else if (line.StartsWith (BeginDefaultProfile)) {
					got_default = true;
				}
			}
			reader.Close ();
			return profile;
		}

		string GetThunderbirdFilePath (string filename)
		{
		  string path, home, profile;
		  home = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
		  profile = GetThuderbirdDefaultProfilePath();
		  if (profile == null) {
			return null;
		  }
		  path = System.IO.Path.Combine (home, ".thunderbird");
		  path = System.IO.Path.Combine (path, profile);
		  path = System.IO.Path.Combine (path, filename);
		  return path;
		}
		
		string GetThunderbirdHistoryFilePath ()
		{
		  return GetThunderbirdFilePath("history.mab");
		}

		string GetThunderbirdAddressBookFilePath ()
		{
 		  return GetThunderbirdFilePath("abook.mab");
		}
		
	}
}
