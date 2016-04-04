
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
	[Activity (Label = "Начать рабочий день")]			
	public class StartDayActivity : KeepingDataContextActivity
	{
		private TextView _flightField = null;
		private TextView _dataField = null;
		private TextView _frequencyText = null;
		private EditText _frequencyField = null;
		private LinearLayout _headerLayout = null;
		private Spinner _cmbFlightDirector;
		private ListView _winchListView = null;
		private ListView _startCommandListView = null;

		private WorkingDay _formContext = null;

		#region Методы формы
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			SetContentView (Resource.Layout.StartDay);

			_flightField = FindViewById<TextView> (Resource.Id.textParadrom);
			_dataField = FindViewById<TextView> (Resource.Id.textData);
			_frequencyText = FindViewById<TextView> (Resource.Id.textFreq);
			_frequencyField = FindViewById<EditText> (Resource.Id.editFreq);

			_headerLayout = FindViewById<LinearLayout> (Resource.Id.layoutHeader);
			_cmbFlightDirector = FindViewById<Spinner>(Resource.Id.spnFlightDir);
			_winchListView = FindViewById<ListView>(Resource.Id.lvWinchTeam);
			_startCommandListView = FindViewById<ListView>(Resource.Id.lvStartCommand);

			_frequencyField.TextChanged += delegate
			{
				if (_formContext != null)
				{
					decimal freq;
					if (decimal.TryParse(_frequencyField.Text, out freq) && freq > 0)
					{
						_formContext.Frequency = freq;
					}
				}
			};

			_frequencyField.FocusChange += OnFreqFocusChanged;

			_frequencyField.Click += delegate
			{
				_frequencyField.InputType = Android.Text.InputTypes.NumberFlagDecimal;
			};

			_cmbFlightDirector.ItemSelected += delegate
			{
				if (_formContext != null)
				{
					var emploee = _dataAccessor.GetEmploees()
						.Where(e => e.CallSing == _cmbFlightDirector.SelectedItem.ToString())
						.FirstOrDefault();
					_formContext.FlightDirector = emploee;
				}
			};

			#region Кнопки
			var buttonAddWinch = FindViewById<ImageButton>(Resource.Id.btnAddWinch);
			var buttonDelWinch = FindViewById<ImageButton>(Resource.Id.btnDelWinch);

			buttonAddWinch.Click += delegate
			{
				SaveDataToStorage (DataKeeperKeys.SELECTED_TEAM, WorkingContext);

				var intent = new Intent(this, typeof(SelectWinchCommandActivity));
				StartActivity(intent);
			};

			buttonDelWinch.Click += delegate 
			{
				OnDelWinchCrewMember();	
			};

			var buttonAddStarter = FindViewById<ImageButton>(Resource.Id.btnStarterAdd);
			var buttonDelStarter = FindViewById<ImageButton>(Resource.Id.btnStarterDel);
			buttonAddStarter.Click += delegate
			{
				SaveDataToStorage (DataKeeperKeys.SELECTED_TEAM, WorkingContext);

				var intent = new Intent(this, typeof(SelectStartWorkerActivity));
				StartActivity(intent);
			};

			buttonDelStarter.Click += delegate 
			{
				OnDelStartCrewMember();	
			};

			var buttonStartDay = FindViewById<Button>(Resource.Id.btnStartDay);
			buttonStartDay.Click += delegate 
			{
				OnStartDay();	
			};

			#endregion 

		}

		public override void OnServiceConnected ()
		{
			base.OnServiceConnected ();
			var service = GetKeeperDataService ();

			if (service != null)
			{
				var startContext = service.GetInfo (DataKeeperKeys.SELECTED_SCHEDULE_ITEM);
				if (startContext != null)
				{
					var flightScheduleItem = startContext.Data as FlightScheduleItem;
					if (flightScheduleItem != null)
					{
						//WorkingContext = startContext;
						_formContext.FlightScheduleItem = flightScheduleItem;
					}
				}

				if (CreatorKey == DataKeeperKeys.SELECTED_WINCH_TEAM)
				{
					var selectWinchContext = service.GetInfo (DataKeeperKeys.SELECTED_WINCH_TEAM);
					if (selectWinchContext != null)
					{
						var winchCommand = selectWinchContext.Data as WinchCommand;
						if (winchCommand != null)
						{
							_formContext.WinchCommands.Add (winchCommand);
						}
						DeleteDataFromStorage (DataKeeperKeys.SELECTED_WINCH_TEAM);
					}
				}

				if (CreatorKey == DataKeeperKeys.ADDED_STARTER_WORKER)
				{
					var addedStartWorkerContext = service.GetInfo (DataKeeperKeys.ADDED_STARTER_WORKER);
					if (addedStartWorkerContext != null)
					{
						var addedStartWorker = addedStartWorkerContext.Data as StartWorker;
						if (addedStartWorker != null)
							_formContext.StartCommand.Add (addedStartWorker);
	
						DeleteDataFromStorage (DataKeeperKeys.ADDED_STARTER_WORKER);
					}
				}
					
				if (CreatorKey == DataKeeperKeys.CURRENT_WORK_DAY)
				{
					var workingDayContext = service.GetInfo (DataKeeperKeys.CURRENT_WORK_DAY);
					if (workingDayContext != null)
					{
						var workingDay = workingDayContext.Data as WorkingDay;
						if (workingDay != null)
							_formContext = workingDay;
					}
				}

				ShowData ();
			}
		}

		private void ShowData()
		{
			if (_formContext != null)
			{
				var emploeeList = new List<string> (){ string.Empty };
				var emploees = _dataAccessor.GetEmploees ();
				foreach (var callSign in emploees.Select(e => e.CallSing))
				{
					emploeeList.Add (callSign);
				}

				if (_formContext.Frequency > 0)
					_frequencyField.Text = _formContext.Frequency.ToString ();

				var flightDirAdapter = new ArrayAdapter<string> (this, 
					                       Resource.Layout.SimpleListRow, 
					                       emploeeList.ToArray ());
				_cmbFlightDirector.Adapter = flightDirAdapter;
			

				if (_formContext.FlightDirector != null)
				{
					var pos = emploees.FindIndex (e => e.CallSing == _formContext.FlightDirector.CallSing);
					_cmbFlightDirector.SetSelection (pos + 1);
				}

				//var flightScheduleItem = WorkingContext.Data as FlightScheduleItem;
				if (_formContext.FlightScheduleItem != null)
				{
					_flightField.Text = string.Format ("Парадром: {0}     Поле: {1}",
						_formContext.FlightScheduleItem.Paradrom.Name, 
						_formContext.FlightScheduleItem.CurrentField.Name);

					_dataField.Text = string.Format ("Дата: {0}", 
						_formContext.FlightScheduleItem.FlightDate.Date.ToString ("dd.MM.yyyy"));
				}
				//_cmbFlightDirector.SetSelection (1);

				var winchTableItems = new List<WinchTeamViewTableRow> ();
				foreach (var item in _formContext.WinchCommands)
				{
					winchTableItems.Add (new WinchTeamViewTableRow () { 
						EmploeeName = item.WinchMaster.CallSing, 
						Position = EnumWinchEmploeePosition.WinchMaster, 
						WinchName = item.Winch.Name
					});

					if (item.WinchAssistant != null)
					{
						winchTableItems.Add (new WinchTeamViewTableRow () { 
							EmploeeName = item.WinchAssistant.CallSing, 
							Position = EnumWinchEmploeePosition.WinchAssistant, 
							WinchName = item.Winch.Name 
						});
					}
				}

				_winchListView.Adapter = new WinchTeamAdapter (this, winchTableItems);

				var startWorkers = new List<StartCommandViewTableRow> ();
				foreach (var startWorker in _formContext.StartCommand)
				{
					startWorkers.Add (new StartCommandViewTableRow () {
						Emploee = startWorker.Emploee,
						Position = startWorker.Position,
						Selected = false
					});
				}
				_startCommandListView.Adapter = new StartCommandAdapter (this, startWorkers, _startCommandListView);
			}
			else
			{
				_flightField.Text = "TEST";
			}
		}

