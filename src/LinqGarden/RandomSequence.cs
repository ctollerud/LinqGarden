using System;
using System.Collections.Generic;
using System.Linq;
using LinqGarden.Enumerables;

namespace LinqGarden
{
    public class RandomSequence<T>
    {
        internal Random<IEnumerable<T>> Random { get; }

        internal RandomSequence( Random<IEnumerable<T>> random )
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



        public static RandomSequence<T> Repeat<T>( this Random<T> input) =>
            Sequence.Repeat(input).AsRandomSequence();

        public static RandomSequence<T> Repeat<T>( this Random<T> value, int numberOfRepeats) =>
            Enumerable.Repeat(value, numberOfRepeats).AsRandomSequence();
    }

}
