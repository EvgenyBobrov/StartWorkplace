
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VectorDataLayer;

namespace KeeperData
{
	[Activity (Label = "KeepingDataContextActivity")]			
	public class KeepingDataContextActivity : Activity
	{
		protected const string CREATOR_BUNDLE_KEY = "ActivityCreator";

		#region Переменные класса
		protected KeeperDataServiceConnection _serviceConnection;

		protected WorkingContext WorkingContext { get; set;}

		public bool IsBound = false;

		protected WorkContextReceiver _contextChangeReceiver;

		protected string ActivityKey { get; set;}

		protected IDataAccessor _dataAccessor;

		/// <summary> Причина создания формы </summary>
		protected string CreatorKey { get; set;}

		#endregion


		#region Перегруженные методы Activity
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			CreatorKey = Intent.GetStringExtra(CREATOR_BUNDLE_KEY);

			// restore from connection there was a configuration change, such as a device rotation
			var workingContext = LastNonConfigurationInstance as WorkingContext;
			// Если удалось поднять контекст из конфигурации (т.е. видимо изменили ориентацию)
			if (workingContext != null)
			{
				_serviceConnection = workingContext.ServiceConnection;
				WorkingContext = workingContext;
			}
			//Если форма создается с нуля, либо идёт возвращение к ранее открытой
			/*else
			{
				WorkingContext = LoadWorkingContext();
			}*/

			_dataAccessor = new DataAccessorFactory ().GetDataAccessor ();
			var errorMessage = _dataAccessor.GetErrorMessage ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			// Обновление элементов конечной формы.
			// нужно перегрузить в потомке
			InitForm ();

			//Log.Debug ("MainActivity", "OnStart" );

			if (_serviceConnection == null)
			{
				_serviceConnection = new KeeperDataServiceConnection (this);
			}
			var demoServiceIntent = new Intent (this, typeof(KeeperDataService));
			BindService (demoServiceIntent, _serviceConnection, Bind.AutoCreate);
			

			if (_contextChangeReceiver == null)
			{
				_contextChangeReceiver = new WorkContextReceiver ();
				var intentFilter = new IntentFilter (KeeperDataService.DataChangedIntent);
				RegisterReceiver (_contextChangeReceiver, intentFilter);
			}
			/*			if (workingContext != null)
			{
				_serviceConnection.KeeperServiceBinder.GetService()
					.SaveInfo (workingContext.Info);
				OnGet ();
			}*/
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

/*			//Log.Debug ("MainActivity", "OnDestroy" );
			if (IsBound)
			{
				UnbindService (_serviceConnection);
				IsBound = false;
			}*/
		}

		// return the service connection if there is a configuration change
		public override Java.Lang.Object OnRetainNonConfigurationInstance ()
		{
			base.OnRetainNonConfigurationInstance ();

			//Log.Debug ("MainActivity", "OnRetainNonConfigurationInstance" );

			//string info = IsBound ? _serviceConnection.KeeperServiceBinder.GetService ().GetInfo () : string.Empty;

			return new WorkingContext (){ServiceConnection = _serviceConnection, 
				Data = WorkingContext.Data ?? null};
		}	

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);
			if (hasFocus)
				SetView();
		}
		#endregion

		#region Методы для перегрузки в потомках
		/// <summary>
		/// Вызывается при получении WorkContextReceiver'ом 
		/// сообщения от сервиса об изменении данных
		/// </summary>
		public virtual void OnChangeWorkingContext()
		{
		}

		/// <summary>
		/// Вызывается, когда KeeperDataServiceConnection.OnServiceConnected()
		/// успешно выполняется и можно пользоваться сервисом данных
		/// </summary>
		public virtual void OnServiceConnected()
		{
			if (WorkingContext == null)
				WorkingContext = LoadWorkingContext();
		}

		/// <summary> Вызывается потомками для установки размеров элементов формы </summary>
		protected virtual void SetView()
		{
		}

		protected virtual WorkingContext LoadWorkingContext()
		{
			return new WorkingContext ();
		}

		/// <summary> Вызывается в OnCreate(). 
		/// Используется для восстановления
		/// после смены ориентации экрана </summary>
		protected virtual void InitForm()
		{
		}

		#endregion

		#region Protected-методы
		/// <summary> Вызывается потомками для сохранения данных в сервис хранения </summary>
		protected bool SaveDataToStorage(string key, WorkingContext info)
		{
			if (_serviceConnection != null && _serviceConnection.KeeperServiceBinder != null)
			{
				_serviceConnection.KeeperServiceBinder.GetService ().SaveInfo (key, info);
				return true;
			}
			return false;
		}

		/// <summary> Вызывается потомками для удаления данных из сервиса хранения </summary>
		protected void DeleteDataFromStorage(string key)
		{
			if (_serviceConnection != null && _serviceConnection.KeeperServiceBinder != null)
			{
				_serviceConnection.KeeperServiceBinder.GetService ().DeleteInfo (key);
			}
		}

		protected KeeperDataService GetKeeperDataService()
		{
			if (_serviceConnection != null && _serviceConnection.KeeperServiceBinder != null)
				return _serviceConnection.KeeperServiceBinder.GetService ();

			return null;
		}

		protected T GetServiceData<T>(string key, bool deleteData)
		{
			var service = GetKeeperDataService ();
			T serviceData = default(T);

			var dataContext = service.GetInfo (key);
			if (dataContext != null)
			{
				try
				{
					serviceData = (T)dataContext.Data;
				}
				catch
				{
					
				}
			}

			if(deleteData)
				DeleteDataFromStorage (key);
			
			return serviceData;
		}

		protected FragmentTransaction GetFragmentTransaction (string tag)
		{
			FragmentTransaction ft = FragmentManager.BeginTransaction();
			//Remove fragment else it will crash as it is already added to backstack
			Fragment prev = FragmentManager.FindFragmentByTag(tag);
			if (prev != null) {
				ft.Remove(prev);
			}

			ft.AddToBackStack(null);
			//ft.Commit ();
			return ft;
		}
		#endregion
	}
}

