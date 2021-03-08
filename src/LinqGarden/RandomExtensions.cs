using System;
using LinqGarden.Functions;

namespace LinqGarden
{
    public static class RandomExtensions
    {
        private static Random<T> Make<T>(Func<Random, T> func) =>
            new Random<T>(func);

        public static Random<TNew> Select<TOld, TNew>(this Random<TOld> input, Func<TOld, TNew> transformation) =>
            Make(input.Function.Then(transformation));

        public static Random<TFinal> SelectMany<TStart, TMiddle, TFinal>(
            this Random<TStart> input,
            Func<TStart, Random<TMiddle>> transformation,
            Func<TStart, TMiddle, TFinal> resultSelector) =>
                Make(random =>
                {
                    var inputValue = input.Function(random);
                    var middleValue = transformation(inputValue).Function(random);
                    return resultSelector(inputValue, middleValue);
                });

        public static Random<TNew> SelectMany<TOld, TNew>(
            this Random<TOld> input,
            Func<TOld, Random<TNew>> transformation) =>
                Make(random => input.Function(random).Pipe(transformation).Function(random));
    }
   }
