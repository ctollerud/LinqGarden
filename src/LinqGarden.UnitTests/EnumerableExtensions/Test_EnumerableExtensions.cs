using FluentAssertions;
using LinqGarden.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LinqGarden.UnitTests.EnumerableExtensions
{
	public class Test_EnumerableExtensions
	{
		[Fact]
		public static void StartWith_ReturnsExpectedResults() =>
			new[] { 42, 43, 44 }
			.StartWith( 1337 )
			.Should().BeEquivalentTo( 
				new[] { 1337, 42, 43, 44 },
				o => o.WithStrictOrdering() );

		[Fact]
		public static void AsCollection_WhenInputIsNotACollection_ExpectedResultReturned() =>
			Enumerable.Range( 1, 3 )
			.AsCollection().Should().BeEquivalentTo( new[] { 1, 2, 3 }, o => o.WithStrictOrdering() );

		[Fact]
		public static void AsCollection_WhenInputIsAlreadyCollection_InputCollectionIsReturned()
		{
			var input = new[] { 1, 2, 3 };
			input.AsCollection().Should().BeSameAs( input );
		}

        [Fact]
        public static void Do_DoesNotPerformWork_UntilForEachCalled()
        {
            int sum = 0;
            IEnumerable<int> enumerable =
            new[]{ 1,2,3 }
            .Do(x =>
            {
                sum += x;
            });

            sum.Should().Be(0, "Do is lazy, so no work should have been performed yet.");

            enumerable.ForEach();

            sum.Should().Be(6, "The sequence has been evaluated so everything should have been added up.");
        }

        [Fact]
        public static void FirstOrNone_WhenSequenceIsEmpty_ResultIsNone() =>
            new string[] { }.FirstOrNone().Should().Be(Maybe.None<string>());

        [Fact]
        public static void FirstOrNone_WhenFirstValueIsNull_ResultIsNone() =>
            new string?[] { null, "abc" }.FirstOrNone().Should().Be(Maybe.None<string>());

        [Fact]
        public static void FirstOrNone_WhenFirstValueIsSet_ResultIsValue() =>
            new string?[] { "abc", "def" }.FirstOrNone().Should().Be(Maybe.Some( "abc" ));

        [Fact]
        public static void Repeat_WhenInputIsEmpty_EmptyEnumerableGetsReturned()
        {
            var result = new int[] { }.Repeat().ToList();

        }

        [Fact]
        public static void Repeat_WhenInputIsPopulated_ExpectedBehaviorOccurs()
        {
            var result = new int[] { 1,2,3 }.Repeat().Take(5).ToList();

            result.Should().BeEquivalentTo( new[] { 1, 2, 3, 1, 2 } );
        }

        [Fact]
        public static void Pairwise_WhenInputIsEmpty_ResultIsEmpty()
        {
            new int[] { }.Pairwise().Should().BeEmpty();
        }

        [Fact]
        public static void Pairwise_WhenInputHasOneItem_ResultIsEmpty()
        {
            new int[] { 42 }.Pairwise().Should().BeEmpty();
        }

        [Fact]
        public static void Pairwise_WhenInputHasTwoItems_ResultHasOneItem()
        {
            new int[] { 42, 43 }.Pairwise().Should().BeEquivalentTo(new[] { (42, 43) });
        }

        [Fact]
        public static void Pairwise_WhenInputHasFiveItems_ResultHasFourItems()
        {
            new int[] { 1,2,3,4,5 }.Pairwise().Should().BeEquivalentTo(new[] { (1,2), (2,3), (3,4), (4,5) });
        }

        [Fact]
        public static void Pairwise_WorksAppropriatelyWithReferenceTypes()
        {
            new [] { "abc","def", "ghi" }.Pairwise().Should().BeEquivalentTo(new[] { ( "abc", "def" ), ( "def", "ghi" ) });
        }

        [Fact]
        public static void Pairwise_WorksAppropriatelyWithNullableReferenceTypes()
        {
            new[] { null, "def", "ghi" }.Pairwise().Should().BeEquivalentTo(new[] { (null, "def"), ("def", "ghi") });
        }
    }
}
