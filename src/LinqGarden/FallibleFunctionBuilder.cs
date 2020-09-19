using System;
using System.Diagnostics.Contracts;

namespace LinqGarden {

    /// <summary>
    /// Used as a building-block for building a FallibleFunction.
    /// At this phase of construction, the failure type of the FallibleFunction is not 
    /// yet known.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TSuccess"></typeparam>
    public class FallibleFunctionBuilder<TInput, TSuccess>
    {
        /// <summary>
        /// The function that will be adorned with some fallibility
        /// </summary>
        private readonly Func<TInput, TSuccess> _rawFunction;

        /// <summary>
        /// Constructor.  Only intended to be used via extension methods.
        /// </summary>
        /// <param name="rawFunction"></param>
        internal FallibleFunctionBuilder(Func<TInput, TSuccess> rawFunction)
        {
            _rawFunction = rawFunction ?? throw new ArgumentNullException(nameof(rawFunction));
        }

        /// <summary>
        /// Constructs a FallibleFunction that captures an exception of a specific type as 
        /// a failure value if encountered.
        /// 
        /// An exception is probably not what's desired as the ultimate failure type, so it's recommended
        /// to mold the failure into a more desireable type afterwards ( with, for instance, .SelectFailure() ).
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
        [Pure]
        public FallibleFunction<TInput, TException, TSuccess> Catch<TException>() where TException : Exception =>
            new FallibleFunction<TInput, TException, TSuccess>(
                x =>
                {
                    try
                    {
                        var result = _rawFunction(x);
                        return Fallible.Success<TException, TSuccess>(result);
                    }
                    catch (TException exception)
                    {
                        return Fallible.Failure<TException, TSuccess>(exception);
                    }
                });
    }
}
