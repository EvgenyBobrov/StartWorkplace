using System;

namespace VectorDataLayer
{
	// Лётное поле
	public class FlightField
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Location { get; set; }

		public EnumParadrom EnumParadrom { get; set; }
	}
}

