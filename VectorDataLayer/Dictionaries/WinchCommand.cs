using System;

namespace VectorDataLayer
{
	public class WinchCommand
	{
		public long Id { get; set; }

		public Winch Winch { get; set; }

		public Employee WinchMaster { get; set; }

		public Employee WinchAssistant { get; set; }

		public Employee SecondWinchAssistant { get; set; }
	}
}

