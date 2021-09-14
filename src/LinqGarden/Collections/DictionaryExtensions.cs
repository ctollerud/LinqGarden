using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqGarden.Collections
{
    public static class DictionaryExtensions
    {
        public static Maybe<TValue> TryGetValue<TKey,TValue>( 
            this IReadOnlyDictionary<TKey,TValue> dictionary,
            TKey key )
        {
            if( dictionary.TryGetValue( key, out TValue? value ) )
            {
                return Maybe.Some(value);
            }
            return Maybe.None<TValue>();
        }
    }
}
