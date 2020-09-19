using System;
using System.Collections.Generic;
using System.Text;

namespace LinqGarden.Strings
{
    public static class StringExtensions
    {
        /// <summary>
        /// Evaluate the enumerable, joining the items into a single string using the 
        /// provided separator.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinStrings(this IEnumerable<string> input, string separator) =>
            string.Join(separator, input);

        /// <summary>
        /// Converts the string to None if null or empty, otherwise Some.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Maybe<string> NoneIfEmpty(this string input) =>
            string.IsNullOrEmpty(input) ? Maybe.None<string>() : Maybe.Some(input);
    }
}
