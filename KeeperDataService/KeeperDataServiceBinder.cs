using System;
using Android.App;
using Android.Util;
using System.Threading;
using Android.Content;
using Android.OS;

namespace KeeperData
{
	public class KeeperDataServiceBinder : Binder
	{
		private KeeperDataService _service;

		public KeeperDataServiceBinder (KeeperDataService service)
		{
			_service = service;
		}

		public KeeperDataService GetService()
		{
			return _service;
		}
	}
}

