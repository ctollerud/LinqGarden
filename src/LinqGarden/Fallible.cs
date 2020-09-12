using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
	public class Fallible<TFailure,TSuccess>
	{
		private readonly Either<TFailure, TSuccess> _Data;

		internal Fallible( Either<TFailure,TSuccess> data )
		{
			_Data = data ?? throw new ArgumentNullException( nameof( data ) );
		}

		public Maybe<TFailure> FailureValue => _Data.LeftValue;

		public Maybe<TSuccess> SuccessValue => _Data.RightValue;

		public TTo To<TTo>( Func<TFailure, TTo> ifFailure, Func<TSuccess, TTo> ifSuccess ) =>
			_Data.To<TTo>( ifFailure, ifSuccess );
	}

	public static class Fallible
	{
		public static Fallible<TFailure, TSuccess> Failure<TFailure, TSuccess>( TFailure failureValue ) =>
			new Fallible<TFailure, TSuccess>( Either.Left<TFailure, TSuccess>( failureValue ) );

		public static Fallible<TFailure, TSuccess> Success<TFailure, TSuccess>( TSuccess successValue ) =>
			new Fallible<TFailure, TSuccess>( Either.Right<TFailure, TSuccess>( successValue ) );

		public static Fallible<TFailure, TSuccess> IfNoneFail<TFailure, TSuccess>(
			this Maybe<TSuccess> input,
			TFailure failureValue ) =>
				input.To<Fallible<TFailure, TSuccess>>(
					() => Fallible.Failure<TFailure,TSuccess>( failureValue ),
					x => Fallible.Success<TFailure,TSuccess>( x ) );

		public static Fallible<TError, TTo> Select<TError, TFrom, TTo>(
			this Fallible<TError, TFrom> input,
			Func<TFrom, TTo> mappingFunction ) =>
				input.To<Fallible<TError, TTo>>(
					error => Fallible.Failure<TError, TTo>( error ),
					success => Fallible.Success<TError, TTo>( mappingFunction( success ) ) );

		public static Fallible<TError, TSuccessFinal> SelectMany<TError, TSuccessStart, TSuccessMiddle, TSuccessFinal>(
			this Fallible<TError, TSuccessStart> input,
			Func<TSuccessStart, Fallible<TError, TSuccessMiddle>> bindFunction,
			Func<TSuccessStart, TSuccessMiddle, TSuccessFinal> resultsSelector ) =>
				input.To<Fallible<TError, TSuccessFinal>>(
					error1 => Fallible.Failure<TError, TSuccessFinal>( error1 ),
					successStart =>
						bindFunction( successStart )
						.To<Fallible<TError, TSuccessFinal>>(
							error2 => Fallible.Failure<TError, TSuccessFinal>( error2 ),
							successMiddle => Fallible.Success<TError, TSuccessFinal>( resultsSelector( successStart, successMiddle ) ) ) );

	}
}
