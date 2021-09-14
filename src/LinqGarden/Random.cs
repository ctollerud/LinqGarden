using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden
{
    public interface Random<out T>
    {
        internal Func<Random,T> RawFunc { get; }
    }

    internal record RandomImpl<T>(
        Func<Random,T> RawFunc
        ) : Random<T>
    {
        Func<Random, T> Random<T>.RawFunc => RawFunc;
    }
}
