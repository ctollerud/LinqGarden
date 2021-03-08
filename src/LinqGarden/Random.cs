using System;
using System.Collections.Generic;
using System.Text;

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
   }
