using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using VectorDataLayer;

namespace StartWorkplace
{
	public class StartCommandAdapter : BaseAdapter<StartCommandViewTableRow>
	{
		private Activity _activity;
		private List<StartCommandViewTableRow> _items;
		private ListView _listView;

		public StartCommandAdapter (Activity activity, List<StartCommandViewTableRow> items, ListView listView)
		{
			_activity = activity;
			_items = items;
			_listView = listView;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override StartCommandViewTableRow this[int position]
		{
			get { return _items [position]; }
		}

		public override int Count { get { return _items.Count; } }

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = _items [position];
			View view = convertView;

			if (view == null)
			{
				view = _activity.LayoutInflater.Inflate (Resource.Layout.StartCommandTableRow, null);
				view.SetBackgroundResource (Android.Resource.Color.HoloOrangeLight);
				view.Click += delegate
				{

					if (item.Selected)
					{
						view.SetBackgroundResource (Android.Resource.Color.HoloOrangeLight);
						item.Selected = false;
					} else
					{
						view.SetBackgroundResource ((Android.Resource.Color.HoloOrangeDark));
						item.Selected = true;
					}
				};
			}
			else
			{
				if (item.Selected)
					view.SetBackgroundResource ((Android.Resource.Color.HoloOrangeDark));
				else
					view.SetBackgroundResource (Android.Resource.Color.HoloOrangeLight);
			}

			if (_listView.LayoutParameters != null)
			{
				var width = _listView.LayoutParameters.Width /2;
				var emploee = view.FindViewById<TextView> (Resource.Id.textStarterEmploee);
				var pos = view.FindViewById<TextView> (Resource.Id.textStarterPosition);

				emploee.SetWidth (width);
				pos.SetWidth (width);
				//emploee.LayoutParameters = new ViewGroup.LayoutParams(width, _listView.LayoutParameters.Height);
				//pos.LayoutParameters = new ViewGroup.LayoutParams(width, ViewGroup.LayoutParams.MatchParent);
			}
			view.FindViewById<TextView> (Resource.Id.textStarterEmploee).Text = item.Emploee.CallSing;
			view.FindViewById<TextView> (Resource.Id.textStarterPosition).Text = item.Position.GetStartWorkerPositionName();


			return view;
		}

		public List<StartCommandViewTableRow> GetSelected()
		{
			return _items.Where (i => i.Selected).ToList ();
		}

		public void RemoveItem (StartCommandViewTableRow item)
		{
			_items.Remove (item);
			NotifyDataSetChanged();
		}		
	}
}

