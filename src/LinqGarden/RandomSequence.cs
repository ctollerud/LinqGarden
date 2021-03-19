using System;
using System.Collections.Generic;
using System.Linq;
using LinqGarden.Enumerables;

namespace LinqGarden
{
    public class RandomSequence<T>
    {
        internal Random<IEnumerable<T>> Random { get; }

        internal RandomSequence(Random<IEnumerable<T>> random)
        {
            Random = random;
        }
    }

    public static class RandomSequence
    {
        public static RandomSequence<T> AsRandomSequence<T>(this Random<IEnumerable<T>> input) =>
            new RandomSequence<T>(input);

        public static Random<IEnumerable<T>> AsRandom<T>(this RandomSequence<T> input) =>
            input.Random;

        public static Random<TNew> Collapse<T, TNew>(this RandomSequence<T> input, Func<IEnumerable<T>, TNew> collapser) =>
            input.AsRandom().Select(collapser);

        public static RandomSequence<T> AsRandomSequence<T>(this IEnumerable<Random<T>> input)
        {

            IEnumerable<T> ConcatImpl(Random random)
            {
                foreach (var value in input)
                {
                    yield return value.Function(random);
                }
            }

            return new Random<IEnumerable<T>>(ConcatImpl).AsRandomSequence();
        }

        public static RandomSequence<T> Return<T>(IEnumerable<T> input) =>
            MakeRandom.Return(input).AsRandomSequence();

        public static RandomSequence<T> Empty<T>() =>
            Return(Enumerable.Empty<T>());

        private static RandomSequence<TNew> TransformEnumerable<TOld, TNew>(this RandomSequence<TOld> input, Func<IEnumerable<TOld>, IEnumerable<TNew>> transform) =>
            input.AsRandom().Select(transform).AsRandomSequence();

        public static RandomSequence<T> Concat<T>(this RandomSequence<T> thisSequence, RandomSequence<T> nextSequence) =>
            (from first in thisSequence.AsRandom()
             from next in nextSequence.AsRandom()
             select first.Concat(next)
            ).AsRandomSequence();

        public static RandomSequence<T> StartWith<T>(this RandomSequence<T> input, T startWith) =>
            input.TransformEnumerable(x => x.StartWith(startWith));

        /// <summary>
        /// Convert the random into a single-item sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static RandomSequence<T> ToRandomSequence<T>(this Random<T> input) =>
            input.Select(x => new[] { x }.AsEnumerable()).AsRandomSequence();

        public static RandomSequence<T> StartWith<T>(this RandomSequence<T> input, Random<T> startWith) =>
            from startingValue in startWith.ToRandomSequence()
            from combined in input.StartWith(startingValue)
            select combined;

        public static RandomSequence<TSecond> Select<TFirst, TSecond>(
            this RandomSequence<TFirst> input,
            Func<TFirst, TSecond> transformation) =>
                input.Random.Select(sequence => sequence.Select(transformation))
                .AsRandomSequence();

        public static RandomSequence<TCombined> SelectMany<TFirst, TTransformed, TCombined>(
            this RandomSequence<TFirst> input,
            Func<TFirst, RandomSequence<TTransformed>> transformation,
            Func<TFirst, TTransformed, TCombined> combiner) =>
            (
                from items in input.AsRandom()
                let sequenceOfRandoms =
                    from item in items
                    let middleSequence = transformation(item)
                    select
                        from transformedItems in middleSequence.AsRandom()
                        select
                            from transformedItem in transformedItems
                            select combiner(item, transformedItem)
                from sequenceOfSequences in
                    sequenceOfRandoms.AsRandomSequence()
                    .AsRandom()
                let flattenedSequence = sequenceOfSequences.SelectMany(x => x)
                select flattenedSequence
            ).AsRandomSequence();

        public static RandomSequence<TCombined> SelectMany<TFirst, TTransformed, TCombined>(
            this RandomSequence<TFirst> input,
            Func<TFirst, Random<TTransformed>> transformation,
                Func<TFirst, TTransformed, TCombined> combiner) =>
                (
                        from items in input.AsRandom()
                        let sequenceOfRandoms =
                            from item in items
                            let middleRandom = transformation(item)
                            let fullyCombinedValue =
                                from middleValue in middleRandom
                                select combiner(item, middleValue)
                            select fullyCombinedValue
                        from randomSequence in sequenceOfRandoms.AsRandomSequence().AsRandom()
                        select randomSequence
                ).AsRandomSequence();

        public static RandomSequence<TCombined> SelectMany<TFirst, TTransformed, TCombined>(
            this RandomSequence<TFirst> input,
            Func<TFirst, IEnumerable<TTransformed>> transformation,
            Func<TFirst, TTransformed, TCombined> combiner) =>
            (
                from items in input.AsRandom()
                let sequenceOfCombined =
                    from item in items
                    from transformedItem in transformation(item)
                    select combiner(item, transformedItem)
                select sequenceOfCombined
            ).AsRandomSequence();



        public static RandomSequence<T> Repeat<T>(this Random<T> input) =>
            Sequence.Repeat(input).AsRandomSequence();

        public static RandomSequence<T> Repeat<T>(this Random<T> value, int numberOfRepeats) =>
            Enumerable.Repeat(value, numberOfRepeats).AsRandomSequence();
    }

}
