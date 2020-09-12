using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
	public sealed class Either<TLeft, TRight>
	{
		public Maybe<TLeft> LeftValue { get; }
		public Maybe<TRight> RightValue { get; }

		private Either( Maybe<TLeft> left, Maybe<TRight> right )
		{
			LeftValue = left;
			RightValue = right;
		}

		public static Either<TLeft, TRight> Left( TLeft left ) =>
			new Either<TLeft, TRight>( Maybe.Some( left ), Maybe.None<TRight>() );

		public static Either<TLeft, TRight> Right( TRight right ) =>
			new Either<TLeft, TRight>( Maybe.None<TLeft>(), Maybe.Some( right ) );


		public TOutput To<TOutput>( Func<TLeft, TOutput> ifLeft, Func<TRight, TOutput> ifRight ) =>
			LeftValue.To<TOutput>(
				left => ifLeft( left ),
				() => ifRight( RightValue.ValueOrDefault() ) );
	}

	public static class Either
	{
		public static Either<TLeft, TRight> Left<TLeft, TRight>( TLeft left ) =>
			Either<TLeft, TRight>.Left( left );

		public static Either<TLeft, TRight> Right<TLeft, TRight>( TRight right ) =>
				Either<TLeft, TRight>.Right( right );
	}
}
