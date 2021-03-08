using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
    public static class ValueTupleExtensions
    {
        public static (T1, T2, T3) Append<T1, T2, T3>(this (T1, T2) input, T3 newValue) =>
            (input.Item1, input.Item2, newValue);

        public static (T1, T2, T3, T4) Append<T1, T2, T3, T4>(this (T1, T2, T3) input, T4 newValue) =>
            (input.Item1, input.Item2, input.Item3, newValue);
    }
}
