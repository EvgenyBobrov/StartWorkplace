﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using VectorDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using KeeperData;
using VectorDictionary;

namespace StartWorkplace
{
	[Activity (Label = "Начало работы", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : KeepingDataContextActivity
	{
		private TextView _infoNovoe;
		private TextView _infoKonchinka;

		private List<FlightScheduleItem> _selectedScheduleItems = new List<FlightScheduleItem>();

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			StartService(new Intent(this, typeof(KeeperDataService)));

			// Get our button from the layout resource,
			// and attach an event to it
			var layoutMain = FindViewById<LinearLayout> (Resource.Id.layoutMain);
			var textFlights = FindViewById<TextView> (Resource.Id.textFilghts);
			var layoutNovoe = FindViewById<LinearLayout>(Resource.Id.layoutNovoe);
			var layoutKonchinka = FindViewById<LinearLayout>(Resource.Id.layoutKonchinka);
			_infoNovoe = FindViewById<TextView>(Resource.Id.textNovoe);
			_infoNovoe.TextSize = 20;
			_infoKonchinka = FindViewById<TextView>(Resource.Id.textKonchinka);
			_infoKonchinka.TextSize = 20;


			var buttonNovoe = FindViewById<Button> (Resource.Id.btnSelectNovoe);
			buttonNovoe.TextSize = 24;
			buttonNovoe.Click += delegate
			{
				OnClick(EnumParadrom.Novoe);
			};
			var buttonKonchinka = FindViewById<Button> (Resource.Id.btnSelectKonchinka);
			buttonKonchinka.TextSize = 24;
			buttonKonchinka.Click += delegate
			{
				OnClick(EnumParadrom.Konchinka);
			};

		}

		protected override void OnStart ()
		{
			base.OnStart ();

			ShowSchedule ();
		}

		private void ShowSchedule()
		{
			var selectNovoe = FindViewById<Button> (Resource.Id.btnSelectNovoe);
			selectNovoe.Enabled = false;
			_infoNovoe.Text = "Полёты не объявлены";
			var selectKonchinka = FindViewById<Button> (Resource.Id.btnSelectKonchinka);
			selectKonchinka.Enabled = false;
			_infoKonchinka.Text = "Полёты не объявлены";

			if (!string.IsNullOrEmpty (_dataAccessor.GetErrorMessage ()))
			{
				FindViewById<TextView> (Resource.Id.textErrorDataLayer)
					.Text = _dataAccessor.GetErrorMessage ();
				return;
			}
			var schedule = _dataAccessor.GetSchedule (DateTime.Today);
			if (!string.IsNullOrEmpty (_dataAccessor.GetErrorMessage ()))
			{
				FindViewById<TextView> (Resource.Id.textErrorDataLayer)
					.Text = _dataAccessor.GetErrorMessage ();
				return;
			}

			foreach (var flightScheduleItem in schedule)
			{
				if (flightScheduleItem.Paradrom.EnumParadrom == EnumParadrom.Novoe)
				{
					selectNovoe.Enabled = true;
					_infoNovoe.Text = "Объявлены полёты " + flightScheduleItem.FlightDate.ToString ("dd.MM.yyyy")
						+ System.Environment.NewLine + "Поле " + flightScheduleItem.CurrentField.Name;
					_selectedScheduleItems.Add (flightScheduleItem);
				}


				if (flightScheduleItem.Paradrom.EnumParadrom == EnumParadrom.Konchinka)
				{
					selectKonchinka.Enabled = true;
					_infoKonchinka.Text = "Объявлены полёты " + flightScheduleItem.FlightDate.ToString ("dd.MM.yyyy");
					_selectedScheduleItems.Add (flightScheduleItem);
				}
			}
				
		}

		public void OnClick(EnumParadrom paradromType)
		{
			var flightScheduleItem = _selectedScheduleItems.Where (i => i.Paradrom.EnumParadrom == paradromType).FirstOrDefault ();

			if (flightScheduleItem == null)
				return;
			
			if (_dataAccessor.IsDayOnParadromStarted (flightScheduleItem.Paradrom, flightScheduleItem.FlightDate))
			{
				var workingDay = _dataAccessor.ReadWorkingDay (flightScheduleItem.Paradrom, flightScheduleItem.FlightDate);
				var openContext = new WorkingContext(){ServiceConnection = null, Data = workingDay};
				SaveDataToStorage (DataKeeperKeys.CURRENT_WORK_DAY, openContext);
				var intent = new Intent(this, typeof(WorkListActivity));
				intent.PutExtra (CREATOR_BUNDLE_KEY, DataKeeperKeys.CURRENT_WORK_DAY);
				StartActivity(intent);
			} 
			else
			{
				// Показать экран начала рабочего дня
				var openContext = new WorkingContext(){ServiceConnection = null, Data = flightScheduleItem};
				SaveDataToStorage (DataKeeperKeys.SELECTED_SCHEDULE_ITEM, openContext);
				var intent = new Intent(this, typeof(StartDayActivity));
				intent.PutExtra (CREATOR_BUNDLE_KEY, DataKeeperKeys.SELECTED_SCHEDULE_ITEM);
				StartActivity(intent);
			}
		}
	}
}


