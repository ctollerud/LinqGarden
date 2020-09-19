
using System;
using System.Collections;
using System.Collections.Generic;

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
	public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        /// <summary>
        /// stores the underlying value for the Maybe.
        /// </summary>
		private readonly T _value;

        /// <summary>
        /// true if there is a value present.
        /// </summary>
		public bool HasValue { get; }

		private Maybe( T value, bool hasValue )
		{
			_value = value;
			HasValue = hasValue;
		}

		public TTo To<TTo>( Func<TTo> ifNone, Func<T,TTo> ifSome ) =>
			HasValue ? ifSome( _value ) : ifNone();

		public TTo To<TTo>( Func<T, TTo> ifSome, Func<TTo> ifNone ) =>
			HasValue ? ifSome( _value ) : ifNone();

        /// <summary>
        /// Convert the maybe to a sequence of zero to one items
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ToEnumerable()
        {

            // checking that value != null to convince compiler that result items in sequence are not nullable
            if (HasValue && _value != null) 
            {
                yield return _value;
            }
        }

		internal static Maybe<T> Some( T input ) =>
			new Maybe<T>(
				input ?? throw new ArgumentNullException( nameof( input ) ),
				hasValue: true );

        /// <summary>
        /// Used to ease implementation of some other odds and ends.
        /// </summary>
        /// <returns></returns>
		private (bool, T) BuildComparisonTuple() => (HasValue, _value);

        /// <summary>
        /// Two maybes are considered equal if:
        /// - They are the same type
        /// AND
        /// - They both don't have a value OR they both have a value that equal one another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
		public bool Equals( Maybe<T> other ) =>
			BuildComparisonTuple().Equals( other.BuildComparisonTuple() );

        /// <summary>
        /// Two maybes are considered equal if:
        /// - They are the same type
        /// AND
        /// - They both don't have a value OR they both have a value that equal one another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool operator ==( Maybe<T> first, Maybe<T> other ) =>
			first.Equals( other );

		public static bool operator !=( Maybe<T> first, Maybe<T> other ) =>
			!first.Equals( other );

        public override bool Equals( object obj ) =>
			obj is Maybe<T> maybe && Equals( maybe );

		public override int GetHashCode() =>
			BuildComparisonTuple().GetHashCode();
    }

	public static class Maybe
	{
        /// <summary>
        /// constructs a Maybe with a value (i.e. a 'Some' )
        /// the input cannot be null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
		public static Maybe<T> Some<T>( T input ) =>
			Maybe<T>.Some( input );

        /// <summary>
        /// constructs a Maybe without a value (i.e. a 'None').
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public static Maybe<T> None<T>() => default;

#nullable disable
        /// <summary>
        /// Returns the underlying value for the Maybe, or the default value 
        /// for type if the input is a None.
        /// 
        /// In the case of classes, this default would often be null, so use with care.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T ValueOrDefault<T>( this Maybe<T> input ) =>
			input.To<T>(
				() => default( T ),
				x => x );
#nullable enable

        /// <summary>
        /// Converts the input to a Maybe.
        /// 
        /// if the input is null then the result will be unset.
        /// In all other circumstances, the Maybe will have a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Maybe<T> ToMaybe<T>( this T input )
		{
			if( input == null )
			{
				return default;
			}
			return Some( input );
		}

        /// <summary>
        /// Maps the Maybe into another type.  If the input is Some, then the transformation is applied, and 
        /// the result will be Some.
        /// 
        /// If the input is None, then the result will also be None.
        /// </summary>
        /// <typeparam name="TOld"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="input"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
		public static Maybe<TNew> Select<TOld, TNew>( this Maybe<TOld> input, Func<TOld, TNew> transform ) =>
			input.To<Maybe<TNew>>(
				() => default,
				x => Maybe.Some( transform( x ) ) );

        /// <summary>
        /// Maps the maybe into another maybe, then flattens the result.
        /// If the input is None, then the output is None.
        /// If the input is Some, but the transformation returns None, then the output is None.
        /// If the input is Some and the transformation returns Some, then the input value and transformation value
        /// get combined into a final result.
        /// 
        /// This is a standard SelectMany implementation, which enables chaining multiple "from" clauses together 
        /// using linq query syntax.
        /// </summary>
        /// <typeparam name="TStart"></typeparam>
        /// <typeparam name="TMiddle"></typeparam>
        /// <typeparam name="TFinal"></typeparam>
        /// <param name="input"></param>
        /// <param name="bindFunction"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Maybe<TFinal> SelectMany<TStart, TMiddle, TFinal>(
            this Maybe<TStart> input,
            Func<TStart, Maybe<TMiddle>> bindFunction,
            Func<TStart, TMiddle, TFinal> selector) =>
            input.To<Maybe<TFinal>>(
                () => Maybe.None<TFinal>(),
                start =>
                    bindFunction(start)
                    .To<Maybe<TFinal>>(
                        () => Maybe.None<TFinal>(),
                        middle => Some( selector( start, middle ) ) ) );

        /// <summary>
        /// Performs some filtering on the input.  
        /// If the input is None, the output will also be None.
        /// If the input is Some, and the predicate returns false, then the result is None
        /// If the input is Some and the predicate returns true, then the input is returned as is.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Maybe<T> Where<T>(this Maybe<T> input, Func<T, bool> predicate) =>
            from inputVal in input
            from filteredVal in predicate(inputVal) ? input : Maybe.None<T>()
            select filteredVal;

        /// <summary>
        /// Performs an action if the input has a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="ifSome"></param>
        public static Maybe<T> IfSomeDo<T>( this Maybe<T> input, Action<T> ifSome ) =>
			input.To<Maybe<T>>(
				() => input,
				x =>
				{
					ifSome( x );
					return input;
				} );

        /// <summary>
        /// Perform an action if the input doesn't have a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="ifNone"></param>
        public static Maybe<T> IfNoneDo<T>(this Maybe<T> input, Action ifNone) =>
            input.To<Maybe<T>>(
                () =>
                {
                    ifNone();
                    return input;
                },
                x => input);
    }

}
