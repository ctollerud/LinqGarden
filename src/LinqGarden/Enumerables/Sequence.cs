using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqGarden.Enumerables
{

    /// <summary>
    /// A collection of functions for generating IEnumerables
    /// </summary>
    public static class Sequence
    {
        /// <summary>
        /// Create an infinite sequence by applying the function to the value repeatedly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstValue"></param>
        /// <param name="propagate"></param>
        /// <returns></returns>
        public static IEnumerable<T> Unfold<T> ( T firstValue, Func<T,T> propagate )
        {
            var nextValue = firstValue;

            while(true)
            {
                yield return nextValue;
                nextValue = propagate( nextValue );
            }
        }

        /// <summary>
        /// Repeat a given value infinitely
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>( T value )
        {
            while(true)
            {
                yield return value;
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> input, IEnumerable<T2> next) =>
            input.Zip(next, (x, y) => (x, y));

        public static IEnumerable<T4> Zip<T1, T2, T3, T4>(
            this IEnumerable<T1> input,
            IEnumerable<T2> next,
            IEnumerable<T3> next2,
            Func<T1, T2, T3, T4> combiner) =>
            input.Zip(next).Zip(next2, (tuple, last) => combiner(tuple.Item1, tuple.Item2, last));

        public static IEnumerable<(T1, T2, T3)> Zip<T1, T2, T3>(
            this IEnumerable<T1> input,
            IEnumerable<T2> next,
            IEnumerable<T3> next2) =>
                input.Zip<T1, T2, T3, (T1, T2, T3)>(next, next2, (x, y, z) => (x, y, z));

        public static IEnumerable<T5> Zip<T1, T2, T3, T4, T5>(
            this IEnumerable<T1> input,
            IEnumerable<T2> next,
            IEnumerable<T3> next2,
            IEnumerable<T4> next3,
            Func<T1, T2, T3, T4, T5> combiner) =>
            input.Zip(next, next2).Zip(next3, (tuple, last) => combiner(tuple.Item1, tuple.Item2, tuple.Item3, last));

        public static IEnumerable<(T1, T2, T3, T4)> Zip<T1, T2, T3, T4>(
            this IEnumerable<T1> input,
            IEnumerable<T2> next,
            IEnumerable<T3> next2,
            IEnumerable<T4> next3) =>
            input.Zip(next, next2, next3, (a, b, c, d) => (a, b, c, d));

    }
}
