
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
	[Activity (Label = "Регистрация пилота")]			
	public class RegisterPilotActivity : KeepingDataContextActivity
	{
		private Spinner _lvQualification;
		private Spinner _lvWeight;

		private int currentStartNum = 0;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.RegisterPilotForm);

			currentStartNum = Intent.GetIntExtra (DataKeeperKeys.CURRENT_START_NUMBER, 1);

			_lvQualification = FindViewById<Spinner> (Resource.Id.spnRegLevel);
			var qualificationList =  new List<string> () { "" };
			qualificationList.Add (PilotQualificationEnum.Newer.GetPilotQualificationName());
			qualificationList.Add (PilotQualificationEnum.Skilled.GetPilotQualificationName());
			var qualificationAdapter = new ArrayAdapter<string> (this, 
				Resource.Layout.SimpleListRow, 
				qualificationList.ToArray());
			_lvQualification.Adapter = qualificationAdapter;


			_lvWeight = FindViewById<Spinner> (Resource.Id.spnRegWeight);
			var weightList =  new List<string> () { "" };
			weightList.Add (EnumPilotWeight.UltraLight.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.Light.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.Medium.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.Heavy.GetPilotWeightName());
			weightList.Add (EnumPilotWeight.UltraHeavy.GetPilotWeightName());
			var weightAdapter = new ArrayAdapter<string> (this, 
				Resource.Layout.SimpleListRow, 
				weightList.ToArray());
			_lvWeight.Adapter = weightAdapter;

			var curNumView = FindViewById<TextView> (Resource.Id.txtOrdNum);
			curNumView.Text = string.Format("№  {0}", currentStartNum.ToString());

			var btnRegister = FindViewById<Button> (Resource.Id.btnRegisterPilot);
			btnRegister.TextSize = 24;
			btnRegister.Click += delegate
			{
				OnRegister();
			};

			var btnCancel = FindViewById<Button> (Resource.Id.btnCancelReg);
			btnCancel.TextSize = 24;
			btnCancel.Click += delegate
			{
				base.OnBackPressed();
			};

		}

		private void OnRegister()
		{
			DeleteDataFromStorage (DataKeeperKeys.REGISTER_PILOT);

			var pilot = new PilotOnStart () 
				{
				GliderInfo = FindViewById<EditText>(Resource.Id.dfGlider).Text,
				Qualification = 
					DictionaryExtensions.GetPilotQualificationByName(_lvQualification.SelectedItem.ToString()),
				Weight = DictionaryExtensions.GetPilotWeightByName(_lvWeight.SelectedItem.ToString()),
				RentFixer = FindViewById<CheckBox>(Resource.Id.cbRentFixer).Checked,
				RentGlider = FindViewById<CheckBox>(Resource.Id.cbRentGlider).Checked,
				RentRadio  = FindViewById<CheckBox>(Resource.Id.cbRentRadio).Checked,
				RentTandem = FindViewById<CheckBox>(Resource.Id.cbRentTandem).Checked,
				Sign = FindViewById<EditText>(Resource.Id.dfSign).Text,
				StartNumber = currentStartNum,
				};
			WorkingContext.Data = pilot;

			SaveDataToStorage (DataKeeperKeys.REGISTER_PILOT, WorkingContext);
			var intent = new Intent(this, typeof(WorkListActivity));
			intent.PutExtra (CREATOR_BUNDLE_KEY, DataKeeperKeys.REGISTER_PILOT);
			StartActivity(intent);

		}
	}
}

