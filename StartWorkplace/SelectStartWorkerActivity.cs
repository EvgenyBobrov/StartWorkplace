
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KeeperData;
using VectorDataLayer;

namespace StartWorkplace
{
	[Activity (Label = "Рабочий старта")]			
	public class SelectStartWorkerActivity : KeepingDataContextActivity
	{
		private Spinner _spnEmploee;
		private Spinner _spnPosition;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.SelectStarterWorker);
			// Create your application here

			_spnEmploee = FindViewById<Spinner>(Resource.Id.spnAddStarterEmploee);
			_spnPosition = FindViewById<Spinner>(Resource.Id.spnAddStarterPos);

			var emploeeList = new List<string> () { "" };
			emploeeList.AddRange (_dataAccessor.GetEmploees().Select(e => e.CallSing).ToList());

			var emploeeAdapter = new ArrayAdapter<string> (this, 
				//Android.Resource.Layout.SimpleSpinnerItem, 
				Resource.Layout.SimpleListRow, 
				emploeeList.ToArray());
			_spnEmploee.Adapter = emploeeAdapter;

			var positionList =  new List<string> () { "" };
			positionList.Add (EnumStarterPosition.TandemMaster.GetStartWorkerPositionName ());
			positionList.Add (EnumStarterPosition.Instructor.GetStartWorkerPositionName ());
			positionList.Add (EnumStarterPosition.Issuer.GetStartWorkerPositionName ());
			positionList.Add (EnumStarterPosition.Assistant.GetStartWorkerPositionName ());

			var positionAdapter = new ArrayAdapter<string> (this, 
				//Android.Resource.Layout.SimpleSpinnerItem, 
				Resource.Layout.SimpleListRow, 
				positionList.ToArray());
			_spnPosition.Adapter = positionAdapter;

		
			var buttonOk = FindViewById<Button> (Resource.Id.btnStarterMemberAdd);
			buttonOk.TextSize = 24;
			buttonOk.Click += delegate
			{
				OnClick();
			};

			var buttonCancel = FindViewById<Button> (Resource.Id.btnStarterCancel);
			buttonCancel.TextSize = 24;
			buttonCancel.Click += delegate
			{
				base.OnBackPressed();
			};
		}

		private void OnClick()
		{
			DeleteDataFromStorage (DataKeeperKeys.ADDED_STARTER_WORKER);

			var emploee = _dataAccessor.GetEmploees ()
				.Where (w => w.CallSing == _spnEmploee.SelectedItem.ToString()).FirstOrDefault ();

			if (emploee == null || string.IsNullOrEmpty(_spnPosition.SelectedItem.ToString()))
			{
				var errorText = FindViewById<TextView>(Resource.Id.textErrorAddStarter);
				errorText.Text = "Сотрудник и должность должны быть выбраны!";
				return;
			}
			var starterPosition = DictionaryExtensions
				.GetStarterPositionByName(_spnPosition.SelectedItem.ToString());

			var startWorker = new StartWorker (){ Emploee = emploee, Position = starterPosition };

			WorkingContext.Data = startWorker;

			SaveDataToStorage (DataKeeperKeys.ADDED_STARTER_WORKER, WorkingContext);
			var intent = new Intent(this, typeof(StartDayActivity));
			intent.PutExtra (CREATOR_BUNDLE_KEY, DataKeeperKeys.ADDED_STARTER_WORKER);
			StartActivity(intent);
		}

	}
}

