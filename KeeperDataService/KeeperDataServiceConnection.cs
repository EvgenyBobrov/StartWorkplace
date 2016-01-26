using System;
using Android.Util;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.App;

namespace KeeperData
{
	public class KeeperDataServiceConnection : Java.Lang.Object, IServiceConnection
	{
		private KeepingDataContextActivity _activity;

		public KeeperDataServiceBinder KeeperServiceBinder { get; private set; }

		public KeeperDataServiceConnection (KeepingDataContextActivity activity)
		{
			_activity = activity;
		}

		public void OnServiceConnected (ComponentName name, IBinder binder)
		{
			//Log.Debug ("KeeperService", "OnServiceConnected" );
			//Toast.MakeText (_activity, "KeeperService connected", ToastLength.Long).Show ();
			KeeperServiceBinder = binder as KeeperDataServiceBinder;
			if (KeeperServiceBinder != null)
			{
				_activity.OnServiceConnected();
/*				_activity.IsBound = true;
				if (_activity.WorkingContext != null)
				{
					KeeperServiceBinder.GetService ().SaveInfo (_activity.WorkingContext.Info);
				}*/
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			//_activity.IsBound = false;
			/*Log.Debug ("KeeperService", "OnServiceDisconnected" );
			Toast.MakeText (_activity, "OnServiceDisconnected", ToastLength.Short).Show ();

			//_activity = null;*/
		}

	}
}

