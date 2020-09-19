using System;
using System.Diagnostics.Contracts;
using LinqGarden.Functions;

namespace LinqGarden {
    /// <summary>
    /// Represents a single-input function that returns a fallible.
    /// Provides a composition-friendly interface.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TFailure"></typeparam>
    /// <typeparam name="TSuccess"></typeparam>
    public class FallibleFunction<TInput, TFailure, TSuccess>
    {
        private readonly Func<TInput, Fallible<TFailure, TSuccess>> _func;

        internal FallibleFunction(Func<TInput, Fallible<TFailure, TSuccess>> func) {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <summary>
        /// Retrieves the underlying delegate.
        /// </summary>
        /// <returns></returns>
        public Func<TInput, Fallible<TFailure, TSuccess>> AsFunc() => _func;

        /// <summary>
        /// Invokes the function, materializing the Fallible
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Fallible<TFailure, TSuccess> Invoke(TInput input) => _func(input);

        /// <summary>
        /// Wraps the function in exception-handling for the specified exception type.
        /// 
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="convertToFailure"></param>
        /// <returns></returns>
        [Pure]
        public FallibleFunction<TInput, TFailure, TSuccess> Catch<TException>(Func<TException, TFailure> convertToFailure)
            where TException : Exception =>
                new FallibleFunction<TInput, TFailure, TSuccess>(x =>
                {
                    try
                    {
                        return _func(x);
                    }
                    catch (TException exception)
                    {
                        return Fallible.Failure<TFailure, TSuccess>(convertToFailure(exception));
                    }
                });
    }

    public static class FallibleFunction
    {
        public static FallibleFunction<TInput, TFailureOutput, TSuccess> SelectFailure<TInput, TFailureInput, TFailureOutput, TSuccess>(
            this FallibleFunction<TInput, TFailureInput, TSuccess> input,
            Func<TFailureInput, TFailureOutput> failureTransformation) =>
                new FallibleFunction<TInput, TFailureOutput, TSuccess>(
                    input.AsFunc().Then(x => x.SelectFailure(failureTransformation)));

        public static Fallible<TFailure, TSuccess> Invoke<TFailure, TSuccess>(this FallibleFunction<Unit, TFailure, TSuccess> input) =>
            input.Invoke(Unit.Instance);

        [Pure]
        public static FallibleFunctionBuilder<Unit, Unit> Build(Action action) =>
            new FallibleFunctionBuilder<Unit, Unit>(_ =>
            {
                action();
                return Unit.Instance;
            });

        [Pure]
        public static FallibleFunctionBuilder<Unit, T> Build<T>(Func<T> function) =>
            new FallibleFunctionBuilder<Unit, T>(_ => function());
    }
}
