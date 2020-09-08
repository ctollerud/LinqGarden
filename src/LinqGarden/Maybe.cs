using System;

namespace LinqGarden
{
	public struct Maybe<T>
	{
		private readonly T _value;

		private readonly bool _hasValue;

		private Maybe( T value, bool hasValue )
		{
			_value = value;
			_hasValue = hasValue;
		}

		public TTo To<TTo>( Func<TTo> ifNone, Func<T,TTo> ifSome ) =>
			_hasValue ? ifSome( _value ) : ifNone();

		public TTo To<TTo>( Func<T, TTo> ifSome, Func<TTo> ifNone ) =>
			_hasValue ? ifSome( _value ) : ifNone();

		internal static Maybe<T> Some( T input ) =>
			new Maybe<T>(
				input ?? throw new ArgumentNullException( nameof( input ) ),
				hasValue: true );

		internal static Maybe<T> None() =>
			new Maybe<T>( default, hasValue: false );

	}

	public static class Maybe
	{
		public static Maybe<T> Some<T>( T input ) =>
			Maybe<T>.Some( input );

		public static Maybe<T> None<T>() =>
			Maybe<T>.None();
	}

}
