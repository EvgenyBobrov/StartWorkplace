using System;
using System.Collections.Generic;

namespace VectorDataLayer
{
	public interface IDataAccessor
	{
		List<FlightScheduleDay> GetSchedule (DateTime date);

		bool IsDayOnParadromStarted (Paradrom paradrom, DateTime flightDate);

		List<Employee> GetEmploees ();

		List<Winch> GetWinches ();
	}
}

