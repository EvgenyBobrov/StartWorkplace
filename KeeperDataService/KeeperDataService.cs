using System;
using System.Collections.Generic;
using Android.App;
using Android.Util;
using System.Threading;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace KeeperData
{
	[Service]
	[IntentFilter(new String[]{"com.vector.KeeperDataService"})]
	public class KeeperDataService : Service
	{
		private KeeperDataServiceBinder _binder;

		private Dictionary<string, WorkingContext> _info = new Dictionary<string, WorkingContext>();

		//private int jobCounter = 0;

		public const string DataChangedIntent = "DataChanged";

		public KeeperDataService ()
		{
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			//Log.Debug ("KeeperService", "OnStartCommand" );

			/*Toast.MakeText (this, "KeeperService started", ToastLength.Long).Show ();
			var task = new Thread (() =>
								{
									DoWork();
								}
							);

			task.Start ();*/

			return StartCommandResult.Sticky;
		}

		public void DoWork()
		{
			//jobCounter++;
			Thread.Sleep (5000);
			//Log.Debug ("KeeperService", "Finished job № " + jobCounter.ToString());
			StopSelf ();
		}

		public override void OnDestroy ()
		{
			//Log.Debug ("KeeperService", "OnDestroy" );
			base.OnDestroy ();
			//Toast.MakeText (this, "KeeperService stoppped", ToastLength.Long).Show ();

		}

		public override Android.OS.IBinder OnBind (Intent intent)
		{
			_binder = new KeeperDataServiceBinder (this);
			//Toast.MakeText (this, "KeeperService binded", ToastLength.Long).Show ();
			//Log.Debug ("KeeperService", "OnBind" );

			return _binder;
		}

		public override bool OnUnbind (Intent intent)
		{
			//Toast.MakeText (this, "KeeperService unbinded", ToastLength.Long).Show ();
			return base.OnUnbind (intent);
		}

		public void SaveInfo (string key, WorkingContext context )
		{
			if (_info.ContainsKey (key))
			{
				_info [key] = context;
			}
			else
			{
				_info.Add (key, context);
			}	

			var task = new Thread (() =>
			{
				Thread.Sleep(100);
				var updateIntent = new Intent (DataChangedIntent);

				SendOrderedBroadcast (updateIntent, null);
			}
			);

			task.Start ();
		}

		public WorkingContext GetInfo(string key)
		{
			if (_info.ContainsKey (key))
			{
				return _info[key];
			}

			return null;
		}

		public void DeleteInfo(string key)
		{
			if (_info.ContainsKey (key))
			{
				_info.Remove(key);
			}
		}

	}
}

