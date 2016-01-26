using System;
using System.Collections.Generic;

namespace VectorDataLayer
{
	public class WorkingDay
	{
		public long Id { get; set; }

		//  Ссылка на лётный день
		public FlightScheduleItem FlightScheduleItem { get; set; }

		// РП
		public Employee FlightDirector { get; set; }

		// Команды лебёдок
		public List<WinchCommand> WinchCommands { get; set; }

		// Стартовая команда
		public List<StartWorker> StartCommand { get; set; }

		// Рабочая частота
		public decimal Frequency { get; set; }

		public WorkingDay()
		{
			WinchCommands = new List<WinchCommand> ();
			StartCommand = new List<StartWorker> ();
		}
	}
}

