//  EmpathyAccountItemSource.cs
//  
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//  
//  Copyright (c) 2010 
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using Mono.Addins;
using Do.Universe;
using Do.Platform;

namespace EmpathyPlugin
{

	public class EmpathyAccountItemSource : ItemSource
	{

		List<Item> items;

		public EmpathyAccountItemSource ()
		{
			items = new List<Item> ();
		}
		
		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Empathy Accounts"); }
		}

		public override string Description 
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Available Empathy IM Accounts"); }
		}

		public override string Icon
		{
			get { return "empathy"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { 
				yield return typeof (EmpathyAccountItem); 
				yield return typeof (IApplicationItem);
				yield return typeof (EmpathyBrowseAccountItem);
			}
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (EmpathyPlugin.IsTelepathy (item)) {
				yield return new EmpathyBrowseAccountItem ();
			} else if (item is EmpathyBrowseAccountItem) {
				foreach (EmpathyAccountItem account in items) {
					yield return account;
				}
			}
		}
		
		public override IEnumerable<Item> Items
		{
			get { return items; }
		}
		
		public override void UpdateItems ()
		{
			if (EmpathyPlugin.InstanceIsRunning) {
				items.Clear ();
				try {
					foreach (Account account in EmpathyPlugin.GetAllAccounts) {
						items.Add (new EmpathyAccountItem (account));
					}
				} catch (Exception e) { 
					Log<EmpathyAccountItemSource>.Error ("Could not get Empathy accounts: {0}", e.Message);
					Log<EmpathyAccountItemSource>.Debug (e.StackTrace);
				}
			}
		}
	}
}
