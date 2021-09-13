using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqGarden
{
    public interface FallibleExceptionFunction<in TIn, out TException, out TSuccess>
        : FallibleFunction<TIn,TException,TSuccess>
        where TException : Exception
    {

        //TODO: experiment with implementing this sucka
        new FallibleExceptionFunction<TIn, TException, TSuccess> CatchAsFailure<TNewException>() => this; 
    }

    public interface FallibleFunction<in TIn, out TFailure, out TSuccess>
        : Function<TIn, Fallible<TFailure,TSuccess>>
    {
        
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
        public static Function<TIn, TOut> From<TIn, TOut>(Func<TIn, TOut> input) =>
            new FunctionImpl<TIn, TOut>(input);

        public static Function<Unit, TOut> From<TOut>(Func<TOut> input) =>
            new FunctionImpl<Unit, TOut>( _ => input() );

        public static TOut Invoke<TIn, TOut>(this Function<TIn, TOut> input, TIn arg) =>
            input.RawFunc.Invoke(arg);

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
                
                
    }
}
