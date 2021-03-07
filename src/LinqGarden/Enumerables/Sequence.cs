using System;
using System.Collections.Generic;
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
    }
}
