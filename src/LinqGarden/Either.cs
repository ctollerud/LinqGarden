using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
    public interface Either<out TLeft, out TRight>
    {
        internal TLeft RawLeftValue { get; }

        internal TRight RawRightValue { get; }

        internal bool IsRight { get; }

        TOutput To<TOutput>(Func<TLeft, TOutput> ifLeft, Func<TRight, TOutput> ifRight) =>
            IsRight ? ifRight(RawRightValue) : ifLeft(RawLeftValue);
    }

    /// <summary>
    /// Represents a value can be in two mutually exclusive states.
    /// It is either a "Left" or a "Right".
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    internal record EitherImpl<TLeft, TRight>(
        TLeft RawLeftValue,
        TRight RawRightValue,
        bool IsRight
    ) : Either<TLeft,TRight>
    {
        TLeft Either<TLeft, TRight>.RawLeftValue => RawLeftValue;

        TRight Either<TLeft, TRight>.RawRightValue => RawRightValue;

        bool Either<TLeft, TRight>.IsRight => IsRight;
    }

    public static class Either
    {
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left) =>
#nullable disable
            new EitherImpl<TLeft, TRight>(left, default, false );
#nullable enable

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right) =>
#nullable disable
                new EitherImpl<TLeft, TRight>(default, right, true);
#nullable enable

        public static Maybe<TRight> GetRight<TLeft, TRight>(this Either<TLeft, TRight> input) =>
            input.IsRight ? Maybe.Some(input.RawRightValue) : Maybe.None<TRight>();

        public static Maybe<TLeft> GetLeft<TLeft, TRight>(this Either<TLeft, TRight> input) =>
            !input.IsRight ? Maybe.Some(input.RawLeftValue) : Maybe.None<TLeft>();

    }
}
