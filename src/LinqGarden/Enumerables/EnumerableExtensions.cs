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

        /// <summary>
        /// Repeat the provided collection an infinite number of times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>( this ICollection<T> input )
        {
            if(input.Count == 0)
            {
                yield break;
            }

            while ( true )
            {
                foreach( var item in input )
                {
                    yield return item;
                }
            }
        }
		public static IEnumerable<T> StartWith<T>(this IEnumerable<T> input, T start)
		{
			yield return start;
			foreach(var item in input)
			{
				yield return item;
			}
		}

        public static IEnumerable<T> EndWith<T>(this IEnumerable<T> input, T last)
        {
            foreach (var item in input)
            {
                yield return item;
            }
            yield return last;
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


        /// <summary>
        /// Accumulate a value and emit it each time it changes
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="input"></param>
        /// <param name="seed"></param>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        public static IEnumerable<TAccumulate> AggregateSequence<TSource, TAccumulate>(
            this IEnumerable<TSource> input,
            TAccumulate seed,
            Func<TAccumulate,TSource,TAccumulate> aggregator )
        {
            var accumulator = seed;
            foreach( var value in input )
            {
                accumulator = aggregator(accumulator,value);
                yield return accumulator;
            }
        }

        /// <summary>
        /// Iterate over the collection in a pairwise fashion
        /// There must be at least 2 items in the sequence for iteration to be performed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=""></param>
        /// <returns></returns>
        public static IEnumerable<(T,T)> Pairwise<T>( this IEnumerable<T> input )
        {
            //suppress the nullability warning.  This will never be iterated over from the caller's perspective.
            //we just need some default values to prime the pump of the aggregator.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            (T, T) seed = (default(T), default(T));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

            return
                input.AggregateSequence(seed, (acc, next) => (acc.Item2, next))

                //first value is garbage.
                .Skip(1);

        }

    }
}
