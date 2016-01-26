using System;
using System.Collections.Generic;
using System.Linq;

namespace VectorDataLayer
{
	public class TestDataAccessor : IDataAccessor
	{
		private List<FlightField> _flightFields;
		private List<Paradrom> _paradroms;

		private List<WorkingDay> _workingDays { get; set; } 

		private List<Employee> _emploees;

		private List<Winch> _winches;

		public TestDataAccessor()
		{
			_flightFields = GetTotalFlightFieldList ();
			_paradroms = CreateParadromList ();
			_workingDays = new List<WorkingDay> ();
			CreateEmploeeList ();
			CreateWinchList ();
		}

		public bool IsDayOnParadromStarted(Paradrom paradrom, DateTime flightDate)
		{
			return _workingDays.Exists(w => w.FlightScheduleItem.Paradrom == paradrom 
				&& w.FlightScheduleItem.FlightDate == flightDate.Date);
		}

		public List<FlightScheduleDay> GetSchedule (DateTime date)
		{
			var paradroms = GetFlightScheduleItemList ();
			var schedule = new List<FlightScheduleDay> ()
				{
					new FlightScheduleDay() {Date = DateTime.Today, Paradroms = paradroms}
				};

			return schedule;
		}

		public List<Employee> GetEmploees()
		{
			return _emploees;
		}

		public List<Winch> GetWinches()
		{
			return _winches;
		}




		private List<FlightScheduleItem> GetFlightScheduleItemList()
		{
			if (_paradroms == null || _paradroms.Count == 0)
				return null;
			
			var novoeScheduleItem = new FlightScheduleItem () 
				{ 
					Paradrom = _paradroms.Where(p => p.EnumParadrom == EnumParadrom.Novoe).FirstOrDefault(), 
					CurrentField = _flightFields.Where (f => f.Id == 1).First (),
					FlightDate = DateTime.Today
				};


			var konchinkaScheduleItem = new FlightScheduleItem () 
				{ 
					Paradrom = _paradroms.Where(p => p.EnumParadrom == EnumParadrom.Konchinka).FirstOrDefault(), 
					CurrentField = _flightFields.Where (f => f.Id == 3).First (),
					FlightDate = DateTime.Today
				};

			return new List<FlightScheduleItem> (){ novoeScheduleItem, konchinkaScheduleItem };
		}

		private List<FlightField> GetTotalFlightFieldList()
		{
			var foxHoleField = new FlightField () 
				{
					Id = 1, 
					Name = "Лисья нора", 
					EnumParadrom = EnumParadrom.Novoe,
					Location = "Въехать в село Теряево и повернуть направо у разрушенной церкви. Дальше прямо."
				};

			var pokrovskoeField = new FlightField () 
			{
				Id = 2, 
				Name = "Покровское", 
				EnumParadrom = EnumParadrom.Novoe,
				Location = "Проехать деревню Новое, далее, на Е-образном перекрёске налево."
			};

			var konchinkaMainField = new FlightField () 
			{
				Id = 3, 
				Name = "Кончинка-главное", 
				EnumParadrom = EnumParadrom.Konchinka,
				Location = "135 км. шоссе М4"
			};

			return new List<FlightField> (){ foxHoleField, pokrovskoeField, konchinkaMainField};
		}

		private List<Paradrom> CreateParadromList()
		{
			var flightFields = _flightFields == null ? GetTotalFlightFieldList () : _flightFields;


			var novoe = new Paradrom () {
				Id = 10, 
				Name = "Новое", 
				EnumParadrom = EnumParadrom.Novoe, 
				Fileds = flightFields.Where (f => f.EnumParadrom == EnumParadrom.Novoe).ToList ()
			};

			var konchinka = new Paradrom () {
				Id = 11, 
				Name = "Кончинка", 
				EnumParadrom = EnumParadrom.Konchinka, 
				Fileds = flightFields.Where (f => f.EnumParadrom == EnumParadrom.Konchinka).ToList ()
			};

			return  new List<Paradrom> (){ novoe, konchinka};
		}


		private void CreateEmploeeList()
		{
			_emploees = new List<Employee> ();

			_emploees.Add (new Employee(){Id = 30, CallSing = "Бандана"});
			_emploees.Add (new Employee(){Id = 31, CallSing = "Веня"});
			_emploees.Add (new Employee(){Id = 32, CallSing = "Борисыч"});
			_emploees.Add (new Employee(){Id = 33, CallSing = "Боря"});
		}

		private void CreateWinchList()
		{
			_winches = new List<Winch> ();

			_winches.Add (new Winch (){Id = 40, Name = "Унимог", StartQuantity = 1, StateNumber = "хо123о197" });
			_winches.Add (new Winch (){Id = 41, Name = "Тойота", StartQuantity = 2, StateNumber = "вс456а99" });
		}
	}
}

