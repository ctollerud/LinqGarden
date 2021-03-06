using System;
using System.Collections.Generic;

namespace LinqGarden
{
    /// <summary>
    /// Encapsulates a routine that uses a notion of state.  At it's core it is a 
    /// function that takes a state, and returns a new state along with a final value.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TValue"></typeparam>
	public class State<TState,TValue>
	{
        /// <summary>
        /// get the underlying function.  
        /// </summary>
		public Func<TState, Result> Func { get; }

		public class Result
		{
			public Result( TState state, TValue value )
			{
				State = state;
				Value = value;
			}

			public TState State { get; }
			public TValue Value { get; }
		}

		internal State( Func<TState,Result> stateFunc )
		{
			Func = stateFunc;
		}

		public static State<TState, TValue> Return( TValue value ) =>
			new State<TState, TValue>( state => new Result( state, value ) );

	}
	public static class State
	{
		public static State<TState, TValue> Return<TState, TValue>( TValue value ) =>
			State<TState, TValue>.Return( value );

		public static State<TState, TValue> Return<TState, TValue>( Func<TValue> value ) =>
			new State<TState, TValue>( s => new State<TState, TValue>.Result( state: s, value: value() ) );

		public static State<TState,TState> Get<TState>() =>
				new State<TState,TState>( state => new State<TState,TState>.Result( state, state ) );

		public static State<TState, Unit> Put<TState>( TState state ) =>
			new State<TState, Unit>( _ => new State<TState, Unit>.Result( state, Unit.Instance ) );

		public static State<TState, TVal3> SelectMany<TState,TVal1, TVal2, TVal3>( 
			this State<TState,TVal1> source, 
			Func<TVal1, State<TState,TVal2>> midValue, Func<TVal1, TVal2, TVal3> resultSelector )
		{
			State<TState,TVal3>.Result NewRunState( TState state )
			{
				var firstResult = source.Func( state );

				var middleResult = midValue( firstResult.Value ).Func( firstResult.State );

				var finalResult = resultSelector( firstResult.Value, middleResult.Value );

				return new State<TState, TVal3>.Result( middleResult.State, finalResult );
			}

			return new State<TState, TVal3>( NewRunState );
		}

		public static State<TState, TVal2> SelectMany<TState, TVal1, TVal2>(
			this State<TState, TVal1> source,
			Func<TVal1, State<TState, TVal2>> projection ) =>
			source.SelectMany( projection, ( _, y ) => y );

		public static State<T1, ICollection<T2>> Concat<T1, T2>( IEnumerable<State<T1, T2>> states )
		{
			State<T1, ICollection<T2>>.Result ConcatImpl( T1 initial )
			{
				var current = initial;
				List<T2> aggregate = new List<T2>();

				foreach( var state in states )
				{
					var midResult = state.Func( current );
					aggregate.Add( midResult.Value );
					current = midResult.State;
				}

				return new State<T1, ICollection<T2>>.Result( current, (ICollection<T2>)aggregate );
			}

			return new State<T1, ICollection<T2>>( ConcatImpl );
		}

        /// <summary>
        /// Applies a transformation to the value of the first state, leaving the 
        /// result state as it was.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TVal1"></typeparam>
        /// <typeparam name="TVal2"></typeparam>
        /// <param name="state"></param>
        /// <param name="map"></param>
        /// <returns></returns>
		public static State<TState, TVal2> Select<TState, TVal1, TVal2>( this State<TState, TVal1> state, Func<TVal1, TVal2> map )
		{
			State<TState,TVal2>.Result StateFunction( TState startState )
			{
				var x = state.Func( startState );

				return new State<TState, TVal2>.Result( x.State, map( x.Value ) );
			}
			return new State<TState, TVal2>( StateFunction );
		}
	}
}
