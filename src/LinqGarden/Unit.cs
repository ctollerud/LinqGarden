using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
	public class Unit
	{
		private Unit() { }
		public static Unit Instance { get; } = new Unit();
	}
}
