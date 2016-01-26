
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KeeperData
{
	[BroadcastReceiver]
	public class WorkContextReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			var activity = context as KeepingDataContextActivity;
			if (activity != null)
			{
				activity.OnChangeWorkingContext ();
			}
		}
	}
}

