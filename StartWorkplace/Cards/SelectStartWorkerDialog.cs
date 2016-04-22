
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
	public class SelectStartWorkerDialog : DialogFragment
	{
		private Spinner _spnEmploee;
		private Spinner _spnPosition;
		private View _dialog;
		private IDataAccessor _dataAccessor;


		public delegate void RegStartEmploeeEventHandler (object sender, StartEmploeeArgs args);

		public event RegStartEmploeeEventHandler Registred;

		public override Dialog OnCreateDialog (Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog (savedInstanceState);
			dialog.SetTitle ("Регистрация работника старта");
			return dialog;
		}

		public override View OnCreateView (	LayoutInflater inflater, 
			ViewGroup container, 
			Bundle savedInstanceState)
		{

			_dialog = inflater.Inflate(Resource.Layout.SelectStarterWorker, container, false);
			// Create your application here

			_dataAccessor = new DataAccessorFactory ().GetDataAccessor ();

			_spnEmploee = _dialog.FindViewById<Spinner>(Resource.Id.spnAddStarterEmploee);
			_spnPosition = _dialog.FindViewById<Spinner>(Resource.Id.spnAddStarterPos);

			var emploeeList = new List<string> () { "" };
			emploeeList.AddRange (_dataAccessor.GetEmploees().Select(e => e.CallSing).ToList());

			var emploeeAdapter = new ArrayAdapter<string> (_dialog.Context, 
				Resource.Layout.SimpleListRow, 
				emploeeList.ToArray());
			_spnEmploee.Adapter = emploeeAdapter;

			var positionList =  new List<string> () { "" };
			positionList.Add (EnumStarterPosition.TandemMaster.GetStartWorkerPositionName ());
			positionList.Add (EnumStarterPosition.Instructor.GetStartWorkerPositionName ());
			positionList.Add (EnumStarterPosition.Issuer.GetStartWorkerPositionName ());
			positionList.Add (EnumStarterPosition.Assistant.GetStartWorkerPositionName ());

			var positionAdapter = new ArrayAdapter<string> (_dialog.Context, 
				Resource.Layout.SimpleListRow, 
				positionList.ToArray());
			_spnPosition.Adapter = positionAdapter;

		
			var buttonOk = _dialog.FindViewById<Button> (Resource.Id.btnStarterMemberAdd);
			buttonOk.Click += delegate
			{
				OnClick();
			};

			var buttonCancel = _dialog.FindViewById<Button> (Resource.Id.btnStarterCancel);
			buttonCancel.Click += delegate
			{
				Dismiss();
			};

			return _dialog;
		}

		private void OnClick()
		{
			var emploee = _dataAccessor.GetEmploees ()
				.Where (w => w.CallSing == _spnEmploee.SelectedItem.ToString()).FirstOrDefault ();

			if (emploee == null || string.IsNullOrEmpty(_spnPosition.SelectedItem.ToString()))
			{
				var errorText = _dialog.FindViewById<TextView>(Resource.Id.textErrorAddStarter);
				errorText.Text = "Сотрудник и должность должны быть выбраны!";
				return;
			}
			var starterPosition = DictionaryExtensions
				.GetStarterPositionByName(_spnPosition.SelectedItem.ToString());

			Dismiss ();
			if (Registred != null)
			{
				var startWorker = new StartWorker (){ Emploee = emploee, Position = starterPosition };
				var args = new StartEmploeeArgs () { StartWorker = startWorker };
				Registred (this, args);
			}
		}

	}
}

