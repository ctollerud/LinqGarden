using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqGarden.Enumerables
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> StartWith<T>(this IEnumerable<T> input, T start)
		{
			yield return start;
			foreach(var item in input)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Returns the sequence as a collection.
		/// If the input is already a collection, then no work is performed other than casting to an ICollection
		/// </summary>
		public static ICollection<T> AsCollection<T>( this IEnumerable<T> input )
		{
			return input switch
			{
				null => throw new ArgumentNullException( nameof( input ) ),
				ICollection<T> collection => collection,
				IEnumerable<T> anythingElse => anythingElse.ToList()
			};
		}
	}
}
