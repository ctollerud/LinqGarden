using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
    /// <summary>
    /// Represents a value can be in two mutually exclusive states.
    /// It is either a "Left" or a "Right".
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public sealed class Either<TLeft, TRight>
    {
        /// <summary>
        /// Returns Some if this is a left, otherwise nothing.
        /// </summary>
        public Maybe<TLeft> LeftValue { get; }

        /// <summary>
        /// Returns Some if this is a right, otherwise nothing.
        /// </summary>
        public Maybe<TRight> RightValue { get; }

        /// <summary>
        /// Only should be available internally, since it permits invalid states.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        internal Either(Maybe<TLeft> left, Maybe<TRight> right)
        {
            LeftValue = left;
            RightValue = right;
        }

        /// <summary>
        /// Convert to some other type.  functions need to be provided to handle if it's a left or a right.
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="ifLeft"></param>
        /// <param name="ifRight"></param>
        /// <returns></returns>
        public TOutput To<TOutput>(Func<TLeft, TOutput> ifLeft, Func<TRight, TOutput> ifRight) =>
            LeftValue.To<TOutput>(
                left => ifLeft(left),
                () => ifRight(RightValue.ValueOrDefault()));
    }

    public static class Either
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left) =>
            new Either<TLeft, TRight>(Maybe.Some(left), default);

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right) =>
                new Either<TLeft, TRight>(default, Maybe.Some(right));
    }
}
