
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
	[Activity (Label = "Выбор команды лебёдки")]			
	public class SelectWinchCommandActivity : KeepingDataContextActivity
	{
		private Spinner _spnWinch;
		private Spinner _spnWinchOperator;
		private Spinner _spnWinchAssistant;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.SelectWinchCommand);
			// Create your application here
			_spnWinch = FindViewById<Spinner>(Resource.Id.spnSelectWinch);
			_spnWinchOperator = FindViewById<Spinner>(Resource.Id.spnSelectWinchOperator);
			_spnWinchAssistant = FindViewById<Spinner>(Resource.Id.spnSelectWinchAssistant);

			var winchList = new List<string> () { "" };
			_dataAccessor.GetWinches ().ForEach ((winch) => {winchList.Add(winch.Name);} );
			var winchAdapter = new ArrayAdapter<string> (this, 
				//Android.Resource.Layout.SimpleSpinnerItem, 
				Resource.Layout.SimpleListRow, 
				winchList.ToArray());
			_spnWinch.Adapter = winchAdapter;

			var emploeeList = new List<string> () { "" };
			emploeeList.AddRange (_dataAccessor.GetEmploees().Select(e => e.CallSing).ToList());

			var operatorAdapter = new ArrayAdapter<string> (this, 
//				Android.Resource.Layout.SimpleSpinnerItem, 
				Resource.Layout.SimpleListRow, 
				emploeeList.ToArray());
			_spnWinchOperator.Adapter = operatorAdapter;

			var assistantAdapter = new ArrayAdapter<string> (this, 
				//Android.Resource.Layout.SimpleSpinnerItem, 
				Resource.Layout.SimpleListRow, 
				emploeeList.ToArray());
			_spnWinchAssistant.Adapter = assistantAdapter;

			var buttonOk = FindViewById<Button> (Resource.Id.btnSelectWinchCommand);
			buttonOk.TextSize = 24;
			buttonOk.Click += delegate
			{
				OnClick();
			};

			var buttonCancel = FindViewById<Button> (Resource.Id.btnCancelWinchCommand);
			buttonCancel.TextSize = 24;
			buttonCancel.Click += delegate
			{
				base.OnBackPressed();
			};
		}

		private void OnClick()
		{
			DeleteDataFromStorage (DataKeeperKeys.SELECTED_WINCH_TEAM);

			var winch = _dataAccessor.GetWinches ()
				.Where (w => w.Name == _spnWinch.SelectedItem.ToString()).FirstOrDefault ();

			var winchOperator = _dataAccessor.GetEmploees ()
				.Where (w => w.CallSing == _spnWinchOperator.SelectedItem.ToString()).FirstOrDefault ();

			var winchAssistant = _dataAccessor.GetEmploees ()
				.Where (w => w.CallSing == _spnWinchAssistant.SelectedItem.ToString()).FirstOrDefault ();

			if (winch == null || winchOperator == null)
			{
				var errorText = FindViewById<TextView>(Resource.Id.textError);
				errorText.Text = "Лебёдка и оператор должны быть выбраны!";
				return;
			}

			var winchCommand = new WinchCommand()
				{Id = 0, Winch = winch, WinchMaster = winchOperator, WinchAssistant = winchAssistant};

			WorkingContext.Data = winchCommand;

			SaveDataToStorage (DataKeeperKeys.SELECTED_WINCH_TEAM, WorkingContext);
			var intent = new Intent(this, typeof(StartDayActivity));
			StartActivity(intent);
		}

		private void OnCancel()
		{
			base.OnBackPressed();
		}
	}
}

