using System;

namespace VectorDataLayer
{
	public class DataAccessorFactory 
	{
		public IDataAccessor GetDataAccessor()
		{
			return new TestDataAccessor ();
		}
	}
}

