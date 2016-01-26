using System;
using VectorDataLayer;

namespace StartWorkplace
{
	public class WinchTeamViewTableRow
	{
		public string EmploeeName { get; set;}

		/// <summary>Должность</summary>
		public EnumWinchEmploeePosition Position { get; set;}

		public string WinchName { get; set;}

		public bool Selected { get; set;}
	}
}

