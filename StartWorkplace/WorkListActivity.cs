using System;
using System.Collections.Generic;
using System.Linq;
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
	[Activity (Label = "Рабочий лист")]			
	public class WorkListActivity : KeepingDataContextActivity
	{
		#region Внутренние константы и переменные
		private const string PILOTS_TAB = "Пилоты";
		private const string TANDEMS_TAB = "Тандемы";
		private const string CADET_TAB = "Курсанты";
		private const string REGISTER_PILOT_CARD_TAG = "RegisterPilot";

		private string CurrentTab = PILOTS_TAB;

		private TextView _paradromField;
		private TextView _flightMasterField;
		private TextView _frequencyField;
		private TextView _flightDataField;
		private ListView _pilotList;

		private WorkingDay _formContext = null;
		#endregion

		#region Методы формы
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.WorkingList);

			_paradromField = FindViewById<TextView> (Resource.Id.txtWorkListParadrom);

			_flightMasterField = FindViewById<TextView> (Resource.Id.txtxWorkListFM);

			_frequencyField = FindViewById<TextView> (Resource.Id.txtWorkListFreq);

			_flightDataField = FindViewById<TextView> (Resource.Id.txtWorkListData);

			_pilotList = FindViewById<ListView> (Resource.Id.lvPilotTable);

			var btnPilots = FindViewById<Button> (Resource.Id.btnPilotList);
			var btnTandems = FindViewById<Button> (Resource.Id.btnTandemList);
			btnPilots.Click += delegate 
			{
				OnClickTabButton( btnPilots);	
			};

			btnTandems.Click += delegate 
			{
				OnClickTabButton( btnTandems);	
			};

			var btnRegigister = FindViewById<Button> (Resource.Id.btnRegister);
			btnRegigister.Click += delegate
			{
				{
					OnClickRegister( );	
				};
			};
		}


		public override void OnServiceConnected ()
		{
			base.OnServiceConnected ();
			var service = GetKeeperDataService ();

			SetView ();

			if (service != null)
			{
				var workingDay = GetServiceData<WorkingDay> (DataKeeperKeys.CURRENT_WORK_DAY, false);
				if (workingDay != null)
				{
					_formContext = workingDay;
				}

				if (CreatorKey == DataKeeperKeys.REGISTER_PILOT)
				{
					var newPilot = GetServiceData<PilotOnStart> (CreatorKey, true);
					if (newPilot != null && _formContext != null)
					{
						_formContext.Pilots.Add (newPilot);
						_dataAccessor.SaveWorkingDay (_formContext);
					}
				}

				ShowData ();
			}
		}

		protected override WorkingContext LoadWorkingContext ()
		{
			var service = GetKeeperDataService ();

			var currentContext = service.GetInfo (DataKeeperKeys.CURRENT_WORK_DAY);
			if (currentContext == null)
			{
				currentContext = new WorkingContext ();
				_formContext = new WorkingDay ();
				currentContext.Data = _formContext;
			}
			else
			{
				var formContext = currentContext.Data as WorkingDay;
				if (formContext != null)
					_formContext = formContext;
				else
					_formContext = new WorkingDay ();

			}

			return currentContext;
		}

		private void ShowData()
		{
			if (_formContext != null)
			{
				_paradromField.Text = string.Format("Парадром: {0}",
					_formContext.FlightScheduleItem.Paradrom.Name);
				_flightMasterField.Text = string.Format("РП: {0}",
					_formContext.FlightDirector.CallSing);
				_frequencyField.Text = string.Format("Частота: {0}",
					_formContext.Frequency.ToString ());
				_flightDataField.Text = string.Format("Дата: {0}",
					_formContext.FlightScheduleItem.FlightDate.ToString ("dd.MM.yyyy"));

				var pilotRows = new List<PilotListTableRow> ();
				foreach (var pilot in _formContext.Pilots)
				{
					pilotRows.Add (new PilotListTableRow () {
						StartNumber = pilot.StartNumber,
						PilotSign = pilot.Sign,
						GliderName = pilot.GliderInfo,
						Qualification = pilot.Qualification,
						Selected = false
					});
				}
				_pilotList.Adapter = new PilotTableAdapter (this, pilotRows);

			}
		}

		protected override void SetView ()
		{
			var btnPilots = FindViewById<Button> (Resource.Id.btnPilotList);
			var btnTandems = FindViewById<Button> (Resource.Id.btnTandemList);
			var btnCadets = FindViewById<Button> (Resource.Id.btnCadetList);

			#region Отрисовка вкладок
			var llPilotList = FindViewById<LinearLayout> (Resource.Id.llPilotList);
			var llTandemList = FindViewById<LinearLayout> (Resource.Id.llTandemList);
			var btnRegister = FindViewById<Button> (Resource.Id.btnRegister);
			var btnReport = FindViewById<Button> (Resource.Id.btnReport);

			if (CurrentTab == PILOTS_TAB)
			{
				btnPilots.Enabled = false;
				btnTandems.Enabled = true;

				llPilotList.Visibility = ViewStates.Visible;
				llTandemList.Visibility = ViewStates.Invisible;

				btnReport.Visibility = ViewStates.Visible;
				btnReport.Text = "Отчет по пилоту";
			}
			else if (CurrentTab == TANDEMS_TAB)
			{
				btnPilots.Enabled = true;
				btnTandems.Enabled = false;

				llTandemList.Visibility = ViewStates.Visible;
				llPilotList.Visibility = ViewStates.Invisible;
				btnReport.Visibility = ViewStates.Invisible;
			}

			#endregion
		}

		protected override void InitForm ()
		{
			if (WorkingContext != null && _formContext == null)
			{
				_formContext = WorkingContext.Data as WorkingDay;
				if (_formContext != null)
					ShowData ();
			}
		}
		#endregion

		#region Обработчики событий

		private void OnClickTabButton (Button button)
		{
			if (button.Text == PILOTS_TAB)
			{
				CurrentTab = PILOTS_TAB;
			}
			else if (button.Text == TANDEMS_TAB)
			{
				CurrentTab = TANDEMS_TAB;
			}

			button.Enabled = false;

			SetView ();
		}

		private void OnClickRegister()
		{
			SaveDataToStorage (DataKeeperKeys.CURRENT_WORK_DAY, WorkingContext);

			if (CurrentTab == PILOTS_TAB)
			{
				var currentStartNum = _formContext.Pilots.Count == 0 ? 1 
					: _formContext.Pilots.Max (p => p.StartNumber) + 1;
				
				var registerDialog = new RegisterPilotDialog (currentStartNum);
				registerDialog.Registred += FinishRegistration;
				var ft = GetFragmentTransaction(REGISTER_PILOT_CARD_TAG);
				registerDialog.Show (ft, REGISTER_PILOT_CARD_TAG);
			}
		}

		public void FinishRegistration (object sender, RegisterPilotArgs args)
		{
			if (args.PilotOnStart != null && _formContext != null)
			{
				_formContext.Pilots.Add (args.PilotOnStart);
				_dataAccessor.SaveWorkingDay (_formContext);
				ShowData();
			}
		}
		#endregion
	}
}

