using System;
using System.Collections.Generic;

namespace VectorDataLayer
{
	// Парадром
	public class Paradrom
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public EnumParadrom EnumParadrom { get; set; }

		public string Description { get; set; }

		public List<FlightField> Fileds { get; set; }
	}
}

