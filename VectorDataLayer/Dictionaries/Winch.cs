using System;

namespace VectorDataLayer
{
	public class Winch
	{
		public long Id { get; set; }

		public string Name { get; set; }

		// Госномер
		public string StateNumber {get; set;}

		// КОличество стартов
		public int StartQuantity { get; set; }
	}
}

