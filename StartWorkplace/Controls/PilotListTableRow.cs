using System;
using VectorDataLayer;

namespace StartWorkplace
{
	public class PilotListTableRow
	{
		public int StartNumber { get; set;}

		public PilotQualificationEnum Qualification { get; set;}

		public string PilotSign { get; set;}

		public string GliderName {get; set;}

		public bool Selected { get; set;}
	}
}

