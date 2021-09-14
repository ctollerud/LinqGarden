using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqGarden.Functions;

namespace LinqGarden
{
    public interface FallibleExceptionFunction<in TIn, out TException, out TSuccess>
        : FallibleFunction<TIn,TException,TSuccess>
        where TException : Exception
    {

        new public FallibleExceptionFunction<TIn, Exception, TSuccess> CatchAsFailure<TException2>()
            where TException2 : Exception =>
                Function.CatchAsFailure<TIn, Exception, TSuccess, TException2>(this);

    }

    public interface FallibleFunction<in TIn, out TFailure, out TSuccess>
        : Function<TIn, Fallible<TFailure,TSuccess>>
    {
        Function<TIn, TResult> To<TResult>(Func<TFailure, TResult> ifFailure, Func<TSuccess, TResult> ifSuccess) =>
            Function.To<TIn, TFailure, TSuccess, TResult>(this, ifFailure, ifSuccess);
    }

    public interface Function<in TIn, out TOut>
    {
        internal Func<TIn,TOut> RawFunc { get; }

        public FallibleExceptionFunction<TIn, TException, TOut> CatchAsFailure<TException>()
            where TException : Exception =>
                Function.CatchAsFailure<TIn, TException, TOut>(this);
    }

    internal record FunctionImpl<TIn,TOut>(
        Func<TIn,TOut> RawFunc
        ) : Function<TIn,TOut>
    {
        Func<TIn, TOut> Function<TIn, TOut>.RawFunc => RawFunc;
    }

    internal record FallibleExceptionFunctionImpl<TIn,TException,TSuccess>(
        Func<TIn,Fallible<TException,TSuccess>> RawFunc
        ) : FallibleExceptionFunction<TIn,TException,TSuccess>
        where TException : Exception
    {
        Func<TIn, Fallible<TException, TSuccess>> Function<TIn, Fallible<TException, TSuccess>>.RawFunc => RawFunc;
    }

    public static class Function
    {
        public static Function<TIn, TOut> From<TIn, TOut>(Func<TIn, TOut> func) =>
            new FunctionImpl<TIn, TOut>(func);

        public static Function<Unit, TOut> From<TOut>(Func<TOut> func) =>
            new FunctionImpl<Unit, TOut>( _ => func() );

        public static Function<Unit, Unit> FromAction(Action action) =>
            new FunctionImpl<Unit, Unit>(_ =>
            {
                action();
                return Unit.Instance;
            });

        public static TOut Invoke<TIn, TOut>(this Function<TIn, TOut> input, TIn arg) =>
            input.RawFunc.Invoke(arg);

        public static TOut Invoke<TOut>(this Function<Unit, TOut> input) =>
            input.Invoke(Unit.Instance);

        internal static FallibleExceptionFunction<TIn, TException, TSuccess> CatchAsFailure<TIn, TException, TSuccess>(
            Function<TIn, TSuccess> input)
            where TException : Exception =>
                new FallibleExceptionFunctionImpl<TIn, TException, TSuccess>(x =>
                 {
                     TSuccess success;
                     try
                     {
                         success = input.Invoke(x);
                     }
                     catch (TException exception)
                     {
                         return Fallible.Failure<TException, TSuccess>(exception);
                     }

                     return Fallible.Success<TException, TSuccess>(success);
                 });

        public static Function<TIn, TOut> AsFunction<TIn, TOut>(this Function<TIn, TOut> input) =>
            input;

        public static FallibleExceptionFunction<TIn, TException, TSuccess> AsFallibleExceptionFunction<TIn, TException, TSuccess>(this Function<TIn, Fallible<TException, TSuccess>> input)
            where TException : Exception =>
                input is FallibleExceptionFunction<TIn, TException, TSuccess> x
                ? x
                : new FallibleExceptionFunctionImpl<TIn, TException, TSuccess>(input.RawFunc);


        internal static FallibleExceptionFunction<TIn, Exception, TSuccess> CatchAsFailure<TIn, TException1, TSuccess, TException2>(
            FallibleExceptionFunction<TIn, TException1, TSuccess> input
            )
            where TException1 : Exception
            where TException2 : Exception =>
                input.AsFunction()
                .CatchAsFailure<TException2>()
                .To<Fallible<Exception, TSuccess>>(
                    failure => Fallible.Failure<Exception, TSuccess>(failure),
                    fallible => fallible)
                .AsFallibleExceptionFunction();

        public static Function<TIn, TOut2> Select<TIn, TOut1, TOut2>(this Function<TIn, TOut1> input, Func<TOut1, TOut2> selector) =>
            Function.From(input.RawFunc.Then(selector));

        internal static Function<TIn, TOut> To<TIn, TFailure, TSuccess, TOut>(
            FallibleFunction<TIn, TFailure, TSuccess> input, Func<TFailure, TOut> ifFailure, Func<TSuccess, TOut> ifSuccess) =>
                input
                .AsFunction()
                .Select(fallible => fallible.To<TOut>(ifFailure, ifSuccess));

            
    }
}