/*		protected override void SetView()
		{
			
			var layoutWidth = _headerLayout.Width;
			if (layoutWidth > 0)
			{
				var blockWidth = layoutWidth / 2;
				_flightField.LayoutParameters =  new LinearLayout.LayoutParams(blockWidth, _headerLayout.Height);
				_flightField.TextSize = 24;

				_dataField.LayoutParameters = 
					new LinearLayout.LayoutParams (400, _headerLayout.Height);
				_dataField.TextSize = 20;

				_frequencyField.LayoutParameters = 
					new LinearLayout.LayoutParams(260, _headerLayout.Height) ;
				_frequencyField.TextSize = 24;
			}
		}*/

		protected override WorkingContext LoadWorkingContext ()
		{
			var service = GetKeeperDataService ();

			var currentContext = service.GetInfo (DataKeeperKeys.SELECTED_TEAM);
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
		private void OnDelWinchCrewMember()
		{
			var adapter = ((WinchTeamAdapter)_winchListView.Adapter);
			var items = adapter.GetSelected();
			items.ForEach (i =>
			{
				adapter.RemoveItem(i);
				var winchCrew = _formContext.WinchCommands
					.Where(w => w.Winch.Name == i.WinchName).FirstOrDefault();
				if (i.Position == EnumWinchEmploeePosition.WinchMaster)
					winchCrew.WinchMaster = null;
				else if (i.Position == EnumWinchEmploeePosition.WinchAssistant)
					winchCrew.WinchAssistant = null;
				else if (i.Position == EnumWinchEmploeePosition.WinchSecondAssistant)
					winchCrew.SecondWinchAssistant = null;

				if (winchCrew.WinchMaster == null 
					&& winchCrew.WinchAssistant == null 
					&& winchCrew.SecondWinchAssistant == null)
					_formContext.WinchCommands.Remove(winchCrew);
			});

			return;
		}

		private void OnDelStartCrewMember ()
		{
			var adapter = ((StartCommandAdapter)_startCommandListView.Adapter);
			var items = adapter.GetSelected ();
			items.ForEach (i =>
			{
				adapter.RemoveItem(i);
				_formContext.StartCommand
					.RemoveAll(s => s.Emploee == i.Emploee && s.Position == i.Position);
			});
			return;
		}

		public static void OnFreqFocusChanged (object sender, View.FocusChangeEventArgs args)
		{
			var frequencyField = sender as EditText;
			if (args.HasFocus)
				frequencyField.InputType = Android.Text.InputTypes.Null;
		}

		private void OnStartDay()
		{
			_dataAccessor.SaveWorkingDay (_formContext);
			var openContext = new WorkingContext(){ServiceConnection = null, Data = _formContext};
			SaveDataToStorage (DataKeeperKeys.CURRENT_WORK_DAY, openContext);
			var intent = new Intent(this, typeof(WorkListActivity));
			intent.PutExtra (CREATOR_BUNDLE_KEY, DataKeeperKeys.CURRENT_WORK_DAY);
			StartActivity(intent);

		}

		#endregion
	}
}

