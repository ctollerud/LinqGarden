using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using LinqGarden.Functions;

namespace LinqGarden {

    public interface Fallible<out TFailure, out TSuccess>
    {
        internal Either<TFailure,TSuccess> Data { get; }
        public TTo To<TTo>(Func<TFailure, TTo> ifFailure, Func<TSuccess, TTo> ifSuccess) =>
            Data.To<TTo>(ifFailure, ifSuccess);
    }

    public record FallibleImpl<TFailure, TSuccess>(
        Either<TFailure,TSuccess> Data
        ) : Fallible<TFailure,TSuccess>
    {
        Either<TFailure, TSuccess> Fallible<TFailure, TSuccess>.Data => Data;
    }

    public static class Fallible
    {
        /// <summary>
        /// Construct a failure value
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="failureValue"></param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccess> Failure<TFailure, TSuccess>(TFailure failureValue) =>
            new FallibleImpl<TFailure, TSuccess>(Either.Left<TFailure, TSuccess>(failureValue));

        /// <summary>
        /// Construct a success value
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="successValue"></param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccess> Success<TFailure, TSuccess>(TSuccess successValue) =>
            new FallibleImpl<TFailure, TSuccess>(Either.Right<TFailure, TSuccess>(successValue));

        public static Maybe<TFailure> GetFailure<TFailure, TSuccess>(this Fallible<TFailure, TSuccess> input) =>
            input.Data.GetLeft();

        public static Maybe<TSuccess> GetSuccess<TFailure, TSuccess>(this Fallible<TFailure, TSuccess> input) =>
            input.Data.GetRight();

        /// <summary>
        /// If the Maybe has a value, then the result will be a success.
        /// Otherwise, the provided failure value will be used .
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="input"></param>
        /// <param name="failureValue"></param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccess> IfNoneFail<TFailure, TSuccess>(
            this Maybe<TSuccess> input,
            TFailure failureValue) =>
                input.To<Fallible<TFailure, TSuccess>>(
                    () => Fallible.Failure<TFailure, TSuccess>(failureValue),
                    x => Fallible.Success<TFailure, TSuccess>(x));

        /// <summary>
        /// If this input is a Success, performs a transformation to the success value.
        /// if the input is a Failure then the result is a failure of the same type.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="input"></param>
        /// <param name="mappingFunction"></param>
        /// <returns></returns>
        public static Fallible<TFailure, TTo> Select<TFailure, TFrom, TTo>(
            this Fallible<TFailure, TFrom> input,
            Func<TFrom, TTo> mappingFunction) =>
                input.To<Fallible<TFailure, TTo>>(
                    error => Fallible.Failure<TFailure, TTo>(error),
                    success => Fallible.Success<TFailure, TTo>(mappingFunction(success)));

