
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
using VectorDictionary;

namespace StartWorkplace
{
	public class SelectWinchCommandDialog : DialogFragment
	{
		private Spinner _spnWinch;
		private Spinner _spnWinchOperator;
		private Spinner _spnWinchAssistant;
		private View _dialog;
		private IDataAccessor _dataAccessor;

		public delegate void RegWinchEventHandler (object sender, WinchCommandArgs args);
		public event RegWinchEventHandler Registred;

		public override Dialog OnCreateDialog (Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog (savedInstanceState);
			dialog.SetTitle ("Лебёдка");
			return dialog;
		}

		public override View OnCreateView (	LayoutInflater inflater, 
			ViewGroup container, 
			Bundle savedInstanceState)
		{

			_dialog = inflater.Inflate(Resource.Layout.SelectWinchCommand, container, false);

			_dataAccessor = new DataAccessorFactory ().GetDataAccessor ();

			_spnWinch = _dialog.FindViewById<Spinner>(Resource.Id.spnSelectWinch);
			_spnWinchOperator = _dialog.FindViewById<Spinner>(Resource.Id.spnSelectWinchOperator);
			_spnWinchAssistant = _dialog.FindViewById<Spinner>(Resource.Id.spnSelectWinchAssistant);

			var winchList = new List<string> () { "" };
			_dataAccessor.GetWinches ().ForEach ((winch) => {winchList.Add(winch.Name);} );
			var winchAdapter = new ArrayAdapter<string> (_dialog.Context, 
				Resource.Layout.SimpleListRow, 
				winchList.ToArray());
			_spnWinch.Adapter = winchAdapter;

			var emploeeList = new List<string> () { "" };
			emploeeList.AddRange (_dataAccessor.GetEmploees().Select(e => e.CallSing).ToList());

			var operatorAdapter = new ArrayAdapter<string> (_dialog.Context, 
				Resource.Layout.SimpleListRow, 
				emploeeList.ToArray());
			_spnWinchOperator.Adapter = operatorAdapter;

			var assistantAdapter = new ArrayAdapter<string> (_dialog.Context, 
				Resource.Layout.SimpleListRow, 
				emploeeList.ToArray());
			_spnWinchAssistant.Adapter = assistantAdapter;

			var buttonOk = _dialog.FindViewById<Button> (Resource.Id.btnSelectWinchCommand);
			buttonOk.Click += delegate
			{
				OnClick();
			};

			var buttonCancel = _dialog.FindViewById<Button> (Resource.Id.btnCancelWinchCommand);
			buttonCancel.Click += delegate
			{
				Dismiss();
			};

			return _dialog;
		}

		private void OnClick()
		{
			var winch = _dataAccessor.GetWinches ()
				.Where (w => w.Name == _spnWinch.SelectedItem.ToString()).FirstOrDefault ();

			var winchOperator = _dataAccessor.GetEmploees ()
				.Where (w => w.CallSing == _spnWinchOperator.SelectedItem.ToString()).FirstOrDefault ();

			var winchAssistant = _dataAccessor.GetEmploees ()
				.Where (w => w.CallSing == _spnWinchAssistant.SelectedItem.ToString()).FirstOrDefault ();

			if (winch == null || winchOperator == null)
			{
				var errorText = _dialog.FindViewById<TextView>(Resource.Id.textError);
				errorText.Text = "Лебёдка и оператор должны быть выбраны!";
				return;
			}

			var winchCommand = new WinchCommand()
				{Id = 0, Winch = winch, WinchMaster = winchOperator, WinchAssistant = winchAssistant};

			Dismiss ();
			if (Registred != null)
			{
				var args = new WinchCommandArgs (){ WinchCommand = winchCommand };
				Registred (this, args);
			}
		}

		private void OnCancel()
		{
			Dismiss();
		}
	}
}

