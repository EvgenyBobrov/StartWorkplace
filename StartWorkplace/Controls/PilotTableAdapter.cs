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
	public class PilotTableAdapter : BaseAdapter<PilotListTableRow>
	{
		private Activity _activity;
		private List<PilotListTableRow> _items;

		public PilotTableAdapter (Activity context, List<PilotListTableRow> items)
		{
			_activity = context;
			_items = items;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override PilotListTableRow this[int position]
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
				view = _activity.LayoutInflater.Inflate (Resource.Layout.PilotListRow, null);
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

			view.FindViewById<TextView> (Resource.Id.txtPilotNum).Text = item.StartNumber.ToString();
			view.FindViewById<TextView> (Resource.Id.txtPilotSign).Text = item.PilotSign;
			view.FindViewById<TextView> (Resource.Id.txtPilotStatus).Text = item.Qualification.GetPilotQualificationName();
			view.FindViewById<TextView> (Resource.Id.txtPilotGlider).Text = item.GliderName;

			return view;
		}
	}
}