        /// <summary>
        /// Performs a transformation to input fallible.  If the input is a failure, or the 
        /// transformation fails, then the result is a failure.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccessStart"></typeparam>
        /// <typeparam name="TSuccessMiddle"></typeparam>
        /// <typeparam name="TSuccessFinal"></typeparam>
        /// <param name="input"></param>
        /// <param name="bindFunction">function to apply to the input if it's a success.</param>
        /// <param name="resultsSelector">function used to combine the input's success value with the transformation's success value if everything succeeded.</param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccessFinal> SelectMany<TFailure, TSuccessStart, TSuccessMiddle, TSuccessFinal>(
            this Fallible<TFailure, TSuccessStart> input,
            Func<TSuccessStart, Fallible<TFailure, TSuccessMiddle>> bindFunction,
            Func<TSuccessStart, TSuccessMiddle, TSuccessFinal> resultsSelector) =>
                input.To<Fallible<TFailure, TSuccessFinal>>(
                    error1 => Fallible.Failure<TFailure, TSuccessFinal>(error1),
                    successStart =>
                        bindFunction(successStart)
                        .To<Fallible<TFailure, TSuccessFinal>>(
                            error2 => Fallible.Failure<TFailure, TSuccessFinal>(error2),
                            successMiddle => Fallible.Success<TFailure, TSuccessFinal>(resultsSelector(successStart, successMiddle))));

        /// <summary>
        /// Performs a transformation to input fallible.  If the input is a failure, or the 
        /// transformation fails, then the result is a failure.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccessStart"></typeparam>
        /// <typeparam name="TSuccessMiddle"></typeparam>
        /// <typeparam name="TSuccessFinal"></typeparam>
        /// <param name="input"></param>
        /// <param name="bindFunction">function to apply to the input if it's a success.</param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccessOutput> SelectMany<TFailure, TSuccessInput, TSuccessOutput>(
            this Fallible<TFailure, TSuccessInput> input,
            Func<TSuccessInput, Fallible<TFailure, TSuccessOutput>> bindFunction) =>
                input.SelectMany(bindFunction, (_, output) => output);

        /// <summary>
        /// Maps the failure value into something else if it's a failure.
        /// If the input is a success, the result will be a success of the same value.
        /// </summary>
        /// <typeparam name="TFailureInput"></typeparam>
        /// <typeparam name="TFailureOutput"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="input"></param>
        /// <param name="transformation"></param>
        /// <returns></returns>
        public static Fallible<TFailureOutput, TSuccess> SelectFailure<TFailureInput, TFailureOutput, TSuccess>(
            this Fallible<TFailureInput, TSuccess> input,
            Func<TFailureInput, TFailureOutput> transformation) =>
                input.To<Fallible<TFailureOutput, TSuccess>>(
                    failure => Fallible.Failure<TFailureOutput, TSuccess>(transformation(failure)),
                    success => Fallible.Success<TFailureOutput, TSuccess>(success));

        /// <summary>
        /// Performs an action if the input is a failure.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="input"></param>
        /// <param name="ifFailure"></param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccess> IfFailureDo<TFailure, TSuccess>(
            this Fallible<TFailure, TSuccess> input,
            Action<TFailure> ifFailure)
        {
            input.GetFailure().IfSomeDo(ifFailure);
            return input;
        }

        /// <summary>
        /// Performs an action if the input is a Success.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="input"></param>
        /// <param name="ifSuccess"></param>
        /// <returns></returns>
        public static Fallible<TFailure, TSuccess> IfSuccessDo<TFailure, TSuccess>(
            this Fallible<TFailure, TSuccess> input,
            Action<TSuccess> ifSuccess)
        {
            input.GetSuccess().IfSomeDo(ifSuccess);
            return input;
        }

        /// <summary>
        /// Builds a fallible that fails with the provided value.
        /// 
        /// This is intended to be used as validation guards when using LINQ query syntax
        /// over the Fallible type.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <param name="validation"></param>
        /// <param name="valueIfFailed"></param>
        /// <returns></returns>
        public static Fallible<TFailure, Unit> Validate<TFailure>(bool validation, TFailure valueIfFailed) =>
            validation
            ? Fallible.Success<TFailure, Unit>(Unit.Instance)
            : Fallible.Failure<TFailure, Unit>(valueIfFailed);

        /// <summary>
        /// Fails with the provided input if it has a value.
        /// </summary>
        /// <typeparam name="TFailure"></typeparam>
        /// <param name="maybeFailure"></param>
        /// <returns></returns>
        public static Fallible<TFailure, Unit> IfSomeFail<TFailure>(this Maybe<TFailure> maybeFailure) =>
            maybeFailure.To<Fallible<TFailure, Unit>>(
                failure => Fallible.Failure<TFailure, Unit>(failure),
                () => Fallible.Success<TFailure, Unit>(Unit.Instance));

        public static TSuccess IfFailureThrow<TFailure, TSuccess>(
            this Fallible<TFailure, TSuccess> input, Func<TFailure, Exception> ifFailure) =>
                input.To<TSuccess>(
                    f => throw ifFailure(f),
                    s => s);
    }
}
