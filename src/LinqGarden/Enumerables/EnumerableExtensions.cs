using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using LinqGarden.Functions;

namespace LinqGarden.Enumerables
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> StartWith<T>(this IEnumerable<T> input, T start)
		{
			yield return start;
			foreach(var item in input)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Returns the sequence as a collection.
		/// If the input is already a collection, then no work is performed other than casting to an ICollection
		/// </summary>
		public static ICollection<T> AsCollection<T>( this IEnumerable<T> input )
		{
			return input switch
			{
				null => throw new ArgumentNullException( nameof( input ) ),
				ICollection<T> collection => collection,
				IEnumerable<T> anythingElse => anythingElse.ToList()
			};
		}

		/// <summary>
		/// Performs a side-effect per element as it passes through.
		/// NOTE: on its own, this method is purely lazy and requires evaluation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input"></param>
		/// <returns></returns>
		[Pure]
		public static IEnumerable<T> Do<T>( this IEnumerable<T> input, Action<T> sideEffect )
		{
			foreach( var item in input )
			{
				sideEffect( item );
				yield return item;
			}
		}

		/// <summary>
		/// Iterate through the sequence.
		/// This would only be valuable if there are side-effects incorporated into the linq expression.
		/// </summary>
		/// <param name="input"></param>
		public static void ForEach<T>( this IEnumerable<T> input )
		{
			foreach( var _ in input )
			{
                //Do nothing, besides iteration.
			}
		}

		public static Maybe<T> FirstOrNone<T>( this IEnumerable<T> input )
		{
			Maybe<T> responseVal = default;

			foreach( var item in input.Take(1) )
			{
				//If a value is located, then use it.
				responseVal = item.ToMaybe();
			}

			return responseVal;
		}

        /// <summary>
        /// Overload, allowing using Maybes in the collections selector.
        /// Maybes can be thought of as a sequence of 0 or 1 non-null values.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TMiddle"></typeparam>
        /// <typeparam name="TFinal"></typeparam>
        /// <param name="input"></param>
        /// <param name="collectionSelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TFinal> SelectMany<TInput, TMiddle, TFinal>(
            this IEnumerable<TInput> input,
            Func<TInput, Maybe<TMiddle>> collectionSelector,
            Func<TInput, TMiddle, TFinal> resultSelector) =>
                input.SelectMany(
                    collectionSelector.Then(x => x.ToEnumerable()),
                    resultSelector);
	}
}
