using System;
using System.Collections.Generic;

namespace VectorDataLayer
{
	public class FlightScheduleDay
	{
		public DateTime Date { get; set;}

		public List<FlightScheduleItem> Paradroms { get; set; }
	}
}

