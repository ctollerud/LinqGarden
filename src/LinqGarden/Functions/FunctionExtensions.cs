using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden.Functions
{
	public static class FunctionExtensions
	{
		public static TOut Pipe<TIn, TOut>( this TIn input, Func<TIn, TOut> func ) =>
			( func ?? throw new ArgumentNullException( nameof( func ) ) )
			( input );

	}
}
