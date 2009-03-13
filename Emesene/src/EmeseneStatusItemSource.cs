using System;
using Do.Universe;
using System.Collections.Generic;

namespace Do.Universe
{
	public class EmeseneStatusItemSource : ItemSource
	{
		
		public EmeseneStatusItemSource()
		{
		}

		public override void UpdateItems ()
		{
			return;
		}
		
		public override string Name { get { return "Emesene Status"; } }
		public override string Description { get { return "Avaliable emesene status."; }}
		public override string Icon {get { return "emesene"; } }
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get { yield return typeof (EmeseneStatusItem);}
		}
		
		public override IEnumerable<Item> Items 
		{
			get { return Emesene.status; }
		}
	}
}