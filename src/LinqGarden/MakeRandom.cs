using System;

namespace LinqGarden
{
    /// <summary>
    /// Factory methods for primitive randoms
    /// </summary>
    public static class MakeRandom
    {
        private static Random<T> Make<T>(Func<Random, T> func) =>
            new RandomImpl<T>(func);

        public static Random<T> Return<T>(T returnValue) => Make(_ => returnValue);

        public static Random<int> Next() =>
            Make(r => r.Next());

        public static Random<double> NextDouble() => Make(r => r.NextDouble());

        public static Random<bool> NextBool() => NextDouble().Select(x => x >= 0.5);

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
