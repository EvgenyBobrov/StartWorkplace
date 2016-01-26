using System;
using System.Collections;
using System.Collections.Generic;

namespace KeeperData
{
	public class WorkingContext : Java.Lang.Object
	{
		public KeeperDataServiceConnection ServiceConnection { get; set; }

		public Object Data { get; set; }
	}
}

