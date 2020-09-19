using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
    /// <summary>
    /// A placeholder type, generally used to indicate that a type argument 
    /// isn't used.
    /// </summary>
	public sealed class Unit
	{
		private Unit() { }

        /// <summary>
        /// Gets the single instance of the Unit type.
        /// </summary>
		public static Unit Instance { get; } = new Unit();
	}
}
