using System;

namespace LinqGarden
{
	/// <summary>
	/// Represents a potentially-empty value.
	/// Serves as an alternative to null for reference types, as well as 
	/// an alternative to nullable for structs.
	/// 
	/// An item is either a "none" (i.e. lacking a value),
	/// or a "some" (i.e. having a value. )
	/// </summary>
	/// <typeparam name="T"></typeparam>
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

		public static T ValueOrDefault<T>( this Maybe<T> input ) =>
			input.To<T>(
				() => default( T ),
				x => x );
	}

}
