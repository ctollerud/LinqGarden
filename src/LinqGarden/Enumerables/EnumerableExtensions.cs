using System;
using System.Collections.Generic;
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
	}
}
