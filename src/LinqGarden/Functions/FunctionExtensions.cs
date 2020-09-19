using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace LinqGarden.Functions
{
    /// <summary>
    /// Defines various utilities and extension methods for working with delegates.
    /// </summary>
	public static class FunctionExtensions
	{
        /// <summary>
        /// Applies the input to function.
        /// This allows for a method-chaining style of syntax.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="input"></param>
        /// <param name="func"></param>
        /// <returns></returns>
		public static TOut Pipe<TIn, TOut>( this TIn input, Func<TIn, TOut> func ) =>
			( func ?? throw new ArgumentNullException( nameof( func ) ) )
			( input );

		/// <summary>
		/// Converts the action to a Unit->Unit function so it can be used in more contexts
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		[Pure]
		public static Func<Unit,Unit> Function( Action action ) => _ =>
        {
            action();
            return Unit.Instance;
        };

        /// <summary>
        /// Converts the func to a Unit->T function so it can be used in more contexts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        [Pure]
        public static Func<Unit, T> Function<T>(Func<T> function) => _ => function();

        /// <summary>
        /// Composes the functions so that the result of the input is passed to the second function
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TMiddle"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="input"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        [Pure]
        public static Func<TInput, TOutput> Then<TInput, TMiddle, TOutput>(this Func<TInput, TMiddle> input, Func<TMiddle, TOutput> then) =>
            x => then(input(x));

        /// <summary>
        /// Provides an overload of invoke so that Unit doesn't need to be passed in manually.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T Invoke<T>(this Func<Unit, T> input) =>
            input(Unit.Instance);

        /// <summary>
        /// Performs an action with the value before returing it.
        /// 
        /// This is only useful for performing side-effects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="action"></param>
        /// <returns></returns>
		public static T Tee<T>( this T input, Action<T> action )
		{
			action( input );
			return input;
		}
	}
}
