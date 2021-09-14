using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqGarden
{

    internal interface StateResult<TState, out TResult>
    {
        TState State { get; }
        TResult Value { get; }
    }

    /// <summary>
    /// Encapsulates a routine that uses a notion of state.  At it core it is a 
    /// function that takes a state, and returns a new state along with a final value.
    /// </summary>
    public interface State<TState,out TResult>
    {
        internal Func<TState,StateResult<TState, TResult>> RawFunc { get; }
    }

    internal record StateImpl<TState,TValue>(
        Func<TState,StateResult<TState,TValue>> RawFunc
        ) : State<TState,TValue>
    {
        Func<TState, StateResult<TState, TValue>> State<TState, TValue>.RawFunc => RawFunc;
    }

    internal record StateResultImpl<TState,TValue>(
        TState State,
        TValue Value 
        ) : StateResult<TState,TValue>
    {
        TState StateResult<TState, TValue>.State => State;
        TValue StateResult<TState, TValue>.Value => Value;
    }


	public static class State
	{
        internal static State<TState, TValue> MakeRaw<TState, TValue>(Func<TState, StateResult<TState, TValue>> func) =>
            new StateImpl<TState,TValue>(func);

        internal static StateResult<TState, TValue> MakeStateResult<TState, TValue>(TState state, TValue value) =>
            new StateResultImpl<TState, TValue>(state, value);

        public static State<TState, TValue> Return<TState, TValue>(TValue value) =>
            MakeRaw<TState, TValue>(state => MakeStateResult(state, value));

        public static State<TState, TValue> Return<TState, TValue>(Func<TValue> value) =>
            MakeRaw<TState,TValue>(state => MakeStateResult(state, value()));

        public static State<TState, TState> Get<TState>() =>
                MakeRaw<TState, TState>(state => MakeStateResult(state, state));

		public static State<TState, Unit> Put<TState>( TState newState ) =>
            MakeRaw<TState, Unit>(state => MakeStateResult(newState, Unit.Instance) );

        public static State<TState, TVal3> SelectMany<TState, TVal1, TVal2, TVal3>(
            this State<TState, TVal1> source,
            Func<TVal1, State<TState, TVal2>> midValue, Func<TVal1, TVal2, TVal3> resultSelector) =>
                MakeRaw<TState, TVal3>(stateVal1 =>
                {
                    var result1 = source.RawFunc.Invoke(stateVal1);

                    var midStateFunc = midValue(result1.Value).RawFunc;

                    var result2 = midStateFunc(result1.State);

                    return MakeStateResult(result2.State, resultSelector(result1.Value, result2.Value));

                });

		public static State<TState, TVal2> SelectMany<TState, TVal1, TVal2>(
			this State<TState, TVal1> source,
			Func<TVal1, State<TState, TVal2>> projection ) =>
			source.SelectMany( projection, ( _, y ) => y );

        public static State<TState, ICollection<TResultItem>> Concat<TState, TResultItem>(
            IEnumerable<State<TState, TResultItem>> states) =>
            MakeRaw<TState, ICollection<TResultItem>>(initialStateValue =>
            {
                TState currentStateValue = initialStateValue;
                IEnumerable<TResultItem> processItems()
                {
                    foreach (var state in states)
                    {
                        var stateResult = state.RawFunc(currentStateValue);
                        currentStateValue = stateResult.State;
                        yield return stateResult.Value;
                    }
                }

                var results = processItems().ToList();

                return MakeStateResult(currentStateValue, results);
            });

        /// <summary>
        /// Applies a transformation to the value of the input, leaving the 
        /// result state as it was.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TVal1"></typeparam>
        /// <typeparam name="TVal2"></typeparam>
        /// <param name="state"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static State<TState, TVal2> Select<TState, TVal1, TVal2>(this State<TState, TVal1> input, Func<TVal1, TVal2> map) =>
            MakeRaw<TState, TVal2>(state =>
            {
                var result1 = input.RawFunc(state);
                return MakeStateResult(result1.State, map(result1.Value));
            });

        public static (TState State, TValue Value) Run<TState,TValue> (this State<TState,TValue> input, TState initialState)
        {
            var rawResult = input.RawFunc(initialState);
            return (rawResult.State, rawResult.Value);
        }
	}
}
