using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using VectorDictionary;

namespace StartWorkplace
{
	public class WinchTeamAdapter : BaseAdapter<WinchTeamViewTableRow>
	{
		private Activity _activity;
		private List<WinchTeamViewTableRow> _items;

		public WinchTeamAdapter (Activity context, List<WinchTeamViewTableRow> items)
		{
			_activity = context;
			_items = items;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override WinchTeamViewTableRow this[int position]
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
				view = _activity.LayoutInflater.Inflate (Resource.Layout.WinchTeamRow, null);
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

			view.FindViewById<TextView> (Resource.Id.textWinchEmploee).Text = item.EmploeeName;
			view.FindViewById<TextView> (Resource.Id.textWinchPosition).Text = item.Position.GetWinchPositionName();
			view.FindViewById<TextView> (Resource.Id.textWinchName).Text = item.WinchName;

			return view;
		}

		public List<WinchTeamViewTableRow> GetSelected()
		{
			return _items.Where (i => i.Selected).ToList ();
		}

		public void RemoveItem (WinchTeamViewTableRow item)
		{
			_items.Remove (item);
			NotifyDataSetChanged();
		}		 
	}
}

