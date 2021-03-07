using System;
using System.Collections.Generic;
using System.Text;
using LinqGarden.Functions;

namespace LinqGarden
{

    /// <summary>
    /// Provides a functional/deterministic approach for generating pseudo-random values
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Random<T>
    {
        internal Func<System.Random, T> Function { get; }

        internal Random(Func<System.Random, T> function)
        {
            Function = function;
        }

        /// <summary>
        /// Evaluate the random value with a seed.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public T Run(int seed)
        {
            var random = new System.Random(seed);

            return Function(random);
        }
    }


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

    /// <summary>
    /// Factory methods for primitive randoms
    /// </summary>
    public static class MakeRandom
    {
        private static Random<T> Make<T>(Func<Random, T> func) =>
            new Random<T>(func);

        public static Random<int> Next() =>
            Make(r => r.Next());

        public static Random<double> NextDouble() => Make(r => r.NextDouble());

        /// <summary>
        /// Provide an integer between the provided values
        /// Uses System.Random's Next
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Random<int> Next(int greaterOrEqual, int lessThan) =>
            Make(r => r.Next(greaterOrEqual, lessThan));


        public static Random<double> Between(double greaterOrEqual, double lessThan) =>
            NextDouble()
            .Select(x => x * (lessThan - greaterOrEqual))
            .Select(x => x + greaterOrEqual);

        public static Random<int> Between(int greaterOrEqual, int lessThanOrEqual) =>
            Next(greaterOrEqual, lessThanOrEqual + 1);
    }
   }
