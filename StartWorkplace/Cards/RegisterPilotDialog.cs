
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
using VectorDictionary;

namespace StartWorkplace
{
	public class RegisterPilotDialog : DialogFragment
	{
		private Spinner _lvQualification;
		private Spinner _lvWeight;
		private View _dialog;
		private const string CURR_NUM = "current_pilot_num";

		private int _currentStartNum = 0;

		public delegate void RegPilotEventHandler (object sender, RegisterPilotArgs args);

		public event RegPilotEventHandler Registred;

		public RegisterPilotDialog()
		{
		}

		public RegisterPilotDialog (int startNum)
		{
			_currentStartNum = startNum;

		}

		public override Dialog OnCreateDialog (Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog (savedInstanceState);
			dialog.SetTitle ("Регистрация пилота");
			return dialog;
		}

		public override View OnCreateView (	LayoutInflater inflater, 
												ViewGroup container, 
												Bundle savedInstanceState)
		{
			
			_dialog = inflater.Inflate(Resource.Layout.RegisterPilotForm, container, false);
			if (savedInstanceState != null)
			{
				_currentStartNum = savedInstanceState.GetInt (CURR_NUM);
			}

			_lvQualification = _dialog.FindViewById<Spinner> (Resource.Id.spnRegLevel);
			var qualificationList =  new List<string> () { "" };
			qualificationList.Add (PilotQualificationEnum.Newer.GetPilotQualificationName());
			qualificationList.Add (PilotQualificationEnum.Skilled.GetPilotQualificationName());
			var qualificationAdapter = 
				new ArrayAdapter<string> (_dialog.Context, Resource.Layout.SimpleListRow, qualificationList.ToArray());
					
			_lvQualification.Adapter = qualificationAdapter;


			_lvWeight = _dialog.FindViewById<Spinner> (Resource.Id.spnRegWeight);
			var weightList =  new List<string> () { "" };
			weightList.Add (EnumPilotWeight.UltraLight.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.Light.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.Medium.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.Heavy.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.UltraHeavy.GetPilotWeightName());
			var weightAdapter = new ArrayAdapter<string> (_dialog.Context, 
				Resource.Layout.SimpleListRow, 
				weightList.ToArray());
			_lvWeight.Adapter = weightAdapter;

			var curNumView = _dialog.FindViewById<TextView> (Resource.Id.txtOrdNum);
			curNumView.Text = string.Format("№  {0}", _currentStartNum.ToString());

			var btnRegister = _dialog.FindViewById<Button> (Resource.Id.btnRegisterPilot);
			btnRegister.Click += delegate
			{
				OnRegister();
			};

			var btnCancel = _dialog.FindViewById<Button> (Resource.Id.btnCancelReg);
			btnCancel.Click += delegate
			{
				Dismiss();
			};

			return _dialog;
		}

		private void OnRegister()
		{

			var pilot = new PilotOnStart () 
				{
				GliderInfo = _dialog.FindViewById<EditText>(Resource.Id.dfGlider).Text,
				Qualification = 
					DictionaryExtensions.GetPilotQualificationByName(_lvQualification.SelectedItem.ToString()),
				Weight = DictionaryExtensions.GetPilotWeightByName(_lvWeight.SelectedItem.ToString()),
				RentFixer = _dialog.FindViewById<CheckBox>(Resource.Id.cbRentFixer).Checked,
				RentGlider = _dialog.FindViewById<CheckBox>(Resource.Id.cbRentGlider).Checked,
				RentRadio  = _dialog.FindViewById<CheckBox>(Resource.Id.cbRentRadio).Checked,
				RentTandem = _dialog.FindViewById<CheckBox>(Resource.Id.cbRentTandem).Checked,
				Sign = _dialog.FindViewById<EditText>(Resource.Id.dfSign).Text,
				StartNumber = _currentStartNum,
				};
			Dismiss ();
			if (Registred != null)
			{
				var args = new RegisterPilotArgs () { PilotOnStart = pilot };
				Registred (this, args);
			}

		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt (CURR_NUM, _currentStartNum);
			base.OnSaveInstanceState (outState);
		}
	}
}

